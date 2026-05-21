using System;

namespace AnyoxGames.Character
{
    public class CharacterBehaviourAttribute : Attribute
    {
        public readonly int Priority;

        public CharacterBehaviourAttribute(int priority = 0)
        {
            Priority = priority;
        }
    }
}