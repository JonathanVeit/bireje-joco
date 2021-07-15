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

        public void AntiStuck() 
        {
            Vector3 pos = default;
            var scene = matchHandler.MatchConfig.matchScene;
            var config = MapConfigMapping.GetMapping().GetElementForKey(scene);

            switch (localPlayer.Role)
            {
                case PlayerRole.Hunted:
                    pos = config.GetHuntedSpawnPoint(config.GetRandomHuntedSpawnPointIndex());
                    break;

                case PlayerRole.Hunter:
                    pos = config.GetHuntedSpawnPoint(config.GetRandomHunterSpawnPointIndex());
                    break;
            }

            localPlayer.PlayerCharacter.ControllerSetup.CharacterRoot.transform.position = pos;
            localPlayer.PlayerCharacter.ControllerSetup.Mover.SetVelocity(Vector3.zero);
            localPlayer.PlayerCharacter.ControllerSetup.RigidBody.velocity = Vector3.zero;
        }
    }
}