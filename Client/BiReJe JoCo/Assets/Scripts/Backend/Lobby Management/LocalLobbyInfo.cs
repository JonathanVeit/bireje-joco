using Photon.Realtime;
using System;

namespace BiReJeJoCo.Backend
{
    public class LocalLobbyInfo
    {
        public string LobbyId => photonRoom.Name;
        public string HostName => GetHostName();

        public int PlayerAmount => photonRoom.PlayerCount;
        public int MaxPlayerAmount => photonRoom.MaxPlayers;
        public bool IsFull => PlayerAmount >= MaxPlayerAmount;

        public LobbyState State => GetLobbyState();

        private Room photonRoom;

        public LocalLobbyInfo(Room photonRoom)
        {
            this.photonRoom = photonRoom;
        }

        private string GetHostName()
        {
            return photonRoom.CustomProperties["HN"].ToString();
        }
        private LobbyState GetLobbyState()
        {
            var rawValue = photonRoom.CustomProperties["LS"].ToString();
            return (LobbyState)Enum.Parse(typeof(LobbyState), rawValue);
        }

        #region Exposed Methods
        public void SetState(LobbyState state)
        {
            UpdateProperty("LS", state);
        }

        public void Open()
        {
            photonRoom.IsOpen = true;
        }
        public void Close() 
        {
            photonRoom.IsOpen = false;
        }
        #endregion

        #region Helper
        private void UpdateProperty(string key, object value)
        {
            UpdateProperties(key, value);
        }

        private void UpdateProperties(params object[] keyValuePairs)
        {
            if (keyValuePairs.Length % 2 != 0)
                throw new System.ArgumentException($"Cannot update properties. Missing value for key {keyValuePairs[keyValuePairs.Length - 1]}");

            var properties = photonRoom.CustomProperties;
            for (int i = 0; i < keyValuePairs.Length; i += 2)
            {
                properties[keyValuePairs[i]] = keyValuePairs[i + 1];
            }

            Photon.Pun.PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
        }
        #endregion
    }
}