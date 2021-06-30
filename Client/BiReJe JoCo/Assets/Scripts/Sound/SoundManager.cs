using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BiReJeJoCo.Backend;
using JoVei.Base;

namespace BiReJeJoCo.Sound
{
    public class SoundManager : SystemAccessor, IInitializable
    {
        #region Initialization
        public IEnumerator Initialize(object[] parameters)
        {
            DIContainer.RegisterImplementation<SoundManager>(this);
            yield return null;
        }
        public void CleanUp() { DIContainer.UnregisterImplementation<SoundManager>(); }
        #endregion

        public void Play(string soundId, Vector3 position) 
        {
            Play(soundId, position, SoundOptions.Default);
        }

        public void Play(string soundId, Vector3 position, SoundOptions options)
        {
            var clipSetup = SoundMapping.GetMapping().GetElementForKey(soundId);
            var root = new GameObject($"(Sound) {soundId}");
            root.transform.position = position;

            var  audiosource = root.AddComponent<AudioSource>();
            SetupAudioSource(audiosource);

            audiosource.clip = clipSetup.clip;
            audiosource.volume = clipSetup.volume * options.VolumeMultiplier;

            audiosource.Play();
        }
        private void SetupAudioSource(AudioSource audiosource)
        {
            audiosource.playOnAwake = false;
            audiosource.loop = false;
        }
    }

    public struct SoundOptions
    {
        public float VolumeMultiplier { get; set; }

        public static SoundOptions Default 
        {
            get 
            {
                return new SoundOptions()
                {
                    VolumeMultiplier = 1,
                };
            }
        }
    }
}