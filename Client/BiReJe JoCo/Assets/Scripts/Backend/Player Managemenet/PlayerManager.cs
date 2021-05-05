using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using JoVei.Base;
using PhotonPlayer = Photon.Realtime.Player;

namespace BiReJeJoCo.Backend
{
    public class PlayerManager : SystemAccessor, IInitializable
    {
        #region Initialization
        public IEnumerator Initialize(object[] parameters)
        {
            DIContainer.RegisterImplementation<PlayerManager>(this);
            ConnectEvents();

            RegisterPlayer(PhotonNetwork.LocalPlayer, true);
            yield return null;
        }

        public void CleanUp()
        {
            DisconnectEvents();
        }

        private void ConnectEvents()
        {
            messageHub.RegisterReceiver<PlayerJoinedLobbyMsg>(this, OnPlayerJoined);
            messageHub.RegisterReceiver<PlayerLeftLobbyMsg>(this, OnPlayerLeft);

            messageHub.RegisterReceiver<OnJoinedLobbyMsg>(this, OnJoinedRoom);
            messageHub.RegisterReceiver<LeftLobbyMsg>(this, OnLeftRoom);
        }

        private void DisconnectEvents()
        {
            messageHub.UnregisterReceiver<PlayerJoinedLobbyMsg>(this, OnPlayerJoined);
            messageHub.UnregisterReceiver<PlayerLeftLobbyMsg>(this, OnPlayerLeft);

            messageHub.UnregisterReceiver<OnJoinedLobbyMsg>(this, OnJoinedRoom);
            messageHub.UnregisterReceiver<LeftLobbyMsg>(this, OnLeftRoom);
        }
        #endregion

        private Dictionary<string, Player> allPlayer
            = new Dictionary<string, Player>();
        private string localPlayerId;

        public LocalPlayer LocalPlayer => GetPlayer(localPlayerId) as LocalPlayer;
        public Player Host => FindHost();

        public Player GetPlayer(string id)
        {
            return allPlayer[id];
        }
        public Player GetPlayer(int actorNumber)
        {
            foreach (var curPlayer in allPlayer.Values)
            {
                if (curPlayer.NumberInRoom == actorNumber) 
                    return curPlayer;
            }
            return null;
        }
        public Player[] GetAllPlayer()
        {
            return allPlayer.Values.ToArray();
        }

        public bool HasPlayer(string id)
        {
            return allPlayer.ContainsKey(id);
        }
        public bool HasPlayer(int actorNumber)
        {
            foreach (var curPlayer in allPlayer.Values)
            {
                if (curPlayer.NumberInRoom == actorNumber)
                    return true;
            }
            return false;
        }

        private void RegisterPlayer(PhotonPlayer forPlayer, bool isLocal = false)
        {
            if (allPlayer.ContainsKey(forPlayer.UserId)) return;

            Player newPlayer;
            if (isLocal)
            {
                newPlayer = new LocalPlayer(forPlayer);
                localPlayerId = newPlayer.Id;
            }
            else
            {
                newPlayer = new Player(forPlayer);
            }

            allPlayer.Add(newPlayer.Id, newPlayer);
            messageHub.ShoutMessage<AddedPlayerMsg>(this, new AddedPlayerMsg(newPlayer));
        }

        private void UnregisterPlayer(string playerId)
        {
            var tmp = allPlayer[playerId];
            allPlayer.Remove(playerId);
            messageHub.ShoutMessage<RemovedPlayerMsg>(this, new RemovedPlayerMsg(tmp));
        }

        #region Photon Events
        private void OnPlayerJoined(PlayerJoinedLobbyMsg msg)
        {
            var photonPlayer = GetPhotonPlayer(msg.Param1);
            RegisterPlayer(photonPlayer, false);
        }

        private void OnPlayerLeft(PlayerLeftLobbyMsg msg) 
        {
            UnregisterPlayer(msg.Param1);
        }

        private void OnJoinedRoom(OnJoinedLobbyMsg msg)
        {
            foreach (var curPlayer in photonRoomWrapper.PlayerList)
                RegisterPlayer(curPlayer, false);
        }

        private void OnLeftRoom(LeftLobbyMsg msg)
        {
            foreach (var curPlayer in allPlayer.Values.ToArray())
            {
                if (curPlayer != LocalPlayer)
                    UnregisterPlayer(curPlayer.Id);
            }
        }
        #endregion

        #region Helper
        private PhotonPlayer GetPhotonPlayer(string playerId) 
        {
            foreach (var curPlayer in photonRoomWrapper.PlayerList)
            {
                if (curPlayer.UserId == playerId)
                    return curPlayer;
            }

            return null;
        }

        private Player FindHost() 
        {
            foreach (var curPlayer in allPlayer.Values)
            {
                if (curPlayer.IsHost)
                    return curPlayer;
            }

            return null;
        }
        #endregion
    }
}