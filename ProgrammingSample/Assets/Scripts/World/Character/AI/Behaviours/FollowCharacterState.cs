using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AnyoxGames.Character.AI
{
    public class FollowCharacterState : AAIState
    {
        public ACharacter FollowCharacter;
        public float RangeThreshold = 2;
        public float RecalculateTime = 1;

        private float nextRecalcTime;

        public FollowCharacterState(CharacterAIBehaviour characterAIBehaviour, Func<bool> completeCondition = null, Action<AAIState> onComplete = null) : base(characterAIBehaviour, completeCondition, onComplete)
        {
        }

        protected override void OnStart()
        {
            UpdateDestination();
        }

        protected override void OnUpdate()
        {
            if (!FollowCharacter)
            {
                Complete();
                return;
            }

            if (Time.time < nextRecalcTime)
                return;

            UpdateDestination();
        }

        private void UpdateDestination()
        {
            if (!FollowCharacter)
                return;

            if (Vector2.Distance(CharacterAIBehaviour.transform.position, FollowCharacter.transform.position) > RangeThreshold)
            {
                var range = Random.insideUnitCircle * RangeThreshold;
                CharacterAIBehaviour.Agent.SetDestination(FollowCharacter.transform.position + new Vector3(range.x, 0, range.y));
            }

            nextRecalcTime = Time.time + RecalculateTime;
        }
    }
}