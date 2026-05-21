using System.Collections.Generic;
using AnyoxGames.Interactables;
using AnyoxGames.Util;
using UnityEngine;

namespace AnyoxGames.Character.AI
{
    public class SandboxPackage : AAIPackage
    {
        public Vector3 Origin;
        public float MaxRangeFromOrigin;
        public float LastRepopulateTime;

        private readonly List<InteractableObject> interactables = new();

        public SandboxPackage(Vector3 origin, float maxRangeFromOrigin)
        {
            Origin = origin;
            MaxRangeFromOrigin = maxRangeFromOrigin;

            RepopulateInteractables();
        }

        private void RepopulateInteractables()
        {
            interactables.Clear();

            foreach (var collider in Physics.OverlapSphere(Origin, MaxRangeFromOrigin))
                if (collider.TryGetComponent(out InteractableObject interactable) && interactable.CanAIInteract)
                {
                    interactables.Add(interactable);
                }

            LastRepopulateTime = Time.time;
            Debug.Log($"[AI] Repopulated interactables, {interactables.Count} found");
        }

        public override void DecideNextAction(CharacterAIBehaviour aiBehaviour)
        {
            float rng = Random.value;

            if (rng < 0.25f && Time.time - LastRepopulateTime > 7.5f)
            {
                RepopulateInteractables();
            }

            if (rng > 0)
            {
                ACharacter player = null;

                var nextInteractable = interactables.GetRandom();
                aiBehaviour.SetBehaviour(new TravelToGameObjectState(aiBehaviour, onComplete: s1 =>
                {
                    var travelState = s1 as TravelToGameObjectState;

                    aiBehaviour.SetBehaviour(new InteractWithObjectState(aiBehaviour, onComplete: _ => { DecideNextAction(aiBehaviour); })
                    {
                        InteractableObject = nextInteractable,
                        MaxInteracts = Random.Range(1, 30),
                        InteractInterval = 0.5f,
                        InteractRange = travelState?.RangeThreshold ?? 1.5f
                    });
                })
                {
                    TargetObject = nextInteractable.gameObject
                });
            }
        }
    }
}