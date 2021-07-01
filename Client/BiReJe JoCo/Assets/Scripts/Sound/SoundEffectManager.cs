using System.Collections;
using UnityEngine;
using JoVei.Base;

namespace BiReJeJoCo.Audio
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

        public AudioSourceHandler Play(string soundId)
        {
            var audiosource = CreateSoundObject(soundId);
            audiosource.AudioSource.Play();

            return audiosource;
        }
        public AudioSourceHandler Play(string soundId, Vector3 position)
        {
            var audiosource = CreateSoundObject(soundId, position);
            audiosource.AudioSource.Play();

            return audiosource;
        }
        public AudioSourceHandler Play(string soundId, Transform parent)
        {
            var soundHandle = CreateSoundObject(soundId, parent);
            soundHandle.AudioSource.Play();

            return soundHandle;
        }

        private AudioSourceHandler CreateSoundObject(string soundId)
        {
            var clipSetup = SoundEffectMapping.GetMapping().GetElementForKey(soundId);
            var root = poolingManager.PoolInstanceAs<AudioSourceHandler>(clipSetup, Vector3.zero, Quaternion.identity);
            return root;
        }
        private AudioSourceHandler CreateSoundObject(string soundId, Vector3 position) 
        {
            var clipSetup = SoundEffectMapping.GetMapping().GetElementForKey(soundId);
            var root = poolingManager.PoolInstanceAs<AudioSourceHandler>(clipSetup, position, Quaternion.identity);
            return root;
        }
        private AudioSourceHandler CreateSoundObject(string soundId, Transform parent)
        {
            var clipSetup = SoundEffectMapping.GetMapping().GetElementForKey(soundId);
            var root = poolingManager.PoolInstanceAs<AudioSourceHandler>(clipSetup, parent);
            return root;
        }
    }
}