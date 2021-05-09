using BiReJeJoCo.Backend;
using UnityEngine;
using JoVei.Base.UI;
using BiReJeJoCo.UI;

namespace BiReJeJoCo.Character
{
    public class PlayerFloatingElement : SystemBehaviour, IPlayerObserved
    {
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
            }
        }

        private void OnMatchClosed(PhotonMessage msg)
        {
            if (nameFloaty == null) return;
            nameFloaty.gameObject.SetActive(false);
            floatingManager.DestroyElement(nameFloaty);
            photonMessageHub.UnregisterReceiver(this);
        }
    }
}
