using UnityEngine;

namespace AnyoxGames.Audio
{
    public struct AudioInstance
    {
        public AudioSource Source;
        public float EndTime;

        public bool HasExpired => !Source.loop && Time.time > EndTime;

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