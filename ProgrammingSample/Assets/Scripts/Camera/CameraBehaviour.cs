using AnyoxGames.Interactables;
using AnyoxGames.Util;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AnyoxGames.CameraSystem
{
    public abstract class CameraBehaviour : IStateMachineBehaviour<GameCamera>
    {
        [SerializeField] protected bool captureCursor;

        private bool initialized;

        //Use of the Odin Inspector package, commented out as it's not included with this project
/*#if UNITY_EDITOR
    [Button(name: "Enter State")]
    private void Editor_EnterState()
    {
        if (IServiceManager.Default.TryGetService<CameraSystem>(out var cameraSystem))
        {
            cameraSystem.SetBehaviour(this);
        }
    }
#endif*/

        protected virtual void Initialize(GameCamera target) { }

        public void EnterState(GameCamera target)
        {
            if (!initialized)
            {
                Initialize(target);
                initialized = true;
            }
            
            if (!captureCursor && CursorUtils.IsCursorCaptured)
            {
                CursorUtils.ReleaseCursor();
            }
            else if (captureCursor && !CursorUtils.IsCursorCaptured)
            {
                CursorUtils.CaptureCursor();
            }
            
            OnEnter(target);
        }
        protected virtual void OnEnter(GameCamera target) { }
        

        public void UpdateState(GameCamera target)
        {
            if (captureCursor)
            {
                if (!CursorUtils.IsCursorCaptured && Mouse.current != null && Mouse.current.leftButton.isPressed)
                {
                    CursorUtils.CaptureCursor();
                }

                if (CursorUtils.IsCursorCaptured && Keyboard.current != null && Keyboard.current.escapeKey.isPressed)
                {
                    CursorUtils.ReleaseCursor();
                }
            }

            OnUpdate(target);
            
            target.TryFocusOnNewInteractable(GetFocusedInteractable(target));
        }
        protected abstract void OnUpdate(GameCamera target);

        public virtual void ExitState(GameCamera target)
        {
            OnExit(target);
        }
        protected virtual void OnExit(GameCamera target) { }

        public virtual IInteractable GetFocusedInteractable(GameCamera target) => null;
    }
}