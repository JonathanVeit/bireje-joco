using BiReJeJoCo.Backend;
using UnityEngine;
using UnityEngine.UI;

namespace BiReJeJoCo.UI
{
    public class MatchResultPopup : Popup
    {
        [Header("UI Elements")]
        [SerializeField] Text resultLabel;
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

            resultLabel.text = result.winner == PlayerRole.Hunted ? "Hunted wins!" : "Hunter win!";
        }

        #region UI Input
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