using JoVei.Base;
using JoVei.Base.Helper;
using Photon.Pun;
using UnityEngine;
using System.Collections;

namespace BiReJeJoCo.Backend
{
    public class PhotonMessageHubLoader : SystemAccessor, IInitializable
    {
        private GameObject currentMessageHub;

        public IEnumerator Initialize(object[] parameters)
        {
            messageHub.RegisterReceiver<OnJoinedLobbyMsg>(this, OnJoinedLobby);
            messageHub.RegisterReceiver<OnLeftLobbyMsg>(this, OnLeftLobby);

            yield return null;
        }

        public void CleanUp()
        {
            messageHub.UnregisterReceiver<OnJoinedLobbyMsg>(this, OnJoinedLobby);
            messageHub.UnregisterReceiver<OnLeftLobbyMsg>(this, OnLeftLobby);
        }

        private void OnJoinedLobby(OnJoinedLobbyMsg msg)
        {
            if (!photonRoomWrapper.IsHost) return;

            var result = Resources.LoadAll<PhotonMessageHub>("");

            if (result.Length > 0)
            {
                currentMessageHub = PhotonNetwork.Instantiate(result[0].name, Vector2.zero, Quaternion.identity);
            }
            else
            {
                DebugHelper.Print("PhotonMessageHubLoader cannot find PhotonMessageHub prefab in resources.");
            }
        }

        private void OnLeftLobby(OnLeftLobbyMsg msg) 
        {
            if (!photonRoomWrapper.IsHost) return;

            PhotonNetwork.Destroy(currentMessageHub);
        }
    }
}