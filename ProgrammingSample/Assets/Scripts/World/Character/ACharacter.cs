using System;
using System.Collections.Generic;
using System.Linq;
using AnyoxGames.CameraSystem;
using Unity.VisualScripting;
using UnityEngine;

public abstract class ACharacter : ADamageable, ICameraTarget
{
    //[ReadOnly, InfoBox("Behaviours are executed from top to bottom, use CharacterBehaviourAttribute to define priority")]
    [SerializeField] private List<ACharacterBehaviour> allBehaviours;

    protected IInteractable CurrentInteractable;
    
    private Dictionary<Type, ACharacterBehaviour> registeredBehaviours = new();

    protected override void Awake()
    {
        base.Awake();

        foreach (var behaviour in allBehaviours)
        {
            registeredBehaviours.Add(behaviour.GetType(), behaviour);
        }
        
        foreach (var behaviour in allBehaviours)
        {
            behaviour.Initialize();
        }
    }

    protected override void OnDeath(IDamageInvoker fromSource)
    {
        foreach (var behaviour in allBehaviours)
        {
            behaviour.OnDeath(fromSource);
        }
    }

    public bool HasCharacterBehaviour<T>() where T : class, ICharacterBehaviour
    {
        return registeredBehaviours.ContainsKey(typeof(T));
    }

    public bool TryGetCharacterBehaviour<T>(out T behaviour) where T : class, ICharacterBehaviour
    {
        behaviour = null;

        if (!registeredBehaviours.TryGetValue(typeof(T), out var target) || target is not T targetBehaviour)
        {
            Debug.Log(HasCharacterBehaviour<T>() + " + " + (target is not T));
            return false;
        }

        behaviour = targetBehaviour;
        return true;

    }

    protected virtual void Update()
    {
        foreach (var behaviour in allBehaviours)
        {
            behaviour.OnUpdate();
        }
    }

    public void SetCurrentInteractable(IInteractable interactable)
    {
        CurrentInteractable = interactable;
    }

    //[Button("Revalidate Behaviours")]
    public void RevalidateBehaviours()
    {
        allBehaviours = GetComponentsInChildren<ACharacterBehaviour>().OrderBy(behaviour =>
        {
            behaviour.character = this;
            var attribute = behaviour.GetType().GetAttribute<CharacterBehaviourAttribute>();
            return attribute?.Priority ?? 0;
        }).ToList();
    }

    public virtual Transform GetTransformCameraTarget()
    {
        return transform;
    }

    public bool TryGetCharacterCameraTarget(out ACharacter character)
    {
        character = this;
        return true;
    }
}
