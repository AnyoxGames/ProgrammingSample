using System;
using AnyoxGames.Util;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AnyoxGames.CameraSystem
{
    [Serializable]
    public class FirstPersonCameraBehaviour : CameraBehaviour
    {
        [SerializeField] private float maxInteractableDistance;
        [SerializeField] private LayerMask interactableMask = -1;

        [SerializeField, Range(-90f, 90f)] private float minVerticalAngle = -90f;
        [SerializeField, Range(-90f, 90f)] private float maxVerticalAngle = 90f;

        [SerializeField] private float rotationSpeed = 1f;
        [SerializeField] private float rotationSharpness = 10000f;

        [SerializeField] private InputAction lookAction;

        private float targetVerticalAngle;
        private Vector2 lookInputDelta;
        private RaycastHit[] interactHits = new RaycastHit[10];

        public override void EnterState(GameCamera target)
        {
            lookAction.performed += OnLook;
            lookAction.canceled += OnLook;
            lookAction.Enable();
        }

        public override void ExitState(GameCamera target)
        {
            lookAction.performed -= OnLook;
            lookAction.canceled -= OnLook;
            lookAction.Disable();
        }

        protected override void OnUpdate(GameCamera target)
        {
            if (target.CurrentTarget == null)
                return;

            target.transform.position = target.CurrentTarget.GetTransformCameraTarget().position;
            target.transform.forward = target.CurrentTarget.GetTransformCameraTarget().forward;

            targetVerticalAngle -= lookInputDelta.y * rotationSpeed;
            targetVerticalAngle = Mathf.Clamp(targetVerticalAngle, minVerticalAngle, maxVerticalAngle);
            target.transform.rotation = Quaternion.Slerp(target.transform.rotation, target.transform.rotation * Quaternion.Euler(targetVerticalAngle, 0, 0), 1f - Mathf.Exp(-rotationSharpness * Time.deltaTime));

            if (target.CurrentTarget.TryGetCharacterCameraTarget(out var character))
            {
                character.transform.Rotate(new Vector3(0f, lookInputDelta.x * rotationSpeed, 0f), Space.Self);
            }
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

        public override IInteractable FindInteractable(GameCamera target)
        {
            if (target.CurrentTarget.TryGetCharacterCameraTarget(out var character))
            {
                var count = Physics.RaycastNonAlloc(target.transform.position, target.transform.forward, interactHits, maxInteractableDistance, interactableMask);

                for (int i = 0; i < count; i++)
                    if (interactHits[i].collider.TryGetComponent(out IInteractable interactable))
                    {
                        character.SetCurrentInteractable(interactable);
                        return interactable;
                    }

                character.SetCurrentInteractable(null);
            }

            return null;
        }
    }
}