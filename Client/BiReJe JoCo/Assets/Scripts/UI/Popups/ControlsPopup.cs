using UnityEngine;
using UnityEngine.UI;

namespace BiReJeJoCo.UI
{
    public class ControlsPopup : Popup
    {
        [Header("UI Elements")]
        [SerializeField] Image hunterControls;
        [SerializeField] Image huntedControls;

        public override void Show()
        {
            base.Show();

            Cursor.lockState = CursorLockMode.Confined;
            messageHub.ShoutMessage(this, new BlockPlayerControlsMsg(Character.InputBlockState.Menu));

            UpdateUI();
        }

        private void UpdateUI() 
        {
            hunterControls.gameObject.SetActive(localPlayer.Role == Backend.PlayerRole.Hunter);
            huntedControls.gameObject.SetActive(localPlayer.Role == Backend.PlayerRole.Hunted);
        }

        public override void Hide()
        {
            base.Hide();
            Cursor.lockState = CursorLockMode.Locked;
            messageHub.ShoutMessage(this, new UnblockPlayerControlsMsg(Character.InputBlockState.Free));
        }

        public void HideAndReshow()
        {
            Cursor.lockState = CursorLockMode.Locked;
            messageHub.ShoutMessage(this, new UnblockPlayerControlsMsg(Character.InputBlockState.Free));

            LastClosed.Show();
            base.Hide();
        }
    }
}