using System;
using AnyoxGames.Interactables;
using AnyoxGames.Service;
using AnyoxGames.UI;
using AnyoxGames.Util;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AnyoxGames.CameraSystem
{
    public class GameCamera : MonoBehaviourService
    {
        [SerializeField] private FreeCameraBehaviour defaultFreeBehaviour;
        [SerializeField] private FirstPersonCameraBehaviour defaultFirstPersonBehaviour;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private InputAction interactAction;

        private StateMachine<GameCamera> stateMachine;
        private IInteractable focusedInteractable;

        public ICameraTarget OverrideTarget { get; private set; }
        public ICameraTarget Target { get; private set; }
        public ICameraTarget CurrentTarget => OverrideTarget ?? Target;
        public Camera MainCamera => mainCamera;
        public IInteractable FocusedInteractable => focusedInteractable;
        public FreeCameraBehaviour DefaultFreeBehaviour => defaultFreeBehaviour;
        public FirstPersonCameraBehaviour DefaultFirstPersonBehaviour => defaultFirstPersonBehaviour;

        public event Action<IInteractable> OnFocusedInteractableChanged;

        protected override void Awake()
        {
            base.Awake();
            stateMachine = new StateMachine<GameCamera>(this);
        }

        private void Update()
        {
            stateMachine.UpdateState();
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

        public void TryFocusOnNewInteractable(IInteractable interactable)
        {
            if (focusedInteractable == interactable)
                return;
            
            focusedInteractable = interactable;
            OnFocusedInteractableChanged?.Invoke(focusedInteractable);
        }
    }
}