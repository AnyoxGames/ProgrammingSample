using UnityEngine;

namespace AnyoxGames.Character
{
    public abstract class ACharacterBehaviour : MonoBehaviour, ICharacterBehaviour
    {
        [ /*ReadOnly, Required, */SerializeField]
        public ACharacter character;

        public bool HasCharacterBehaviour<T>() where T : class, ICharacterBehaviour => character.HasCharacterBehaviour<T>();
        public bool TryGetCharacterBehaviour<T>(out T component) where T : class, ICharacterBehaviour => character.TryGetCharacterBehaviour(out component);

        public virtual void Initialize()
        {
        }

        public virtual void OnDeath(IDamageInvoker fromSource)
        {
        }

        public virtual void OnUpdate()
        {
        }

        protected bool GetCharacterAs<T>(out T castedCharacter) where T : ACharacter
        {
            if (character is T casted)
            {
                castedCharacter = casted;
                return true;
            }

            castedCharacter = null;
            return false;
        }

/*#if UNITY_EDITOR
    [HideIf("Editor_IsCharacterDefined"), Button("Revalidate Character")]
    private void Editor_RevalidateCharacter()
    {
        GetComponentInParent<ACharacter>().RevalidateBehaviours();
    }

    private bool Editor_IsCharacterDefined() => character;
#endif*/
    }
}