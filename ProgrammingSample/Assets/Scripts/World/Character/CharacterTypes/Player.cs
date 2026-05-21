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
        }

        private void OnDestroy()
        {
            IServiceManager.Default.TryUnregisterService<Player>(this);
        }

        public override Transform GetTransformCameraTarget()
        {
            return CameraTarget;
        }
    }
}