using BiReJeJoCo.Backend;
using UnityEngine;
using JoVei.Base.UI;
using BiReJeJoCo.UI;

namespace BiReJeJoCo.Character
{
    public class PlayerFloatingElement : SystemBehaviour, IPlayerControlled
    {
        [SerializeField] Transform floatingElementTarget;
        [SerializeField] MeshRenderer floatingElementMesh;
        private PlayerNameFloaty nameFloaty;

        private Player player;
        public void Initialize(Player player)
        {
            this.player = player;

            if (!player.IsLocalPlayer)
            {
                SpawnFloaty();
                photonMessageHub.RegisterReceiver<QuitMatchPhoMsg>(this, OnQuitMatch);
            }
        }

        private void SpawnFloaty()
        {
            var config = new FloatingElementConfig("player_character_name", gameUI.floatingElementGrid, floatingElementTarget);
            nameFloaty = floatingManager.GetElementAs<PlayerNameFloaty>(config);
            nameFloaty.Initialize(player.NickName);
            nameFloaty.SetVisibleMesh(floatingElementMesh);
        }

        private void OnQuitMatch(PhotonMessage msg)
        {
            floatingManager.DestroyElement(nameFloaty);
            photonMessageHub.UnregisterReceiver<QuitMatchPhoMsg>(this, OnQuitMatch);
        }
    }
}
