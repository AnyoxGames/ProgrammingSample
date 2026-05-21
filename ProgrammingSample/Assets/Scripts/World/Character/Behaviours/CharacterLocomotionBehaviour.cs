using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterLocomotionBehaviour : ACharacterBehaviour
{
    private const float JUMP_GROUNDING_PREVENTION_TIME = 0.2f;
    private const float GROUND_CHECK_DISTANCE_IN_AIR = 0.07f;
    
    [Tooltip("Max movement speed when grounded (when not sprinting)")]
    public float MaxSpeed = 10f;
    [Tooltip("Sharpness for the movement when grounded, a low value will make the player accelerate and decelerate slowly, a high value will do the opposite")]
    public float MovementSharpnessOnGround = 15;
    [Tooltip("Multiplicator for the sprint speed (based on grounded speed)")]
    public float SprintSpeedMultiplier = 2f;
    [Tooltip("Force applied upward when jumping")]
    public float JumpForce = 9f;
    [Tooltip("Max movement speed when crouching")]
    public float CrouchSpeedMultiplier = 0.5f;
    [Tooltip("Max movement speed when not grounded")]
    public float MaxSpeedInAir = 10f;
    [Tooltip("Acceleration speed when in the air")]
    public float AccelerationSpeedInAir = 25f;
    [Tooltip("Force applied downward when in the air")]
    public float GravityDownForce = 20f;
    [Tooltip("distance from the bottom of the character controller capsule to test for grounded")]
    public float GroundCheckDistance = 0.05f;
    [Tooltip("Physic layers checked to consider the player grounded")]
    public LayerMask GroundCheckLayers = -1;
    [Tooltip("Height of character when crouching")]
    public float CapsuleHeightCrouching = 0.9f;
    [Tooltip("Height of character when standing")]
    public float CapsuleHeightStanding = 2f;
    [Tooltip("Speed of crouching transitions")]
    public float CrouchingSharpness = 10f;

    public Transform CameraTarget;

    private bool LandedThisFrame;
    private bool JumpedThisFrame;
    private bool LeftGroundThisFrame;
    private bool IsCrouching;
    private bool isGrounded;
    private bool isSprinting;
    private CharacterController CharacterController;
    private CharacterInputBehaviour InputHandler;
    
    private Vector3 GroundNormal;
    private Vector3 CharacterVelocity;
    private Vector3 LatestImpactSpeed;
    private float LastTimeJumped = 0f;
    private float FootstepDistanceCounter;
    private float TargetCharacterHeight;

    public bool IsGrounded => isGrounded;
    public bool IsSprinting => isSprinting;
    public Vector3 Velocity => CharacterVelocity;
    
    private void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        // force the crouch state to false when starting
        SetCrouchingState(false, true);
        UpdateCharacterHeight(true);
        TryGetCharacterBehaviour(out InputHandler);
    }

    public override void OnUpdate()
    {
        if (InputHandler == null)
        {
            return;
        }

        bool wasGrounded = isGrounded;
        GroundCheck();

        // landing
        if (isGrounded && !wasGrounded)
        {
            /*// Fall damage
            float fallSpeed = -Mathf.Min(CharacterVelocity.y, m_LatestImpactSpeed.y);
            float fallSpeedRatio = (fallSpeed - MinSpeedForFallDamage) / (MaxSpeedForFallDamage - MinSpeedForFallDamage);
            if (RecievesFallDamage && fallSpeedRatio > 0f)
            {
                float dmgFromFall = Mathf.Lerp(FallDamageAtMinSpeed, FallDamageAtMaxSpeed, fallSpeedRatio);
                m_Health.TakeDamage(dmgFromFall, null);

                // fall damage SFX
                AudioSource.PlayOneShot(FallDamageSfx);
            }
            else
            {
                // land SFX
                AudioSource.PlayOneShot(LandSfx);
            }*/
        }

        SetCrouchingState(InputHandler.CrouchRequested, false);
        
        UpdateCharacterHeight(false);
        HandleCharacterMovement();
    }

    private void HandleCharacterMovement()
    {
        // character movement handling
        bool isSprinting = InputHandler.SprintRequested;
        {
            if (isSprinting)
            {
                isSprinting = SetCrouchingState(false, false);
            }

            float speedModifier = isSprinting ? SprintSpeedMultiplier : 1f;

            // converts move input to a worldspace vector based on our character's transform orientation
            Vector3 worldspaceMoveInput = transform.TransformVector(new Vector3(InputHandler.MoveInput.x, 0, InputHandler.MoveInput.y));

            // handle grounded movement
            if (isGrounded)
            {
                // calculate the desired velocity from inputs, max speed, and current slope
                Vector3 targetVelocity = worldspaceMoveInput * (MaxSpeed * speedModifier);
                
                // reduce speed if crouching by crouch speed ratio
                if (IsCrouching)
                    targetVelocity *= CrouchSpeedMultiplier;
                
                targetVelocity = GetDirectionReorientedOnSlope(targetVelocity.normalized, GroundNormal) * targetVelocity.magnitude;

                // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
                CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVelocity,
                    MovementSharpnessOnGround * Time.deltaTime);

                // jumping
                if (InputHandler.JumpRequested)
                {
                    InputHandler.ConsumeJumpRequest();
                    
                    // force the crouch state to false
                    if (isGrounded && SetCrouchingState(false, false))
                    {
                        // start by canceling out the vertical component of our velocity
                        CharacterVelocity = new Vector3(CharacterVelocity.x, 0f, CharacterVelocity.z);

                        // then, add the jumpSpeed value upwards
                        CharacterVelocity += Vector3.up * JumpForce;

                        // remember last time we jumped because we need to prevent snapping to ground for a short time
                        LastTimeJumped = Time.time;

                        // Force grounding to false
                        isGrounded = false;
                        GroundNormal = Vector3.up;
                    }
                }

                // keep track of distance traveled for footsteps sound
                FootstepDistanceCounter += CharacterVelocity.magnitude * Time.deltaTime;
            }
            // handle air movement
            else
            {
                // add air acceleration
                CharacterVelocity += worldspaceMoveInput * (AccelerationSpeedInAir * Time.deltaTime);

                // limit air speed to a maximum, but only horizontally
                float verticalVelocity = CharacterVelocity.y;
                Vector3 horizontalVelocity = Vector3.ProjectOnPlane(CharacterVelocity, Vector3.up);
                horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, MaxSpeedInAir * speedModifier);
                CharacterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);

                // apply the gravity to the velocity
                CharacterVelocity += Vector3.down * (GravityDownForce * Time.deltaTime);
            }
        }

        // apply the final calculated velocity value as a character movement
        Vector3 capsuleBottomBeforeMove = GetCapsuleBottomHemisphere();
        Vector3 capsuleTopBeforeMove = GetCapsuleTopHemisphere(CharacterController.height);
        CharacterController.Move(CharacterVelocity * Time.deltaTime);

        // detect obstructions to adjust velocity accordingly
        LatestImpactSpeed = Vector3.zero;
        if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, CharacterController.radius,
                CharacterVelocity.normalized, out RaycastHit hit, CharacterVelocity.magnitude * Time.deltaTime, -1,
                QueryTriggerInteraction.Ignore))
        {
            // We remember the last impact speed because the fall damage logic might need it
            LatestImpactSpeed = CharacterVelocity;

            CharacterVelocity = Vector3.ProjectOnPlane(CharacterVelocity, hit.normal);
        }
    }
    
    // Gets the center point of the bottom hemisphere of the character controller capsule    
    private Vector3 GetCapsuleBottomHemisphere()
    {
        return transform.position + (transform.up * CharacterController.radius);
    }

    private void UpdateCharacterHeight(bool force)
    {
        // Update height instantly
        if (force)
        {
            CharacterController.height = TargetCharacterHeight;
            CharacterController.center = Vector3.up * (CharacterController.height * 0.5f);
        }
        // Update smooth height
        else if (CharacterController.height != TargetCharacterHeight)
        {
            // resize the capsule and adjust camera position
            CharacterController.height = Mathf.Lerp(CharacterController.height, TargetCharacterHeight, CrouchingSharpness * Time.deltaTime);
            CharacterController.center = Vector3.up * (CharacterController.height * 0.5f);
        }
        
        CameraTarget.localPosition = Vector3.up * (CharacterController.height * 0.9f);
    }

    private void GroundCheck()
    {
        // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
        float chosenGroundCheckDistance = isGrounded ? (CharacterController.skinWidth + GroundCheckDistance) : GROUND_CHECK_DISTANCE_IN_AIR;

        // reset values before the ground check
        isGrounded = false;
        GroundNormal = Vector3.up;

        // only try to detect ground if it's been a short amount of time since last jump; otherwise we may snap to the ground instantly after we try jumping
        if (Time.time >= LastTimeJumped + JUMP_GROUNDING_PREVENTION_TIME)
        {
            // if we're grounded, collect info about the ground normal with a downward capsule cast representing our character capsule
            if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(CharacterController.height), CharacterController.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, GroundCheckLayers, QueryTriggerInteraction.Ignore))
            {
                // storing the upward direction for the surface found
                GroundNormal = hit.normal;

                // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
                // and if the slope angle is lower than the character controller's limit
                if (Vector3.Dot(hit.normal, transform.up) > 0f && IsNormalUnderSlopeLimit(GroundNormal))
                {
                    isGrounded = true;

                    // handle snapping to the ground
                    if (hit.distance > CharacterController.skinWidth)
                    {
                        CharacterController.Move(Vector3.down * hit.distance);
                    }
                }
            }
        }
    }

    // Returns true if the slope angle represented by the given normal is under the slope angle limit of the character controller
    bool IsNormalUnderSlopeLimit(Vector3 normal)
    {
        return Vector3.Angle(transform.up, normal) <= CharacterController.slopeLimit;
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
        return transform.position + (transform.up * (atHeight - CharacterController.radius));
    }
    
    // returns false if there was an obstruction
    private bool SetCrouchingState(bool crouched, bool ignoreObstructions)
    {
        // set appropriate heights
        if (crouched)
        {
            TargetCharacterHeight = CapsuleHeightCrouching;
        }
        else
        {
            // Detect obstructions
            if (!ignoreObstructions)
            {
                Collider[] standingOverlaps = Physics.OverlapCapsule(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(CapsuleHeightStanding), CharacterController.radius, -1, QueryTriggerInteraction.Ignore);
                
                foreach (Collider c in standingOverlaps)
                {
                    if (c != CharacterController)
                    {
                        return false;
                    }
                }
            }

            TargetCharacterHeight = CapsuleHeightStanding;
        }

        /*if (OnStanceChanged != null)
        {
            OnStanceChanged.Invoke(crouched);
        }*/

        IsCrouching = crouched;
        return true;
    }
}