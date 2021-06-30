using System.Collections;
using UnityEngine;
using JoVei.Base;

namespace BiReJeJoCo.Sound
{
    public class SoundEffectManager : SystemAccessor, IInitializable
    {
        #region Initialization
        public IEnumerator Initialize(object[] parameters)
        {
            DIContainer.RegisterImplementation<SoundEffectManager>(this);
            yield return null;
        }
        public void CleanUp() { DIContainer.UnregisterImplementation<SoundEffectManager>(); }
        #endregion

        public void Play(string soundId)
        {
            var audiosource = CreateSoundObject(soundId);
            audiosource.Play();
        }
        public void Play(string soundId, Vector3 position)
        {
            var audiosource = CreateSoundObject(soundId, position);
            audiosource.Play();
        }
        public void Play(string soundId, Transform parent)
        {
            var audiosource = CreateSoundObject(soundId, parent);
            audiosource.Play();
        }

        private AudioSource CreateSoundObject(string soundId)
        {
            var clipSetup = SoundEffectMapping.GetMapping().GetElementForKey(soundId);
            var root = poolingManager.PoolInstanceAs<AudioSourceHandler>(clipSetup, Vector3.zero, Quaternion.identity);
            return root.AudioSource;
        }
        private AudioSource CreateSoundObject(string soundId, Vector3 position) 
        {
            var clipSetup = SoundEffectMapping.GetMapping().GetElementForKey(soundId);
            var root = poolingManager.PoolInstanceAs<AudioSourceHandler>(clipSetup, position, Quaternion.identity);
            return root.AudioSource;
        }
        private AudioSource CreateSoundObject(string soundId, Transform parent)
        {
            var clipSetup = SoundEffectMapping.GetMapping().GetElementForKey(soundId);
            var root = poolingManager.PoolInstanceAs<AudioSourceHandler>(clipSetup, parent);
            return root.AudioSource;
        }
    }
}