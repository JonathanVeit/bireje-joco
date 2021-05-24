using JoVei.Base;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RoomInfo = Photon.Realtime.RoomInfo;

namespace BiReJeJoCo.Backend
{
    public class LobbyManager : SystemAccessor, IInitializable
    {
        private Dictionary<string, LobbyInfo> allLobbies;
        private LocalLobbyInfo currentLobby;

        #region Initialization
        public IEnumerator Initialize(object[] parameters)
        {
            DIContainer.RegisterImplementation<LobbyManager>(this);
            allLobbies = new Dictionary<string, LobbyInfo>();
            
            ConnectEvents();
            UpdateLobbyList(photonRoomWrapper.RoomList.ToArray());
            yield return null;
        }

        public void CleanUp()
        {
            DisconnectEvents();
        }

        private void ConnectEvents()
        {
            messageHub.RegisterReceiver<PhotonRoomListUpdatedMsg>(this, OnRoomListUpdated);
            messageHub.RegisterReceiver<JoinedLobbyMsg>(this, OnJoinedLobby);
            messageHub.RegisterReceiver<LeftLobbyMsg>(this, OnLeftLobby);
        }

        private void DisconnectEvents()
        {
            messageHub.UnregisterReceiver(this);
        }
        #endregion

        #region Events
        private void OnRoomListUpdated(PhotonRoomListUpdatedMsg msg)
        {
            UpdateLobbyList(msg.rooms);
        }
        private void UpdateLobbyList(RoomInfo[] rooms)
        {
            var roomDic = ConvertToDictionary(rooms);

            // add new lobbies
            foreach (var entry in roomDic)
            {
                if (entry.Value.RemovedFromList)
                {
                    allLobbies.Remove(entry.Value.Name);
                    break;
                }

                if (allLobbies.ContainsKey(entry.Key))
                {
                    allLobbies[entry.Key].Update(entry.Value);
                    break;
                }
             
                allLobbies.Add(entry.Key, new LobbyInfo(entry.Value));
            }

            messageHub.ShoutMessage(this, new LobbyListUpdatedMsg(GetOpenLobbies()));
        }

        private void OnJoinedLobby(JoinedLobbyMsg msg)
        {
            currentLobby = new LocalLobbyInfo(photonRoomWrapper.CurrentRoom);
        }
        private void OnLeftLobby(LeftLobbyMsg msg)
        {
            currentLobby = null;
        }
        #endregion

        public LobbyInfo[] GetOpenLobbies() 
        {
            return allLobbies.Values.ToArray();
        }
        public LocalLobbyInfo GetCurrentLobby() 
        {
            return currentLobby;
        }

        #region Helper
        private Dictionary<string, RoomInfo> ConvertToDictionary(RoomInfo[] infos) 
        {
            var result = new Dictionary<string, RoomInfo>();
            foreach (var info in infos)
            {
                result.Add(info.Name, info);
            }

            return result;
        }
        #endregion
    }
}