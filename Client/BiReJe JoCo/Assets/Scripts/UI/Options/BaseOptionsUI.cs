using UnityEngine;
using UnityEngine.UI;

namespace BiReJeJoCo.UI
{
    public class BaseOptionsUI : UIElement
    {
        [Header("Settings")]
        [SerializeField] Slider volumeSlider;
        [SerializeField] Slider musicSlider;
        [SerializeField] Slider soundSlider;
        [SerializeField] Slider sensitivitySlider;
        [SerializeField] Dropdown qualityDropdown;

        #region
        public void Initialize()
        {
            SetVolume(optionManager.Volume);
            SetMusic(optionManager.Music);
            SetSound(optionManager.Sound);
            SetSensitivity(optionManager.Sensitivity);
            SetQuality(optionManager.Quality);
        }
        #endregion

        #region UI
        public void SetVolume(float value)
        {
            volumeSlider.value = value;
            optionManager.Volume = value;
            optionManager.SetMixerValue("MasterVolume", value);
        }
        public void SetMusic(float value)
        {
            musicSlider.value = value;
            optionManager.Music = value;
            optionManager.SetMixerValue("MusicVolume", value);
        }
        public void SetSound(float value)
        {
            soundSlider.value = value;
            optionManager.Sound = value;
            optionManager.SetMixerValue("SoundEffectsVolume", value);
        }
        public void SetSensitivity(float value)
        {
            sensitivitySlider.value = value;
            optionManager.Sensitivity = value;
        }
        public void SetQuality(int value)
        {
            qualityDropdown.value = value;
            optionManager.Quality = value;
            optionManager.SetQualityLevel(value);
        }
        #endregion

        #region UI Inputs
        public void ReturnToMenu()
        {
            gameManager.OpenMainMenu();
        }
        #endregion
    }
}