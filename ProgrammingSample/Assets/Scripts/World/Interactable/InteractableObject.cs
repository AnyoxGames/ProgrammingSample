using AnyoxGames.Character;
using UnityEngine;
using UnityEngine.Events;

namespace AnyoxGames.Interactables
{
    public class InteractableObject : MonoBehaviour, IInteractable
    {
        [SerializeField] private bool _CanAIInteract = true;
        [SerializeField] private string NameText;
        [SerializeField] private string ActionText;
        [SerializeField] private UnityEvent<ACharacter, IInteractable> InteractCallback;

        public string Name => NameText;
        public string Action => ActionText;
        public bool CanAIInteract => _CanAIInteract;

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(NameText))
            {
                NameText = gameObject.name;
            }
        }

        public void OnInteract(ACharacter character)
        {
            if (character is NPC && !CanAIInteract)
                return;

            InteractCallback?.Invoke(character, this);
        }
    }
}