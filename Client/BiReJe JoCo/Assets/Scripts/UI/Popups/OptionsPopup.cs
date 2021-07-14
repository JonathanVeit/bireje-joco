using BiReJeJoCo.Backend;
using UnityEngine;

namespace BiReJeJoCo.UI
{
    public class OptionsPopup : Popup
    {
        public override void Show()
        {
            base.Show();
            uiManager.GetInstanceOf<BaseOptionsUI>().Initialize();

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
            Cursor.lockState = CursorLockMode.Locked;
            messageHub.ShoutMessage(this, new UnblockPlayerControlsMsg(Character.InputBlockState.Free));

            LastClosed.Show();
            base.Hide();
        }
    }
}