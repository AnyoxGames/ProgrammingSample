using UnityEngine;

namespace AnyoxGames.CameraSystem
{
    public interface ICameraTarget
    {
        public Transform GetTransformCameraTarget();

        public bool TryGetCharacterCameraTarget(out ACharacter character)
        {
            character = null;
            return false;
        }
    }
}