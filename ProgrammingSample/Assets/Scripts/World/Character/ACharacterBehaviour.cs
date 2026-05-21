using System;
using UnityEngine;

public abstract class ACharacterBehaviour : MonoBehaviour, ICharacterBehaviour
{
    [/*ReadOnly, Required, */SerializeField] public ACharacter character;

    public bool HasCharacterBehaviour<T>() where T : class, ICharacterBehaviour => character.HasCharacterBehaviour<T>();
    public bool TryGetCharacterBehaviour<T>(out T component) where T : class, ICharacterBehaviour => character.TryGetCharacterBehaviour(out component);

    public virtual void Initialize() { }
    
    public virtual void OnDeath(IDamageInvoker fromSource) { }

    public virtual void OnUpdate() { }
    
/*#if UNITY_EDITOR
    [HideIf("Editor_IsCharacterDefined"), Button("Revalidate Character")]
    private void Editor_RevalidateCharacter()
    {
        GetComponentInParent<ACharacter>().RevalidateBehaviours();
    }     
    
    private bool Editor_IsCharacterDefined() => character;
#endif*/
}

public interface ICharacterBehaviour { }

public class CharacterBehaviourAttribute : Attribute
{
    public readonly int Priority;
    public CharacterBehaviourAttribute(int priority = 0)
    {
        Priority = priority;
    }
}