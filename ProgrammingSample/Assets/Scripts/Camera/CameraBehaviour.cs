using AnyoxGames.Util;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AnyoxGames.CameraSystem
{
    public abstract class CameraBehaviour : IStateMachineBehaviour<GameCamera>
    {
        [SerializeField] protected bool captureCursor;

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

        public virtual void EnterState(GameCamera target)
        {
            if (!captureCursor && CursorUtils.IsCursorCaptured)
            {
                CursorUtils.ReleaseCursor();
            }
            else if (captureCursor && !CursorUtils.IsCursorCaptured)
            {
                CursorUtils.CaptureCursor();
            }
        }

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
        }

        public virtual IInteractable FindInteractable(GameCamera target) => null;

        protected abstract void OnUpdate(GameCamera target);

        public virtual void ExitState(GameCamera target)
        {
        }
    }
}