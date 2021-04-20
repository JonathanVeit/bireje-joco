using PhotonPlayer = Photon.Realtime.Player;
using ExitGames.Client.Photon;

namespace BiReJeJoCo.Backend
{
    public class LocalPlayer : Player
    {
        public LocalPlayer(PhotonPlayer player) : base(player)
        {
            var properties = new Hashtable()
            {
                { "State", PlayerState.Free }
            };

            rootPlayer.SetCustomProperties(properties);

            ConnectToEvents();
        }

        private void ConnectToEvents()
        {
            messageHub.RegisterReceiver<OnJoinLobbyFailedMsg>(this, OnJoinedLobby);
            messageHub.RegisterReceiver<OnLeftLobbyMsg>(this, OnLeftRoom);

            messageHub.RegisterReceiver<OnLoadedGameMsg>(this, OnGameLoaded);
        }


        #region Messages
        private void OnJoinedLobby(OnJoinLobbyFailedMsg msg)
        {
            UpdateState(PlayerState.Lobby);
        }

        private void OnLeftRoom(OnLeftLobbyMsg msg) 
        {
            UpdateState(PlayerState.Free);
        }

        private void OnGameLoaded(OnLoadedGameMsg msg)
        {
            UpdateState(PlayerState.Ready);
        }
        #endregion

        #region Helper
        private void UpdateState(PlayerState state)
        {
            var properties = rootPlayer.CustomProperties;

            properties["State"] = state;
            rootPlayer.SetCustomProperties(properties);
        }
        #endregion
    }
}