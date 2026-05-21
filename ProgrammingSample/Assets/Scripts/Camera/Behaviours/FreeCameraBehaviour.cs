using System;
using AnyoxGames.Util;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AnyoxGames.CameraSystem
{
    [Serializable]
    public class FreeCameraBehaviour : CameraBehaviour
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private float moveSpeedSharpness;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float rotationSharpness;

        [SerializeField] private InputAction moveAction;
        [SerializeField] private InputAction lookAction;

        private Vector3 moveInputDelta;
        private Vector2 lookInputDelta;
        private Vector3 velocity;
        private Vector3 targetVelocity;
        private Vector3 planarDirection;
        private float targetVerticalAngle;

        public override void EnterState(GameCamera target)
        {
            moveAction.performed += OnMove;
            moveAction.canceled += OnMove;
            moveAction.Enable();
            lookAction.performed += OnLook;
            lookAction.canceled += OnLook;
            lookAction.Enable();

            planarDirection = target.transform.forward;
        }

        protected override void OnUpdate(GameCamera target)
        {
            targetVelocity = new Vector3(moveInputDelta.x, 0, moveInputDelta.z) * moveSpeed;

            // Process rotation input
            Quaternion rotationFromInput = Quaternion.Euler(Vector3.up * (lookInputDelta.x * rotationSpeed));
            planarDirection = rotationFromInput * planarDirection;
            planarDirection = Vector3.Cross(Vector3.up, Vector3.Cross(planarDirection, Vector3.up));
            Quaternion planarRot = Quaternion.LookRotation(planarDirection, Vector3.up);

            targetVerticalAngle -= (lookInputDelta.y * rotationSpeed);
            targetVerticalAngle = Mathf.Clamp(targetVerticalAngle, -90, 90);
            Quaternion verticalRot = Quaternion.Euler(targetVerticalAngle, 0, 0);
            Quaternion targetRotation = Quaternion.Slerp(target.transform.rotation, planarRot * verticalRot, 1f - Mathf.Exp(-rotationSharpness * Time.deltaTime));

            // Apply rotation
            target.transform.rotation = targetRotation;

            velocity = Vector3.Lerp(velocity, targetVelocity, 1f - Mathf.Exp(-moveSpeedSharpness * Time.deltaTime));
            target.transform.Translate(velocity * Time.deltaTime);
        }

        public override void ExitState(GameCamera target)
        {
            moveAction.performed -= OnMove;
            moveAction.canceled -= OnMove;
            moveAction.Disable();
            lookAction.performed -= OnLook;
            lookAction.canceled -= OnLook;
            lookAction.Disable();
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();
            moveInputDelta.Set(value.x, 0, value.y);
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            if (!CursorUtils.IsCursorCaptured)
            {
                lookInputDelta = Vector2.zero;
                return;
            }

            var vector = context.ReadValue<Vector2>();

            if (context.control.device is Mouse)
            {
                //TODO Make this in config file
                vector.x *= 0.05f;
                vector.y *= 0.05f;
            }

            lookInputDelta = new Vector2(vector.x, vector.y);
        }
    }
}