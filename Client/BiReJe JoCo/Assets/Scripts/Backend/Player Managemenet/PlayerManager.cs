using System;
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
            messageHub.RegisterReceiver<OnPlayerJoinedLobbyMsg>(this, OnPlayerJoined);
            messageHub.RegisterReceiver<OnPlayerLeftLobbyMsg>(this, OnPlayerLeft);

            messageHub.RegisterReceiver<OnJoinedLobbyMsg>(this, OnJoinedRoom);
            messageHub.RegisterReceiver<OnLeftLobbyMsg>(this, OnLeftRoom);
        }

        private void DisconnectEvents()
        {
            messageHub.UnregisterReceiver<OnPlayerJoinedLobbyMsg>(this, OnPlayerJoined);
            messageHub.UnregisterReceiver<OnPlayerLeftLobbyMsg>(this, OnPlayerLeft);

            messageHub.UnregisterReceiver<OnJoinedLobbyMsg>(this, OnJoinedRoom);
            messageHub.UnregisterReceiver<OnLeftLobbyMsg>(this, OnLeftRoom);
        }
        #endregion

        public event Action<Player> onPlayerAdded;
        public event Action<Player> onPlayerRemoved;

        private Dictionary<string, Player> allPlayer
            = new Dictionary<string, Player>();
        private string localPlayerId;
        
        public LocalPlayer LocalPlayer => GetPlayer(localPlayerId) as LocalPlayer;
        
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

            onPlayerAdded?.Invoke(newPlayer);
        }

        private void UnregisterPlayer(string playerId)
        {
            var tmp = allPlayer[playerId];
            allPlayer.Remove(playerId);
            onPlayerRemoved?.Invoke(tmp);
        }

        #region Photon Events
        private void OnPlayerJoined(OnPlayerJoinedLobbyMsg msg)
        {
            var photonPlayer = GetPhotonPlayer(msg.Param1);
            RegisterPlayer(photonPlayer, false);
        }

        private void OnPlayerLeft(OnPlayerLeftLobbyMsg msg) 
        {
            UnregisterPlayer(msg.Param1);
        }

        private void OnJoinedRoom(OnJoinedLobbyMsg msg)
        {
            foreach (var curPlayer in photonRoomWrapper.PlayerList)
                RegisterPlayer(curPlayer, false);
        }

        private void OnLeftRoom(OnLeftLobbyMsg msg)
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
        #endregion
    }
}