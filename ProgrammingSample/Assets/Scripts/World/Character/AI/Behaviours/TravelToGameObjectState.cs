using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AnyoxGames.Character.AI
{
    public class TravelToGameObjectState : AAIState
    {
        public GameObject TargetObject;
        public float RecalculateTime = 4;
        public float RangeThreshold = 2;
        public float DestinationThreshold = 1f;

        private float NextRecalcTime;

        public TravelToGameObjectState(CharacterAIBehaviour characterAIBehaviour, Func<bool> completeCondition = null, Action<AAIState> onComplete = null) : base(characterAIBehaviour, completeCondition, onComplete)
        {
        }

        protected override void OnStart()
        {
            UpdateDestination();
        }

        protected override void OnEnd()
        {
            CharacterAIBehaviour.Agent.ResetPath();
        }

        protected override void OnUpdate()
        {
            if (!TargetObject || Vector2.Distance(CharacterAIBehaviour.transform.position, CharacterAIBehaviour.Agent.destination) < DestinationThreshold)
            {
                Complete();
                return;
            }

            if (Time.time < NextRecalcTime)
                return;

            UpdateDestination();
        }

        private void UpdateDestination()
        {
            if (!TargetObject)
                return;

            if (Vector2.Distance(CharacterAIBehaviour.transform.position, TargetObject.transform.position) > RangeThreshold)
            {
                var range = Random.insideUnitCircle * RangeThreshold;
                CharacterAIBehaviour.Agent.SetDestination(TargetObject.transform.position + new Vector3(range.x, 0, range.y));

                Debug.Log($"[AI] Travelling to {TargetObject.gameObject.name} at {TargetObject.transform.position}");
            }

            NextRecalcTime = Time.time + RecalculateTime;
        }
    }
}