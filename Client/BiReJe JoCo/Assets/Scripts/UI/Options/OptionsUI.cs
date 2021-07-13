namespace BiReJeJoCo.UI
{
    public class OptionsUI : UIElement
    {
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
    }
}