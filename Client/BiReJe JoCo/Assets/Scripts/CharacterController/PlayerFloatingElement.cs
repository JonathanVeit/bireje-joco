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
        [SerializeField] SyncVar<int> playerHealth = new SyncVar<int>(0, 100);

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

            colorMesh.material.color = player.Role == PlayerRole.Hunted ? Color.red : Color.white;
        }

        private void SpawnFloaty()
        {
            var config = new FloatingElementConfig("player_character_name", gameUI.floatingElementGrid, floatingElementTarget);
            nameFloaty = floatingManager.GetElementAs<PlayerNameFloaty>(config);
            nameFloaty.Initialize(player.NickName);
            nameFloaty.SetVisibleMesh(floatingElementMesh);

            if (player.Role == PlayerRole.Hunted)
            {
                nameFloaty.ShowHealthBar(playerHealth);
            }
            else
            {
                nameFloaty.HideHealthbar();
            }
        }

        private void OnQuitMatch(PhotonMessage msg)
        {
            floatingManager.DestroyElement(nameFloaty);
            photonMessageHub.UnregisterReceiver<QuitMatchPhoMsg>(this, OnQuitMatch);
        }
    }
}
