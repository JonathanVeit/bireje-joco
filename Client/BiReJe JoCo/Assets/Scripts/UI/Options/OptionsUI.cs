using UnityEngine;

namespace BiReJeJoCo.UI
{
    public class OptionsUI : UIElement
    {
        [Header("Settings")]
        [SerializeField] GameObject optionsPanel;
        [SerializeField] GameObject creditsOverlay;

        #region Initialization
        protected override void OnSystemsInitialized()
        {
            uiManager.GetInstanceOf<BaseOptionsUI>().Initialize();
        }
        #endregion

        public void ReturnToMenu() 
        {
            gameManager.OpenMainMenu();
        }
        public void ShowCredits(bool show)
        {
            optionsPanel.SetActive(!show);
            creditsOverlay.SetActive(show);
        }
    }
}