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
                    Debug.LogWarning("SFX instance was destroyed");
                    inUseSources.RemoveAt(i);
                    CreateNewAudioSource();
                }

                if (inUseSources[i].HasExpired)
                {
                    inUseSources[i].Source.Stop();
                    inUseSources[i].Source.transform.SetParent(transform);
                    availableSources.Enqueue(inUseSources[i]);
                    inUseSources.RemoveAt(i);
                }
            }
        }

        private void CreateNewAudioSource()
        {
            var newInstance = Instantiate(template, transform, false);
            newInstance.gameObject.name = "SFXInstance";
            availableSources.Enqueue(new AudioInstance() { Source = newInstance });
        }

        public static AudioInstance PlayDirect(AudioClip clip, float volume = 1.0f, bool loop = false)
        {
            if (IServiceManager.Default.TryGetService<Sound>(out var soundService))
            {
                return soundService.PlaySoundDirect_Internal(clip, volume, loop);
            }
            
            throw new InvalidImplementationException("No sound service found");
        }

        private AudioInstance PlaySoundDirect_Internal(AudioClip clip, float volume, bool loop)
        {
            var sfxInstance = availableSources.Dequeue();
            sfxInstance.Source.transform.SetParent(transform);
            sfxInstance.Source.spatialBlend = 0;
            sfxInstance.Source.volume = volume;
            sfxInstance.Source.loop = loop;
            sfxInstance.Source.PlayOneShot(clip);
            sfxInstance.EndTime = Time.time + clip.length;
            inUseSources.Add(sfxInstance);

            return sfxInstance;
        }

        public static AudioInstance PlayAtPosition(AudioClip clip, Vector3 position, float volume = 1, bool loop = false)
        {
            if (IServiceManager.Default.TryGetService<Sound>(out var soundService))
            {
                return soundService.PlaySoundAtPosition_Internal(clip, position, volume, loop);
            }
            
            throw new InvalidImplementationException("No sound service found");
        }

        private AudioInstance PlaySoundAtPosition_Internal(AudioClip clip, Vector3 position, float volume, bool loop)
        {
            var sfxInstance = availableSources.Dequeue();
            sfxInstance.Source.transform.SetParent(null);
            sfxInstance.Source.transform.position = position;
            sfxInstance.Source.spatialBlend = 1;
            sfxInstance.Source.volume = volume;
            sfxInstance.Source.loop = loop;
            sfxInstance.Source.clip = clip;
            sfxInstance.Source.Play();
            sfxInstance.EndTime = Time.time + clip.length;
            inUseSources.Add(sfxInstance);

            return sfxInstance;
        }

        public static AudioInstance PlaySoundMoving(AudioClip clip, Transform targetTransform, float volume = 1)
        {
            if (IServiceManager.Default.TryGetService<Sound>(out var soundService))
            {
                return soundService.PlayMoving_Internal(clip, targetTransform, volume);
            }
            
            throw new InvalidImplementationException("No sound service found");
        }

        private AudioInstance PlayMoving_Internal(AudioClip clip, Transform targetTransform, float volume)
        {
            var sfxInstance = availableSources.Dequeue();
            sfxInstance.Source.transform.SetParent(targetTransform, false);
            sfxInstance.Source.transform.localPosition = Vector3.zero;
            sfxInstance.Source.spatialBlend = 1;
            sfxInstance.Source.volume = volume;
            sfxInstance.Source.clip = clip;
            sfxInstance.Source.Play();
            sfxInstance.EndTime = Time.time + clip.length;
            inUseSources.Add(sfxInstance);

            return sfxInstance;
        }
    }
}