using JoVei.Base;
using JoVei.Base.Helper;
using System.Collections;
using UnityEngine;

namespace BiReJeJoCo.Audio
{
    public enum MusicState
    {
        BooUp = 0,
        MainMenu = 1,
        Lobby = 2,
        MatchDefault = 3,
        HunterChasing = 4,
        HuntedChasing = 5,
    }

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
            var root = (GameObject) Object.Instantiate(prefab);
            Object.DontDestroyOnLoad(root);

            audioSource = root.GetComponent<AudioSource>();
            musicObserver = new GameMusicObserver();
        }

        #endregion

        private AudioSource audioSource;
        private Coroutine musicSwitcher;
        private GameMusicObserver musicObserver;

        public MusicState CurrentState { get; private set; }
        public bool IsTransitioning { get; private set; }
        public GameMusicConfig MusicConfig => LoadMusicConfig();

        private float curFadeOut = 1;

        public void Play(MusicState state)
        {
            CurrentState = state;
            if (MusicConfig.GetClip(state, out var clipConfig))
            {
                if (musicSwitcher != null)
                    CoroutineHelper.Instance.StopCoroutine(musicSwitcher);

                musicSwitcher = CoroutineHelper.Instance.StartCoroutine(SwitchMusicClip(clipConfig.clip, clipConfig.fadeIn, curFadeOut));
                curFadeOut = clipConfig.fadeOut;
            }
        }

        private IEnumerator SwitchMusicClip(AudioClip clip, float fadeInSpeed, float fadeOutSpeed)
        {
            var waiter = new WaitForEndOfFrame();
            IsTransitioning = true;

            while (audioSource.volume > 0)
            {
                audioSource.volume -= Time.deltaTime * fadeOutSpeed;
                yield return waiter;
            }
            
            audioSource.volume = 0;
            audioSource.clip = clip;

            if (!audioSource.isPlaying)
                audioSource.Play();

            while (audioSource.volume < 1)
            {
                audioSource.volume += Time.deltaTime * fadeInSpeed;
                yield return waiter;
            }

            audioSource.volume = 1;
            IsTransitioning = false;
        }

        #region Helper
        private GameMusicConfig LoadMusicConfig()
        {
            var configs = Resources.LoadAll<GameMusicConfig>("");
            if (configs.Length == 0)
            {
                Debug.LogError("Unable to find GameMusicConfig asset!");
                return null;
            }

            return configs[0];
        }
        #endregion
    }
}