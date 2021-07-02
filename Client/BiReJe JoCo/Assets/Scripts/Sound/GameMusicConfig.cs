using System;
using System.Collections.Generic;
using UnityEngine;

namespace BiReJeJoCo.Audio
{
    [CreateAssetMenu(fileName = "GameMusicConfig", menuName = "Config/GameMusicConfig")]
    public class GameMusicConfig : ScriptableObject
    {
        [Header("Settings")]
        [SerializeField] List<MusicClipConfig> musicClips;
        [SerializeField] float startChaseMusicDelay;
        [SerializeField] float endChaseMusicDelay;

        #region Access
        public bool GetClip(MusicState situation, out MusicClipConfig clipInfo)
        {
            var matchingClips = musicClips.FindAll(x => x.musicState == situation);

            if (matchingClips.Count > 0)
            {
                clipInfo = matchingClips[0];
                return true;
            }

            clipInfo = default;
            return false;
        }

        public float StartChaseMusicDelay => startChaseMusicDelay;
        public float EndChaseMusicDelay => endChaseMusicDelay;
        #endregion

        [Serializable]
        public struct MusicClipConfig
        {
            public string name;
            public MusicState musicState;
            public AudioClip clip;
            public float fadeIn;
            public float fadeOut;
        }
    }
}