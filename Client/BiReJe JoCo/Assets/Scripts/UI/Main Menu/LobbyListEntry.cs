using BiReJeJoCo.Backend;
using UnityEngine;
using UnityEngine.UI;

namespace BiReJeJoCo.UI
{
    public class LobbyListEntry : UIElement
    {
        [Header("Settings")]
        [SerializeField] Text hostName;
        [SerializeField] Text lobbyName;
        [SerializeField] Text memberAmount;
        [SerializeField] Button joinBtn;

        private LobbyInfo lobbyInfo;

        public void Initialize(LobbyInfo lobby) 
        {
            lobbyInfo = lobby;
            hostName.text = lobby.Name;
            memberAmount.text = $"{lobby.PlayerAmount} / {lobby.MaxPlayerAmount}";
        }

        public void JoinLobby() 
        {
            uiManager.GetInstanceOf<MainMenuUI>().JoinLobby(lobbyInfo.Name);
        }
    }
}