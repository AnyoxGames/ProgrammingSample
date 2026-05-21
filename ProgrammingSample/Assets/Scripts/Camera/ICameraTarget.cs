using AnyoxGames.Character;
using UnityEngine;

namespace AnyoxGames.CameraSystem
{
    public interface ICameraTarget
    {
        public Transform GetTransformCameraTarget();

        public bool TryGetTargetedCharacter(out ACharacter character)
        {
            character = null;
            return false;
        }
    }
}