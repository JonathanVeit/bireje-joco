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
        public AudioSourceHandler Play(string soundId, Vector3 position, bool dontDestroyOnLoad = false)
        {
            var soundHandle = CreateSoundObject(soundId, position);
            soundHandle.AudioSource.Play();
            
            if (dontDestroyOnLoad)
                GameObject.DontDestroyOnLoad(soundHandle.gameObject);

            return soundHandle;
        }
        public AudioSourceHandler Play(string soundId, Transform parent, bool dontDestroyOnLoad = false)
        {
            var soundHandle = CreateSoundObject(soundId, parent);
            soundHandle.AudioSource.Play();

            if (dontDestroyOnLoad)
                GameObject.DontDestroyOnLoad(soundHandle.gameObject);

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