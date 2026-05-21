using System;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class AAIBehaviourState
{
    protected readonly CharacterAIBehaviour CharacterAIBehaviour;

    public bool AllowRunning = false;
    
    protected float TimeElapsed;
    
    protected event Action<AAIBehaviourState> OnComplete;
    protected event Func<bool> IsComplete;

    protected AAIBehaviourState(CharacterAIBehaviour characterAIBehaviour, Func<bool> completeCondition = null, Action<AAIBehaviourState> onComplete = null)
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

    protected virtual void OnStart() { }
    
    public void Update(float dt)
    {
        if (IsComplete != null && IsComplete())
        {
            Complete();
            return;
        }
        
        TimeElapsed += dt;
        OnUpdate();
    }
    protected virtual void OnUpdate() { }
    
    public void End() => OnEnd();
    protected virtual void OnEnd() { }
    
    public void Complete()
    {
        Debug.Log($"[AI] Behaviour {GetType().Name} complete after {TimeElapsed}s");
        OnComplete?.Invoke(this);
    }
}

public class IdleState : AAIBehaviourState
{
    public IdleState(CharacterAIBehaviour characterAIBehaviour, Func<bool> completeCondition = null, Action<AAIBehaviourState> onComplete = null) : base(characterAIBehaviour, completeCondition, onComplete) { }
}

public class FollowCharacterState : AAIBehaviourState
{
    public ACharacter TargetCharacter;
    public float RangeThreshold = 2;
    public float RecalculateTime = 1; 
    
    private float NextRecalcTime;
    
    public FollowCharacterState(CharacterAIBehaviour characterAIBehaviour, Func<bool> completeCondition = null, Action<AAIBehaviourState> onComplete = null) : base(characterAIBehaviour, completeCondition, onComplete) { }

    protected override void OnStart()
    {
        UpdateDestination();
    }
    
    protected override void OnUpdate()
    {
        if (!TargetCharacter)
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
        if (!TargetCharacter)
            return;

        if (Vector2.Distance(CharacterAIBehaviour.transform.position, TargetCharacter.transform.position) > RangeThreshold)
        {
            var range = Random.insideUnitCircle * RangeThreshold;
            CharacterAIBehaviour.Agent.SetDestination(TargetCharacter.transform.position + new Vector3(range.x, 0, range.y));
        }

        NextRecalcTime = Time.time + RecalculateTime;
    }
}

public class TravelToGameObjectState : AAIBehaviourState
{
    public GameObject TargetObject;
    public float RecalculateTime = 4;
    public float RangeThreshold = 2;
    public float DestinationThreshold = 1f;
    
    private float NextRecalcTime;
    
    public TravelToGameObjectState(CharacterAIBehaviour characterAIBehaviour, Func<bool> completeCondition = null, Action<AAIBehaviourState> onComplete = null) : base(characterAIBehaviour, completeCondition, onComplete) { }
    
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

public class InteractWithObjectState : AAIBehaviourState
{
    public InteractableObject InteractableObject;
    public int MaxInteracts = 30;
    public float InteractInterval = 1f;
    public float InteractRange = 1.5f;
    
    private float NextInteractTime;
    private int EndAfterInteractions;
    
    public InteractWithObjectState(CharacterAIBehaviour characterAIBehaviour, Func<bool> completeCondition = null, Action<AAIBehaviourState> onComplete = null) : base(characterAIBehaviour, completeCondition, onComplete) { }

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