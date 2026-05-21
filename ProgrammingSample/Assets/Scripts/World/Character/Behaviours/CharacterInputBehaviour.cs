using AnyoxGames.Util;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AnyoxGames.Character
{
    [CharacterBehaviour(-100)]
    public class CharacterInputBehaviour : ACharacterBehaviour
    {
        [SerializeField] private InputAction moveAction;
        [SerializeField] private InputAction jumpAction;
        [SerializeField] private InputAction sprintAction;
        [SerializeField] private InputAction walkAction;
        [SerializeField] private InputAction crouchAction;

        public Vector2 MoveInput { get; private set; }
        public bool JumpRequested { get; private set; }
        public bool SprintRequested { get; private set; }
        public bool WalkRequested { get; private set; }
        public bool CrouchRequested { get; private set; }

        private void Awake()
        {
            moveAction.performed += OnMove;
            moveAction.canceled += OnMove;
            sprintAction.performed += OnSprint;
            sprintAction.canceled += OnSprint;
            walkAction.performed += OnWalk;
            walkAction.canceled += OnWalk;
            crouchAction.performed += OnCrouch;
            crouchAction.canceled += OnCrouch;
            jumpAction.performed += OnJump;
            moveAction.Enable();
            sprintAction.Enable();
            walkAction.Enable();
            crouchAction.Enable();
            jumpAction.Enable();
        }

        private void OnDestroy()
        {
            moveAction.performed -= OnMove;
            moveAction.canceled -= OnMove;
            sprintAction.performed -= OnSprint;
            sprintAction.canceled -= OnSprint;
            walkAction.performed -= OnWalk;
            walkAction.canceled -= OnWalk;
            crouchAction.performed -= OnCrouch;
            crouchAction.canceled -= OnCrouch;
            jumpAction.performed -= OnJump;
            moveAction.Disable();
            sprintAction.Disable();
            walkAction.Disable();
            crouchAction.Disable();
            jumpAction.Disable();
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            if (!CursorUtils.IsCursorCaptured)
            {
                MoveInput = Vector2.zero;
                return;
            }

            MoveInput = context.ReadValue<Vector2>();
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            if (!CursorUtils.IsCursorCaptured)
            {
                return;
            }

            JumpRequested = context.ReadValueAsButton();
        }

        private void OnSprint(InputAction.CallbackContext context)
        {
            if (!CursorUtils.IsCursorCaptured)
            {
                SprintRequested = false;
                return;
            }

            SprintRequested = context.ReadValueAsButton();
        }

        private void OnWalk(InputAction.CallbackContext context)
        {
            if (!CursorUtils.IsCursorCaptured)
            {
                WalkRequested = false;
                return;
            }

            WalkRequested = context.ReadValueAsButton();
        }

        private void OnCrouch(InputAction.CallbackContext context)
        {
            if (!CursorUtils.IsCursorCaptured)
            {
                CrouchRequested = false;
                return;
            }

            CrouchRequested = context.ReadValueAsButton();
        }

        public void ConsumeJumpRequest()
        {
            JumpRequested = false;
        }
    }
}