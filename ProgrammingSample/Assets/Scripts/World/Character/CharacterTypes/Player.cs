using AnyoxGames.CameraSystem;
using AnyoxGames.Service;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace AnyoxGames.Character 
{
    public class Player : ACharacter, IService
    {
        [SerializeField] private Transform CameraTarget;
        [SerializeField] private InputAction InteractAction;

        protected override void Awake()
        {
            IServiceManager.Default.TryRegisterService<Player>(this);

            if (IServiceManager.Default.TryGetService<PlayerStart>(out var playerStart))
            {
                transform.SetPositionAndRotation(playerStart.transform.position, playerStart.transform.rotation);
            }

            base.Awake();

            SceneManager.LoadScene("Scenes/HUD", LoadSceneMode.Additive);
        }

        private void Start()
        {
            if (CameraTarget && IServiceManager.Default.TryGetService<GameCamera>(out var cameraSystem))
            {
                cameraSystem.SetTarget(this);
                cameraSystem.SetBehaviour(cameraSystem.DefaultFirstPersonBehaviour);
            }

            InteractAction.performed += OnInteract;
            InteractAction.Enable();
        }

        private void OnDestroy()
        {
            InteractAction.Disable();
            InteractAction.performed -= OnInteract;

            IServiceManager.Default.TryUnregisterService<Player>(this);
        }

        private void OnInteract(InputAction.CallbackContext obj)
        {
            if (CurrentInteractable == null)
                return;

            CurrentInteractable.OnInteract(this);
        }

        public override Transform GetTransformCameraTarget()
        {
            return CameraTarget;
        }
    }
}