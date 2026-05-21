using System;
using AnyoxGames.Interactables;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AnyoxGames.Character.AI
{
    public class InteractWithObjectState : AAIState
    {
        public InteractableObject InteractableObject;
        public int MaxInteracts = 30;
        public float InteractInterval = 1f;
        public float InteractRange = 1.5f;

        private float NextInteractTime;
        private int EndAfterInteractions;

        public InteractWithObjectState(CharacterAIBehaviour characterAIBehaviour, Func<bool> completeCondition = null, Action<AAIState> onComplete = null) : base(characterAIBehaviour, completeCondition, onComplete)
        {
        }

        protected override void OnStart()
        {
            EndAfterInteractions = Random.Range(1, MaxInteracts);
        }

        protected override void OnUpdate()
        {
            if (!InteractableObject || Vector2.Distance(CharacterAIBehaviour.transform.position, InteractableObject.transform.position) > InteractRange)
            {
                Complete();
                return;
            }

            if (Time.time < NextInteractTime)
                return;

            InteractableObject.OnInteract(CharacterAIBehaviour.character);
            EndAfterInteractions--;

            Debug.Log($"[AI] Interacted with object, {EndAfterInteractions} interactions left");

            NextInteractTime = Time.time + InteractInterval;

            if (EndAfterInteractions < 1)
            {
                Complete();
            }
        }
    }
}