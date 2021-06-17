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
            rehostButton.gameObject.SetActive(localPlayer.IsHost);
            quitButton.gameObject.SetActive(localPlayer.IsHost);
        }

        public override void Show()
        {
            base.Show();

            rehostButton.gameObject.SetActive(localPlayer.IsHost);
            quitButton.gameObject.SetActive(localPlayer.IsHost);

            Cursor.lockState = CursorLockMode.Confined;
            messageHub.ShoutMessage(this, new BlockPlayerControlsMsg(Character.InputBlockState.Menu));
        }

        public override void Hide()
        {
            base.Hide();
            Cursor.lockState = CursorLockMode.Locked;
            messageHub.ShoutMessage(this, new UnblockPlayerControlsMsg(Character.InputBlockState.Free));
        }

        #region UI Input
        public void Continue()
        {
            Hide();
        }

        public void Rehost() 
        {
            (matchHandler as HostMatchHandler).CloseMatch(CloseMatchMode.ReturnToLobby);
        }

        public void ShowControls() 
        {
            Hide();
            uiManager.GetInstanceOf<ControlsPopup>().Show();
        }
        public void Quit()
        {
            if (matchHandler is HostMatchHandler asHostHandler)
                asHostHandler.CloseMatch(CloseMatchMode.LeaveLobby);
        }
        #endregion
    }
}