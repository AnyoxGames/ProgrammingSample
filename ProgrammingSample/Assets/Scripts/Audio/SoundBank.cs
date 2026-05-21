using AnyoxGames.Util;
using UnityEngine;

namespace AnyoxGames.Audio
{
    [CreateAssetMenu]
    public class SoundBank : ScriptableObject
    {
        [SerializeField] private AudioClip[] sounds;

        public AudioClip[] AllSounds => sounds;
        public AudioClip Next() => sounds.GetRandom();
    }
}