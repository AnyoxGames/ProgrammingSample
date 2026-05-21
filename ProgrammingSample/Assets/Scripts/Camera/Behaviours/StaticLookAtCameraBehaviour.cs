using System;
using UnityEngine;

namespace AnyoxGames.CameraSystem
{
    [Serializable]
    public class StaticLookAtCameraBehaviour : CameraBehaviour
    {
        [SerializeField] private Vector3 positionOffset;
        [SerializeField] private Vector3 aimOffset;

        public override void EnterState(GameCamera target)
        {
            base.EnterState(target);

            target.transform.position = target.CurrentTarget.GetTransformCameraTarget().position;
            target.transform.forward = target.CurrentTarget.GetTransformCameraTarget().forward;
        }

        protected override void OnUpdate(GameCamera target)
        {
            if (target.CurrentTarget == null)
            {
                return;
            }

            target.transform.position = target.CurrentTarget.GetTransformCameraTarget().position + positionOffset;
            target.transform.LookAt(target.CurrentTarget.GetTransformCameraTarget().position + aimOffset);
        }
    }
}