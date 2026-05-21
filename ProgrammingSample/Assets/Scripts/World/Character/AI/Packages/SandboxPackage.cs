using System.Collections.Generic;
using AnyoxGames.Util;
using UnityEngine;

public class SandboxPackage : AIPackage
{
    public Vector3 Origin;
    public float MaxRangeFromOrigin;
    public float LastRepopulateTime;

    private List<InteractableObject> Interactables = new();

    public SandboxPackage(Vector3 origin, float maxRangeFromOrigin)
    {
        Origin = origin;
        MaxRangeFromOrigin = maxRangeFromOrigin;

        RepopulateInteractables();
    }

    private void RepopulateInteractables()
    {
        Interactables.Clear();
        
        foreach (var collider in Physics.OverlapSphere(Origin, MaxRangeFromOrigin)) if (collider.TryGetComponent(out InteractableObject interactable) && interactable.CanAIInteract)
        {
            Debug.Log($"[AI] found interactable, {interactable.gameObject.name}");
            Interactables.Add(interactable);
        }
        
        LastRepopulateTime = Time.time;
        
        Debug.Log($"[AI] Repopulated interactables, {Interactables.Count} found");
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
            var nextInteractable = Interactables.GetRandom();
            aiBehaviour.SetBehaviour(new TravelToGameObjectState(aiBehaviour, onComplete: s1 =>
            {
                var travelState = s1 as TravelToGameObjectState;
                
                aiBehaviour.SetBehaviour(new InteractWithObjectState(aiBehaviour, onComplete: _ =>
                {
                    DecideNextAction(aiBehaviour);
                })
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