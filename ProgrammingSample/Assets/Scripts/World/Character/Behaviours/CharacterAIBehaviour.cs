using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CharacterBehaviour(-100), RequireComponent(typeof(NavMeshAgent))]
public class CharacterAIBehaviour : ACharacterBehaviour
{
    public AIPackage CurrentPackage { get; private set; }
    public AAIBehaviourState CurrentBehaviourState  { get; private set; }
    public NavMeshAgent Agent { get; private set; }

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        CurrentPackage = new SandboxPackage(transform.position, 20);
        DecideNextAction();
    }
    
    private void Update()
    {
        CurrentBehaviourState?.Update(Time.deltaTime);
    }

    public void DecideNextAction() => CurrentPackage.DecideNextAction(this);
    
    public void SetBehaviour(AAIBehaviourState behaviourState)
    {
        CurrentBehaviourState?.End();
        CurrentBehaviourState = behaviourState;
        CurrentBehaviourState?.Start();
    }
}