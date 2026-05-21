using AnyoxGames.Util;
using UnityEngine;
using UnityEngine.InputSystem;

[CharacterBehaviour(-100)]
public class CharacterInputBehaviour : ACharacterBehaviour
{
    [SerializeField] private InputAction MoveAction;
    [SerializeField] private InputAction JumpAction;
    [SerializeField] private InputAction SprintAction;
    [SerializeField] private InputAction WalkAction;
    [SerializeField] private InputAction CrouchAction;

    public Vector2 MoveInput { get; set; }
    public bool JumpRequested { get; set; }
    public bool SprintRequested { get; set; }
    public bool WalkRequested { get; set; }
    public bool CrouchRequested { get; set; }
    
    private void Awake()
    {
        MoveAction.performed += OnMove;
        MoveAction.canceled += OnMove;
        MoveAction.Enable();
        JumpAction.performed += OnJump;
        JumpAction.Enable();
        SprintAction.performed += OnSprint;
        SprintAction.canceled += OnSprint;
        SprintAction.Enable();
        WalkAction.performed += OnWalk;
        WalkAction.canceled += OnWalk;
        WalkAction.Enable();
        CrouchAction.performed += OnCrouch;
        CrouchAction.canceled += OnCrouch;
        CrouchAction.Enable();
    }

    private void OnDestroy()
    {
        MoveAction.performed -= OnMove;
        MoveAction.canceled -= OnMove;
        MoveAction.Disable();
        JumpAction.performed -= OnJump;
        JumpAction.Disable();
        SprintAction.performed -= OnSprint;
        SprintAction.canceled -= OnSprint;
        SprintAction.Disable();
        WalkAction.performed -= OnWalk;
        WalkAction.canceled -= OnWalk;
        WalkAction.Disable();
        CrouchAction.performed -= OnCrouch;
        CrouchAction.canceled -= OnCrouch;
        CrouchAction.Disable();
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