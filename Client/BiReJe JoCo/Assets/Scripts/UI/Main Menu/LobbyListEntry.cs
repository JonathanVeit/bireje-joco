using BiReJeJoCo.Backend;
using UnityEngine;
using UnityEngine.UI;

namespace BiReJeJoCo.UI
{
    public class LobbyListEntry : UIElement
    {
        [Header("Settings")]
        [SerializeField] Text hostName;
        [SerializeField] Text memberAmount;
        [SerializeField] Button joinBtn;
        [SerializeField] GameObject isRunningOverlay;

        private LobbyInfo lobbyInfo;

        public void Initialize(LobbyInfo lobby) 
        {
            lobbyInfo = lobby;
            hostName.text = lobby.HostName;
            memberAmount.text = $"{lobby.PlayerAmount} / {lobby.MaxPlayerAmount}";
            joinBtn.interactable = !lobby.IsFull;

            if (lobby.State == LobbyState.MatchRunning)
            {
                isRunningOverlay.SetActive(true);
            }
        }

        public void JoinLobby() 
        {
            uiManager.GetInstanceOf<MainMenuUI>().JoinLobby(lobbyInfo.LobbyId);
        }
    }
}