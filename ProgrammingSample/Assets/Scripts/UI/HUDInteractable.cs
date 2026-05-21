using AnyoxGames.CameraSystem;
using AnyoxGames.Service;
using TMPro;
using UnityEngine;

namespace AnyoxGames.UI
{
    public class HUDInteractable : MonoBehaviour
    {
        private static readonly Color WhiteClear = new(1f, 1f, 1f, 0);

        [SerializeField] private TextMeshProUGUI interactableName;
        [SerializeField] private TextMeshProUGUI actionName;

        private void Awake()
        {
            interactableName.color = WhiteClear;
            actionName.color = WhiteClear;

            if (IServiceManager.Default.TryGetService<GameCamera>(out var cameraService))
            {
                cameraService.OnFocusedInteractableChanged += UpdateInteractable;
            }
        }

        private void UpdateInteractable(IInteractable newInteractable)
        {
            if (newInteractable != null)
            {
                interactableName.text = newInteractable.Name;
                actionName.text = $"[E] {newInteractable.Action}";
                
                interactableName.color = Color.white;
                actionName.color = Color.white;
            }
            else
            {
                interactableName.text = null;
                actionName.text = null;
                
                interactableName.color = WhiteClear;
                actionName.color = WhiteClear;
            }
        }
    }
}