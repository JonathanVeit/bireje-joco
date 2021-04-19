using UnityEngine;
using UnityEngine.UI;
using JoVei.Base.UI;

namespace BiReJeJoCo.UI
{
    public class LoadingScreenUI : UIElement
    {
        [SerializeField] UIBarHandler loadingBar;
        [SerializeField] Text loadingBarCaption;
        [SerializeField] Text loadingBarProgressCaption;

        private void Start()
        {
            systemsLoader.OnSystemLoaded += OnSystemLoaded;
            systemsLoader.OnAllSystemsLoaded += OnAllSystemsLoaded;
            loadingBar.OverrideValue(0);
        }

        private void OnSystemLoaded(object system) 
        {
            loadingBar.SetValue(systemsLoader.CurrentProgress);
            loadingBarCaption.text = string.Format("{0} loaded...", system.GetType().Name);
            loadingBarProgressCaption.text = string.Format("{0}%", Mathf.RoundToInt(systemsLoader.CurrentProgress * 100));
        }

        private void OnAllSystemsLoaded() 
        {
            loadingBar.SetValue(1);
            loadingBarCaption.text = "finished...";
            loadingBarProgressCaption.text = "100%";

            gameManager.OpenMainMenu();
        }

        protected override void OnBeforeDestroy()
        {
            systemsLoader.OnStartLoadingSystem -= OnSystemLoaded;
            systemsLoader.OnAllSystemsLoaded -= OnAllSystemsLoaded;
        }
    }
}