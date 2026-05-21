using System;

namespace AnyoxGames.Character.AI
{
    public class IdleState : AAIState
    {
        public IdleState(CharacterAIBehaviour characterAIBehaviour, Func<bool> completeCondition = null, Action<AAIState> onComplete = null) : base(characterAIBehaviour, completeCondition, onComplete)
        {
        }
    }
}