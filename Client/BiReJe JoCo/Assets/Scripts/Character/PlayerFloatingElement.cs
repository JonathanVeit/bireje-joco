using BiReJeJoCo.Backend;
using UnityEngine;
using JoVei.Base.UI;
using BiReJeJoCo.UI;

namespace BiReJeJoCo.Character
{
    public class PlayerFloatingElement : SystemBehaviour, IPlayerObserved
    {
        [SerializeField] Transform floatingElementTarget;

        private PlayerNameFloaty nameFloaty;
        private PlayerControlled controller;
        public Player Owner => controller.Player;

        public void Initialize(PlayerControlled controller)
        {
            this.controller = controller;

            if (localPlayer.Role == PlayerRole.Hunted) return;
            SpawnFloaty();
            photonMessageHub.RegisterReceiver<CloseMatchPhoMsg>(this, OnMatchClosed);
        }

        private void SpawnFloaty()
        {
            var config = new FloatingElementConfig("player_character_name", uiManager.GetInstanceOf<GameUI>().floatingElementGrid, floatingElementTarget);
            nameFloaty = floatingManager.GetElementAs<PlayerNameFloaty>(config);
            nameFloaty.Initialize(Owner.NickName);
        }

        private void OnMatchClosed(PhotonMessage msg)
        {
            if (nameFloaty == null) return;
            floatingManager.DestroyElement(nameFloaty);
            photonMessageHub.UnregisterReceiver(this);
        }
    }
}
