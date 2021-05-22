using Photon.Realtime;
using System.Collections.Generic;

namespace BiReJeJoCo.Backend
{
    public class LobbyInfo
    {
        public int HostId => photonRoomInfo.masterClientId;
        public string Name => photonRoomInfo.Name;
        public int PlayerAmount => photonRoomInfo.PlayerCount;
        public int MaxPlayerAmount => photonRoomInfo.MaxPlayers;

        private RoomInfo photonRoomInfo;

        public LobbyInfo(RoomInfo photonRoomInfo)
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
    }
}
