using System;
using AnyoxGames.Character;
using UnityEngine;

namespace AnyoxGames.Interactables
{
    public interface IInteractable
    {
        public string Name { get; }
        public string Action { get; }
        public bool CanAIInteract { get; }

        public void OnInteract(ACharacter character);
    }
}