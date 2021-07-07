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
        [SerializeField] GameObject stateIcon;
        [SerializeField] Button kickButton;
        [SerializeField] Image preferedRoleIcon;
        [SerializeField] GameObject outline;

        private Player player;

        public void Initialize(Player player)
        {
            this.memberName.text = player.NickName;
            this.stateIcon.SetActive(player.IsHost);
            kickButton.gameObject.SetActive(localPlayer.IsHost && !player.IsLocalPlayer);
            
            this.player = player;
        }

        private void Update()
        {
            var icon = SpriteMapping.GetMapping().GetElementForKey("role_" + player.PreferedRole.ToString());
            preferedRoleIcon.sprite = icon;

            outline.gameObject.SetActive(player.ReadToStart);
        }

        public void KickPlayer() 
        {
            photonRoomWrapper.RemovePlayer(player.NumberInRoom);
        }
    }
}