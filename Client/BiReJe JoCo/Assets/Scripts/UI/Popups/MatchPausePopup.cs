using BiReJeJoCo.Backend;
using UnityEngine;
using UnityEngine.UI;

namespace BiReJeJoCo.UI
{
    public class MatchPausePopup : Popup
    {
        [Header("UI Elements")]
        [SerializeField] Button continueButton;
        [SerializeField] Button rehostButton;
        [SerializeField] Button quitButton;

        protected override void OnSystemsInitialized()
        {
            base.OnSystemsInitialized();
        }

        public void Show(MatchResult result)
        {
            base.Show();

            rehostButton.gameObject.SetActive(localPlayer.IsHost);
            quitButton.gameObject.SetActive(localPlayer.IsHost);
        }

        #region UI Input
        public void Continue()
        {
            messageHub.ShoutMessage<PauseMenuClosedMsg>(this);
        }

        public void Rehost() 
        {
            (matchHandler as HostMatchHandler).CloseMatch(CloseMatchMode.ReturnToLobby);
        }

        public void Quit()
        {
            (matchHandler as HostMatchHandler).CloseMatch(CloseMatchMode.LeaveLobby);
        }
        #endregion
    }
}