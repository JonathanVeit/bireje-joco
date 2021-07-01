using JoVei.Base;
using JoVei.Base.Helper;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BiReJeJoCo.Audio
{
    public class MusicManager : SystemAccessor, IInitializable
    {
        #region Initialization
        public IEnumerator Initialize(object[] parameters)
        {
            DIContainer.RegisterImplementation<MusicManager>(this);
            CreateAudioSource();
            yield return null;
        }
        public void CleanUp()
        {
            DIContainer.UnregisterImplementation<MusicManager>();
        }

        private void CreateAudioSource()
        {
            var prefab = Resources.Load("music_source");
            var root = (GameObject)Object.Instantiate(prefab);
            GameObject.DontDestroyOnLoad(root);

            audioSource = root.GetComponent<AudioSource>();
            musicObserver = new GameMusicObserver();
        }

        #endregion

        private AudioSource audioSource;
        private Coroutine musicSwitcher;
        private GameMusicObserver musicObserver;

        public void Play(string clipId, float transitionSpeed = 1)
        {
            var clip = MusicMapping.GetMapping().GetElementForKey(clipId);

            if (musicSwitcher != null)
                CoroutineHelper.Instance.StopCoroutine(musicSwitcher);
            CoroutineHelper.Instance.StartCoroutine(SwitchMusicClip(clip, transitionSpeed));
        }

        private IEnumerator SwitchMusicClip(AudioClip clip, float speed)
        {
            while (audioSource.volume > 0)
            {
                audioSource.volume -= Time.deltaTime * speed;
                yield return null;
            }
            
            audioSource.volume = 0;
            audioSource.clip = clip;

            if (!audioSource.isPlaying)
                audioSource.Play();

            while (audioSource.volume < 1)
            {
                audioSource.volume += Time.deltaTime * speed;
                yield return null;
            }
            audioSource.volume = 1;
        }
    }

    public class GameMusicObserver : SystemAccessor, ITickable
    {
        public GameMusicObserver() 
        {
            tickSystem.Register(this);
            musicManager.Play("boot_up");
        }

        public void Tick(float deltaTime)
        {
            if (Keyboard.current[Key.H].wasPressedThisFrame) 
            {
                musicManager.Play("search_hide");
            }
        }
    }
}