using TMPro;
using UnityEngine;

namespace AnyoxGames.UI
{
    public class HUDInteractable : AHUDBehaviour
    {
        private static readonly Color WhiteClear = new(255, 255, 255, 0);

        [SerializeField] private TextMeshProUGUI interactableName;
        [SerializeField] private TextMeshProUGUI actionName;

        private IInteractable currentInteractable;

        private void Awake()
        {
            interactableName.color = WhiteClear;
            actionName.color = WhiteClear;
        }

        public void TrySetNewInteractable(IInteractable newInteractable)
        {
            if (newInteractable == currentInteractable)
                return;

            currentInteractable = newInteractable;

            if (currentInteractable != null)
            {
                interactableName.text = currentInteractable.Name;
                actionName.text = $"[E] {currentInteractable.Action}";
            }
            else
            {
                interactableName.text = null;
                actionName.text = null;
            }
        }

        private void Update()
        {
            if (currentInteractable != null)
            {
                interactableName.color = Color.Lerp(interactableName.color, Color.white, 50 * Time.deltaTime);
                actionName.color = Color.Lerp(actionName.color, Color.white, 50 * Time.deltaTime);
            }
            else
            {
                interactableName.color = Color.Lerp(interactableName.color, WhiteClear, 50 * Time.deltaTime);
                actionName.color = Color.Lerp(actionName.color, WhiteClear, 50 * Time.deltaTime);
            }


        }
    }
}