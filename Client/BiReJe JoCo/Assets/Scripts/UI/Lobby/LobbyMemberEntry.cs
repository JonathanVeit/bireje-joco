using BiReJeJoCo.Backend;
using UnityEngine;
using UnityEngine.UI;

namespace BiReJeJoCo.UI
{
    [System.Serializable]
    public class LobbyMemberEntry : UIElement
    {
        [Header("Settings")]
        [SerializeField] Text memberName;
        [SerializeField] Text memberState;
        [SerializeField] Button kickButton;

        private Player player;

        public void Initialize(Player player)
        {
            this.memberName.text = player.NickName;
            this.memberState.text = player.IsHost ? "Host" : "Client";
            kickButton.gameObject.SetActive(localPlayer.IsHost && !player.IsLocalPlayer);

            this.player = player;
        }

        public void KickPlayer() 
        {
            photonRoomWrapper.RemovePlayer(player.NumberInRoom);
        }
    }
}