using AnyoxGames.Service;
using AnyoxGames.UI;
using AnyoxGames.Util;
using UnityEngine;

namespace AnyoxGames.CameraSystem
{
    public class GameCamera : MonoBehaviourService
    {
        [SerializeField] private FreeCameraBehaviour defaultFreeBehaviour;
        [SerializeField] private FirstPersonCameraBehaviour defaultFirstPersonBehaviour;
        [SerializeField] private Camera mainCamera;

        private StateMachine<GameCamera> stateMachine;

        public ICameraTarget OverrideTarget { get; private set; }
        public ICameraTarget Target { get; private set; }
        public ICameraTarget CurrentTarget => OverrideTarget ?? Target;
        public Camera MainCamera => mainCamera;
        public FreeCameraBehaviour DefaultFreeBehaviour => defaultFreeBehaviour;
        public FirstPersonCameraBehaviour DefaultFirstPersonBehaviour => defaultFirstPersonBehaviour;

        protected override void Awake()
        {
            base.Awake();
            stateMachine = new StateMachine<GameCamera>(this);
        }

        private void Update()
        {
            stateMachine.UpdateState(out var behaviour);

            if (behaviour is CameraBehaviour cameraBehaviour && IServiceManager.Default.TryGetService<HUD>(out var hudService) && hudService.TryGetHUDBehaviour<HUDInteractable>(out var interactable))
            {
                interactable.TrySetNewInteractable(cameraBehaviour.FindInteractable(this));
            }
        }

        public void SetBehaviour(CameraBehaviour newBehaviour)
        {
            stateMachine.SetState(newBehaviour);
        }

        public void SetOverrideBehaviour(CameraBehaviour overrideBehaviour)
        {
            stateMachine.SetOverrideState(overrideBehaviour);
        }

        public void SetOverrideTarget(ICameraTarget newTarget)
        {
            OverrideTarget = newTarget;
        }

        public void SetTarget(ICameraTarget newTarget)
        {
            Target = newTarget;
        }
    }
}