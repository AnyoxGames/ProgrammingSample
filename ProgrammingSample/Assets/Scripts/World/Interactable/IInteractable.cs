using System;
using UnityEngine;

public interface IInteractable
{
    public string Name { get; }
    public string Action { get; }
    public bool CanAIInteract { get; }

    public void OnInteract(ACharacter character);
}