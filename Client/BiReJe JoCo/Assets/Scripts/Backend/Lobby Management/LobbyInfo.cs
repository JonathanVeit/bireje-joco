using Photon.Realtime;
using System;
using System.Collections.Generic;

namespace BiReJeJoCo.Backend
{
    public enum LobbyState
    {
        None = 0,
        Open = 1, 
        MatchRunning = 2,
    }

    public class LobbyInfo
    {
        public string LobbyId => photonRoomInfo.Name;
        public string HostName => GetHostName();

        public int PlayerAmount => photonRoomInfo.PlayerCount;
        public int MaxPlayerAmount => photonRoomInfo.MaxPlayers;
        public bool IsFull => PlayerAmount >= MaxPlayerAmount;
        public bool IsOpen => photonRoomInfo.IsOpen;

        public LobbyState State => GetLobbyState();

        private RoomInfo photonRoomInfo;

        public LobbyInfo(RoomInfo photonRoomInfo)
        {
            this.photonRoomInfo = photonRoomInfo;
        }
        public void Update(RoomInfo photonRoomInfo)
        {
            this.photonRoomInfo = photonRoomInfo;
        }

        public static LobbyInfo[] Create(IEnumerable<RoomInfo> photonRooms)
        {
            var result = new List<LobbyInfo>();
            foreach (var entry in photonRooms)
            {
                result.Add(new LobbyInfo(entry));
            }

            return result.ToArray();
        }

        private string GetHostName()
        {
            return photonRoomInfo.CustomProperties["HN"].ToString();
        }
        private LobbyState GetLobbyState() 
        {
            var rawValue = photonRoomInfo.CustomProperties["LS"].ToString();
            return (LobbyState)Enum.Parse(typeof(LobbyState), rawValue);
        }
    }
}