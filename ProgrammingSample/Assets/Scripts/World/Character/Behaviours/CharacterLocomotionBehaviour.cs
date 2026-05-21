using System;
using UnityEngine;

namespace AnyoxGames.Character
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterLocomotionBehaviour : ACharacterBehaviour
    {
        private const float JUMP_GROUNDING_PREVENTION_TIME = 0.2f;
        private const float GROUND_CHECK_DISTANCE_IN_AIR = 0.07f;

        [SerializeField] private float maxSpeed = 7f;
        [SerializeField] private float movementSharpnessOnGround = 15;
        [SerializeField] private float sprintSpeedMultiplier = 1.6f;
        [SerializeField] private float jumpForce = 7f;
        [SerializeField] private float crouchSpeedMultiplier = 0.4f;
        [SerializeField] private float maxSpeedInAir = 7f;
        [SerializeField] private float accelerationSpeedInAir = 6f;
        [SerializeField] private float gravityDownForce = 20f;
        [SerializeField] private float groundCheckDistance = 0.05f;
        [SerializeField] private LayerMask groundCheckLayers = -1;
        [SerializeField] private float capsuleHeightCrouching = 0.9f;
        [SerializeField] private float capsuleHeightStanding = 2f;
        [SerializeField] private float crouchingSharpness = 10f;
        [SerializeField] private Transform cameraTarget;

        private bool landedThisFrame;
        private bool jumpedThisFrame;
        private bool leftGroundThisFrame;
        private bool isCrouching;
        private bool isGrounded;
        private bool isSprinting;
        private CharacterController characterController;
        private CharacterInputBehaviour inputHandler;
        private Vector3 groundNormal;
        private Vector3 characterVelocity;
        private Vector3 latestImpactSpeed;
        private float lastTimeJumped;
        private float footstepDistanceCounter;
        private float targetCharacterHeight;

        public bool IsGrounded => isGrounded;
        public Vector3 Velocity => characterVelocity;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void Start()
        {
            // force the crouch state to false when starting
            SetCrouchingState(false, true);
            UpdateCharacterHeight(true);
            TryGetCharacterBehaviour(out inputHandler);
        }

        public override void OnUpdate()
        {
            if (inputHandler == null)
            {
                return;
            }

            GroundCheck();
            SetCrouchingState(inputHandler.CrouchRequested, false);
            UpdateCharacterHeight(false);
            HandleCharacterMovement();
        }

        private void HandleCharacterMovement()
        {
            // character movement handling
            bool isSprinting = inputHandler.SprintRequested;
            {
                if (isSprinting)
                {
                    isSprinting = SetCrouchingState(false, false);
                }

                float speedModifier = isSprinting ? sprintSpeedMultiplier : 1f;

                // converts move input to a worldspace vector based on our character's transform orientation
                Vector3 worldspaceMoveInput = transform.TransformVector(new Vector3(inputHandler.MoveInput.x, 0, inputHandler.MoveInput.y));

                // handle grounded movement
                if (isGrounded)
                {
                    // calculate the desired velocity from inputs, max speed, and current slope
                    Vector3 targetVelocity = worldspaceMoveInput * (maxSpeed * speedModifier);

                    // reduce speed if crouching by crouch speed ratio
                    if (isCrouching)
                        targetVelocity *= crouchSpeedMultiplier;

                    targetVelocity = GetDirectionReorientedOnSlope(targetVelocity.normalized, groundNormal) * targetVelocity.magnitude;

                    // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
                    characterVelocity = Vector3.Lerp(characterVelocity, targetVelocity,
                        movementSharpnessOnGround * Time.deltaTime);

                    // jumping
                    if (inputHandler.JumpRequested)
                    {
                        inputHandler.ConsumeJumpRequest();

                        // force the crouch state to false
                        if (isGrounded && SetCrouchingState(false, false))
                        {
                            // start by canceling out the vertical component of our velocity
                            characterVelocity = new Vector3(characterVelocity.x, 0f, characterVelocity.z);

                            // then, add the jumpSpeed value upwards
                            characterVelocity += Vector3.up * jumpForce;

                            // remember last time we jumped because we need to prevent snapping to ground for a short time
                            lastTimeJumped = Time.time;

                            // Force grounding to false
                            isGrounded = false;
                            groundNormal = Vector3.up;
                        }
                    }

                    // keep track of distance traveled for footsteps sound
                    footstepDistanceCounter += characterVelocity.magnitude * Time.deltaTime;
                }
                // handle air movement
                else
                {
                    // add air acceleration
                    characterVelocity += worldspaceMoveInput * (accelerationSpeedInAir * Time.deltaTime);

                    // limit air speed to a maximum, but only horizontally
                    float verticalVelocity = characterVelocity.y;
                    Vector3 horizontalVelocity = Vector3.ProjectOnPlane(characterVelocity, Vector3.up);
                    horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, maxSpeedInAir * speedModifier);
                    characterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);

                    // apply the gravity to the velocity
                    characterVelocity += Vector3.down * (gravityDownForce * Time.deltaTime);
                }
            }

            // apply the final calculated velocity value as a character movement
            Vector3 capsuleBottomBeforeMove = GetCapsuleBottomHemisphere();
            Vector3 capsuleTopBeforeMove = GetCapsuleTopHemisphere(characterController.height);
            characterController.Move(characterVelocity * Time.deltaTime);

            // detect obstructions to adjust velocity accordingly
            latestImpactSpeed = Vector3.zero;
            if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, characterController.radius,
                    characterVelocity.normalized, out RaycastHit hit, characterVelocity.magnitude * Time.deltaTime, -1,
                    QueryTriggerInteraction.Ignore))
            {
                // We remember the last impact speed because the fall damage logic might need it
                latestImpactSpeed = characterVelocity;

                characterVelocity = Vector3.ProjectOnPlane(characterVelocity, hit.normal);
            }
        }

        // Gets the center point of the bottom hemisphere of the character controller capsule    
        private Vector3 GetCapsuleBottomHemisphere()
        {
            return transform.position + (transform.up * characterController.radius);
        }

        private void UpdateCharacterHeight(bool force)
        {
            // Update height instantly
            if (force)
            {
                characterController.height = targetCharacterHeight;
                characterController.center = Vector3.up * (characterController.height * 0.5f);
            }
            // Update smooth height
            else if (characterController.height != targetCharacterHeight)
            {
                // resize the capsule and adjust camera position
                characterController.height = Mathf.Lerp(characterController.height, targetCharacterHeight, crouchingSharpness * Time.deltaTime);
                characterController.center = Vector3.up * (characterController.height * 0.5f);
            }

            cameraTarget.localPosition = Vector3.up * (characterController.height * 0.9f);
        }

        private void GroundCheck()
        {
            // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
            float chosenGroundCheckDistance = isGrounded ? (characterController.skinWidth + groundCheckDistance) : GROUND_CHECK_DISTANCE_IN_AIR;

            // reset values before the ground check
            isGrounded = false;
            groundNormal = Vector3.up;

            // only try to detect ground if it's been a short amount of time since last jump; otherwise we may snap to the ground instantly after we try jumping
            if (Time.time >= lastTimeJumped + JUMP_GROUNDING_PREVENTION_TIME)
            {
                // if we're grounded, collect info about the ground normal with a downward capsule cast representing our character capsule
                if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(characterController.height), characterController.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, groundCheckLayers, QueryTriggerInteraction.Ignore))
                {
                    // storing the upward direction for the surface found
                    groundNormal = hit.normal;

                    // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
                    // and if the slope angle is lower than the character controller's limit
                    if (Vector3.Dot(hit.normal, transform.up) > 0f && IsNormalUnderSlopeLimit(groundNormal))
                    {
                        isGrounded = true;

                        // handle snapping to the ground
                        if (hit.distance > characterController.skinWidth)
                        {
                            characterController.Move(Vector3.down * hit.distance);
                        }
                    }
                }
            }
        }

        // Returns true if the slope angle represented by the given normal is under the slope angle limit of the character controller
        bool IsNormalUnderSlopeLimit(Vector3 normal)
        {
            return Vector3.Angle(transform.up, normal) <= characterController.slopeLimit;
        }

        // Gets a reoriented direction that is tangent to a given slope
        private Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
        {
            Vector3 directionRight = Vector3.Cross(direction, transform.up);
            return Vector3.Cross(slopeNormal, directionRight).normalized;
        }

        // Gets the center point of the top hemisphere of the character controller capsule    
        Vector3 GetCapsuleTopHemisphere(float atHeight)
        {
            return transform.position + (transform.up * (atHeight - characterController.radius));
        }

        // returns false if there was an obstruction
        private bool SetCrouchingState(bool crouched, bool ignoreObstructions)
        {
            // set appropriate heights
            if (crouched)
            {
                targetCharacterHeight = capsuleHeightCrouching;
            }
            else
            {
                // Detect obstructions
                if (!ignoreObstructions)
                {
                    Collider[] standingOverlaps = Physics.OverlapCapsule(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(capsuleHeightStanding), characterController.radius, -1, QueryTriggerInteraction.Ignore);

                    foreach (Collider c in standingOverlaps)
                    {
                        if (c != characterController)
                        {
                            return false;
                        }
                    }
                }

                targetCharacterHeight = capsuleHeightStanding;
            }

            isCrouching = crouched;
            return true;
        }
    }
}