using BiReJeJoCo.Backend;
using UnityEngine;
using JoVei.Base.UI;
using BiReJeJoCo.UI;

namespace BiReJeJoCo.Character
{
    public class PlayerFloatingElement : SystemBehaviour, IPlayerObserved
    {
        [SerializeField] MeshRenderer colorMesh;
        [SerializeField] Transform floatingElementTarget;
        [SerializeField] MeshRenderer floatingElementMesh;


        private PlayerNameFloaty nameFloaty;


        private PlayerControlled controller;
        public Player Owner => controller.Player;

        public void Initialize(PlayerControlled controller)
        {
            this.controller = controller;

            if (!Owner.IsLocalPlayer)
            {
                SpawnFloaty();
                photonMessageHub.RegisterReceiver<CloseMatchPhoMsg>(this, OnMatchClosed);
            }

            colorMesh.material.color = Owner.Role == PlayerRole.Hunted ? Color.red : Color.white;
        }

        private void SpawnFloaty()
        {
            if (Owner.Role == PlayerRole.Hunter && 
                localPlayer.Role == PlayerRole.Hunter)
            {
                var config = new FloatingElementConfig("player_character_name", uiManager.GetInstanceOf<GameUI>().floatingElementGrid, floatingElementTarget);
                nameFloaty = floatingManager.GetElementAs<PlayerNameFloaty>(config);
                nameFloaty.Initialize(Owner.NickName);
                nameFloaty.SetVisibleRenderer(floatingElementMesh);
                //nameFloaty.ShowHealthBar(playerHealth);
            }
        }

        private void OnMatchClosed(PhotonMessage msg)
        {
            floatingManager.DestroyElement(nameFloaty);
            photonMessageHub.UnregisterReceiver(this);
        }
    }
}
