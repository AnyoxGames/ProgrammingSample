using System;
using UnityEngine;

namespace AnyoxGames.CameraSystem
{
    [Serializable]
    public class StaticLookAtCameraBehaviour : CameraBehaviour
    {
        [SerializeField] private Vector3 positionOffset;
        [SerializeField] private Vector3 aimOffset;

        protected override void OnEnter(GameCamera target)
        {
            target.transform.position = target.CurrentTarget.GetTransformCameraTarget().position;
            target.transform.LookAt(target.CurrentTarget.GetTransformCameraTarget().position + aimOffset);
        }

        protected override void OnUpdate(GameCamera target)
        {
            if (target.CurrentTarget == null)
            {
                return;
            }

            target.transform.LookAt(target.CurrentTarget.GetTransformCameraTarget().position + aimOffset);
        }
    }
}