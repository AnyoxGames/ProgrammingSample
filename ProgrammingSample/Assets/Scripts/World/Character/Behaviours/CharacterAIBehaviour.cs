using System;
using System.Collections.Generic;
using AnyoxGames.Character.AI;
using UnityEngine;
using UnityEngine.AI;

namespace AnyoxGames.Character
{
    [CharacterBehaviour(-100), RequireComponent(typeof(NavMeshAgent))]
    public class CharacterAIBehaviour : ACharacterBehaviour
    {
        public AAIPackage CurrentPackage { get; private set; }
        public AAIState CurrentState { get; private set; }
        public NavMeshAgent Agent { get; private set; }

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            CurrentState?.Update(Time.deltaTime);
        }

        public void DecideNextAction() => CurrentPackage.DecideNextAction(this);

        public void SetBehaviour(AAIState state)
        {
            CurrentState?.End();
            CurrentState = state;
            CurrentState?.Start();
        }

        public void SetPackage(AAIPackage package)
        {
            CurrentPackage = package;
            DecideNextAction();
        }
    }
}