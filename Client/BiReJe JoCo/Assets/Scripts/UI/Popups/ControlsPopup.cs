using UnityEngine;

namespace BiReJeJoCo.UI
{
    public class ControlsPopup : Popup
    {
        public override void Show()
        {
            base.Show();

            Cursor.lockState = CursorLockMode.Confined;
            messageHub.ShoutMessage(this, new BlockPlayerControlsMsg(Character.InputBlockState.Menu));
        }

        public override void Hide()
        {
            base.Hide();
            Cursor.lockState = CursorLockMode.Locked;
            messageHub.ShoutMessage(this, new UnblockPlayerControlsMsg(Character.InputBlockState.Free));
        }

        public void HideAndReshow()
        {
            Hide();
            History[History.Count - 1].Show();
        }
    }
}