using JoVei.Base;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace BiReJeJoCo
{
    public class OptionManager : SystemBehaviour, IInitializable
    {
        #region Initialization
        public IEnumerator Initialize(object[] parameters)
        {
            DIContainer.RegisterImplementation<OptionManager>(this);

            if (!PlayerPrefs.HasKey(OPTION_CHECK_KEY))
                SetDefaultSettings();

            ApplyOptions();
            yield return null;
        }
        public void CleanUp() { }

        private void ApplyOptions() 
        {
            SetMixerValue("MasterVolume", Volume);
            SetMixerValue("MusicVolume", Music);
            SetMixerValue("SoundEffectsVolume", Sound);
            SetQualityLevel(Quality);
        }
        #endregion

        #region Keys
        private const string OPTION_CHECK_KEY = "has_options";
        private const string VOLUME_KEY = "volume";
        private const string MUSIC_KEY = "music";
        private const string SOUND_KEY = "sound";
        private const string SENSITIVITY_KEY = "sensitivity";
        private const string QUALITY_KEY = "quality";
        #endregion

        private AudioMixer mixer;

        public float Volume 
        {
            get
            {
                return float.Parse(LoadValue(VOLUME_KEY));
            }
            set 
            {
                SaveValue(VOLUME_KEY, value);
            }
        }
        public float Music
        {
            get
            {
                return float.Parse(LoadValue(MUSIC_KEY));
            }
            set
            {
                SaveValue(MUSIC_KEY, value);
            }
        }
        public float Sound
        {
            get
            {
                return float.Parse(LoadValue(SOUND_KEY));
            }
            set
            {
                SaveValue(SOUND_KEY, value);
            }
        }
        public float Sensitivity
        {
            get
            {
                return float.Parse(LoadValue(SENSITIVITY_KEY));
            }
            set
            {
                SaveValue(SENSITIVITY_KEY, value);
            }
        }
        public int Quality
        {
            get
            {
                return int.Parse(LoadValue(QUALITY_KEY));
            }
            set
            {
                SaveValue(QUALITY_KEY, value);
            }
        }

        #region Helper
        private void SetDefaultSettings() 
        {
            Volume = 0.8f;
            Music  = 0.8f;
            Sound = 0.8f;
            Sensitivity = 1f;
            Quality = QualitySettings.GetQualityLevel();

            SaveValue(OPTION_CHECK_KEY, true);
        }
        public void SetMixerValue(string name, float value) 
        {
            if (mixer == null)
            {
                var allMixer = Resources.LoadAll<AudioMixer>("");
                if (allMixer.Length == 0)
                    return;

                mixer = allMixer[0];
            }

            mixer.SetFloat(name, Mathf.Lerp(-80, 0,value));
        }
        public void SetQualityLevel(int level)
        {
            QualitySettings.SetQualityLevel(level);
        }

        private void SaveValue(string key, object value)
        {
            PlayerPrefs.SetString(key, value.ToString());
        }
        private string LoadValue(string key)
        {
            if (!PlayerPrefs.HasKey(key))
                SetDefaultSettings();

            return PlayerPrefs.GetString(key);
        }
        #endregion
    }
}