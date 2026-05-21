using System;
using System.Collections.Generic;
using AnyoxGames.Service;
using Unity.VisualScripting;
using UnityEngine;

namespace AnyoxGames.Audio
{
    public class Sound : MonoBehaviourService
    {
        [SerializeField] private int audioSourcePoolSize = 50;
        [SerializeField] private AudioSource template;

        private readonly Queue<AudioInstance> availableSources = new();
        private readonly List<AudioInstance> inUseSources = new();

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            foreach (var inUseSource in inUseSources)
            {
                UnityEditor.Handles.Label(inUseSource.Source.transform.position, inUseSource.Source.clip.name);
            }
        }
#endif

        protected override void Awake()
        {
            base.Awake();
            
            for (int i = 0; i < audioSourcePoolSize; i++)
            {
                CreateNewAudioSource();
            }
        }

        private void Update()
        {
            for (int i = inUseSources.Count - 1; i >= 0; i--)
            {
                if (!inUseSources[i].Source)
                {
                    Debug.LogWarning("Audio instance was destroyed");
                    inUseSources.RemoveAt(i);
                    CreateNewAudioSource();
                }
                
                if (inUseSources[i].HasExpired)
                {
                    inUseSources[i].Source.Stop();
                    inUseSources[i].Source.transform.SetParent(transform, false);
                    availableSources.Enqueue(inUseSources[i]);
                    inUseSources.RemoveAt(i);
                }
            }
        }

        private void CreateNewAudioSource()
        {
            var newInstance = Instantiate(template, transform, false);
            newInstance.gameObject.name = "AudioInstance";
            availableSources.Enqueue(new AudioInstance { Source = newInstance });
        }

        /// <summary>
        /// Play a audio clip outside 3D space, e.g UI sounds
        /// </summary>
        public static AudioInstance PlayDirect(AudioClip clip, float volume = 1.0f, bool loop = false)
        {
            if (!IServiceManager.Default.TryGetService<Sound>(out var soundService))
                throw new InvalidImplementationException("No sound service found");
                
            return soundService.PlaySoundDirect_Internal(clip, volume, loop);
        }

        private AudioInstance PlaySoundDirect_Internal(AudioClip clip, float volume, bool loop)
        {
            var audioInstance = availableSources.Dequeue();
            audioInstance.Source.transform.SetParent(transform);
            audioInstance.Source.spatialBlend = 0;
            audioInstance.Source.volume = volume;
            audioInstance.Source.loop = loop;
            audioInstance.Source.PlayOneShot(clip);
            audioInstance.EndTime = Time.time + clip.length;
            inUseSources.Add(audioInstance);

            return audioInstance;
        }

        /// <summary>
        /// Play a audio clip at a 3D position, e.g light switches, doors opening
        /// </summary>
        public static AudioInstance PlayAtPosition(AudioClip clip, Vector3 position, float volume = 1, bool loop = false)
        {
            if (!IServiceManager.Default.TryGetService<Sound>(out var soundService))
                throw new InvalidImplementationException("No sound service found");

            return soundService.PlaySoundAtPosition_Internal(clip, position, volume, loop);
        }

        private AudioInstance PlaySoundAtPosition_Internal(AudioClip clip, Vector3 position, float volume, bool loop)
        {
            var audioInstance = availableSources.Dequeue();
            audioInstance.Source.transform.SetParent(null);
            audioInstance.Source.transform.position = position;
            audioInstance.Source.spatialBlend = 1;
            audioInstance.Source.volume = volume;
            audioInstance.Source.loop = loop;
            audioInstance.Source.clip = clip;
            audioInstance.Source.Play();
            audioInstance.EndTime = Time.time + clip.length;
            inUseSources.Add(audioInstance);

            return audioInstance;
        }

        /// <summary>
        /// Play a audio clip on a moving 3D object, e.g rigidbody impacts, footsteps, projectiles
        /// </summary>
        public static AudioInstance PlaySoundMoving(AudioClip clip, Transform targetTransform, float volume = 1)
        {
            if (IServiceManager.Default.TryGetService<Sound>(out var soundService))
                throw new InvalidImplementationException("No sound service found");
            
            return soundService.PlayMoving_Internal(clip, targetTransform, volume);
        }

        private AudioInstance PlayMoving_Internal(AudioClip clip, Transform targetTransform, float volume)
        {
            var audioInstance = availableSources.Dequeue();
            audioInstance.Source.transform.SetParent(targetTransform, false);
            audioInstance.Source.transform.localPosition = Vector3.zero;
            audioInstance.Source.spatialBlend = 1;
            audioInstance.Source.volume = volume;
            audioInstance.Source.clip = clip;
            audioInstance.Source.Play();
            audioInstance.EndTime = Time.time + clip.length;
            inUseSources.Add(audioInstance);

            return audioInstance;
        }
    }
}