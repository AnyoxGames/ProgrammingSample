using System;
using UnityEngine;

namespace AnyoxGames.Character.AI
{
    public abstract class AAIState
    {
        protected readonly CharacterAIBehaviour CharacterAIBehaviour;

        protected float timeElapsed;

        protected event Action<AAIState> OnComplete;
        protected event Func<bool> IsComplete;

        protected AAIState(CharacterAIBehaviour characterAIBehaviour, Func<bool> completeCondition = null, Action<AAIState> onComplete = null)
        {
            CharacterAIBehaviour = characterAIBehaviour;
            OnComplete = onComplete ?? (_ => characterAIBehaviour.DecideNextAction());
            IsComplete = completeCondition;
        }

        public void Start()
        {
            Debug.Log($"[AI] Starting behaviour {GetType().Name} complete");
            OnStart();
        }

        protected virtual void OnStart()
        {
        }

        public void Update(float dt)
        {
            if (IsComplete != null && IsComplete())
            {
                Complete();
                return;
            }

            timeElapsed += dt;
            OnUpdate();
        }

        protected virtual void OnUpdate()
        {
        }

        public void End() => OnEnd();

        protected virtual void OnEnd()
        {
        }

        public void Complete()
        {
            Debug.Log($"[AI] Behaviour {GetType().Name} complete after {timeElapsed}s");
            OnComplete?.Invoke(this);
        }
    }
}