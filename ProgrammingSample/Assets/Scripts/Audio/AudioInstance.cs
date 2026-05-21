using UnityEngine;

namespace AnyoxGames.Audio
{
    public struct AudioInstance
    {
        public AudioSource Source;
        public float EndTime;

        public bool HasExpired => !Source.loop && Time.time > EndTime;

        /// <summary>
        /// Instantly expire the audio instance
        /// </summary>
        public void Expire()
        {
            if (Source)
            {
                Source.loop = false;
            }

            EndTime = Time.time;
        }
    }
}