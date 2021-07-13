using JoVei.Base;
using JoVei.Base.Helper;
using System;
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
        HunterWin = 6,
        HuntedWins = 7,
        ResultScreen = 9,
    }

    public class MusicManager : SystemAccessor, IInitializable
    {
        #region Initialization
        public IEnumerator Initialize(object[] parameters)
        {
            DIContainer.RegisterImplementation<MusicManager>(this);
            LoadMusicConfig();
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
            var root = (GameObject) UnityEngine.Object.Instantiate(prefab);
            UnityEngine.Object.DontDestroyOnLoad(root);

            audioSource = root.GetComponent<AudioSource>();
            musicObserver = new GameMusicObserver();
        }

        #endregion

        private AudioSource audioSource;
        private Coroutine musicSwitcher;
        private Coroutine musicFinishedAwaiter;

        private GameMusicObserver musicObserver;

        public MusicState CurrentState { get; private set; }
        public bool IsTransitioning { get; private set; }
        public GameMusicConfig MusicConfig { get; private set; }

        private float curFadeOut = 1;

        public void Play(MusicState state)
        {
            Play(state, null);
        }
        public void Play(MusicState state, Action onFinishedPlay)
        {
            CurrentState = state;

            if (MusicConfig.GetClip(state, out var clipConfig))
            {
                float fadeout = SetupPlay(clipConfig);
                curFadeOut = clipConfig.fadeOut;
                musicSwitcher = CoroutineHelper.Instance.StartCoroutine(SwitchMusicClip(clipConfig.clip, clipConfig.fadeIn, fadeout, onFinishedPlay));
            }
        }

        private float SetupPlay(GameMusicConfig.MusicClipConfig clipConfig)
        {
            if (musicSwitcher != null)
                CoroutineHelper.Instance.StopCoroutine(musicSwitcher);
            if (musicFinishedAwaiter != null)
                CoroutineHelper.Instance.StopCoroutine(musicFinishedAwaiter);

            var fadeout = curFadeOut;
            if (clipConfig.overridePrevious)
                fadeout = clipConfig.overrideFadeOut;
            return fadeout;
        }

        private IEnumerator SwitchMusicClip(AudioClip clip, float fadeInSpeed, float fadeOutSpeed, Action onFinishedPlay)
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
            audioSource.loop = onFinishedPlay == null;
            if (!audioSource.isPlaying)
                audioSource.Play();
            if (onFinishedPlay != null)
                musicFinishedAwaiter = CoroutineHelper.Instance.StartCoroutine(AwaitMusicFinished(onFinishedPlay));

            while (audioSource.volume < MusicConfig.Volume)
            {
                audioSource.volume += Time.deltaTime * fadeInSpeed;
                yield return waiter;
            }

            audioSource.volume = MusicConfig.Volume;
            IsTransitioning = false;
        }
        private IEnumerator AwaitMusicFinished(Action callback) 
        {
            var waiter = new WaitForEndOfFrame();
            while (audioSource.isPlaying)
            {
                yield return waiter;
            }

            callback?.Invoke();
        }

        #region Helper
        private void LoadMusicConfig()
        {
            MusicConfig = Resources.Load<GameMusicConfig>("Configs/GameMusicConfig");
            if (MusicConfig == null)
            {
                Debug.LogError("Unable to find GameMusicConfig asset!");
            }
        }
        #endregion
    }
}