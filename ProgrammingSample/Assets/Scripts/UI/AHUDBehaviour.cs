using UnityEngine;

namespace AnyoxGames.UI
{
    public abstract class AHUDBehaviour : MonoBehaviour
    {
        //Use of the Odin Inspector package, commented out as it's not included with this project
        [SerializeField /*, Required, ReadOnly*/]
        public HUD HUD;

        public bool HasHUDBehaviour<T>() where T : AHUDBehaviour => HUD.HasHUDBehaviour<T>();
        public bool TryGetHUDBehaviour<T>(out T component) where T : AHUDBehaviour => HUD.TryGetHUDBehaviour(out component);

        //Use of the Odin Inspector package, commented out as it's not included with this project
/*#if UNITY_EDITOR
    [HideIf("Editor_IsHUDDefined"), Button("Revalidate HUD")]
    private void Editor_RevalidateCharacter()
    {
        GetComponentInParent<HUD>().RevalidateBehaviours();
    }

    private bool Editor_IsHUDDefined() => HUD;
#endif*/
    }
}