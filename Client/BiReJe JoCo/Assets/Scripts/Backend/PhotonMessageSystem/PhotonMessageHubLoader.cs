using JoVei.Base;
using JoVei.Base.Helper;
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
            if (!localPlayer.IsHost) return;

            var result = Resources.LoadAll<PhotonMessageHub>("");

            if (result.Length > 0)
            {
                currentMessageHub = photonRoomWrapper.Instantiate(result[0].name, Vector3.zero, Quaternion.identity);
            }
            else
            {
                DebugHelper.Print("PhotonMessageHubLoader cannot find PhotonMessageHub prefab in resources.");
            }
        }

        private void OnLeftLobby(OnLeftLobbyMsg msg) 
        {
            if (!localPlayer.IsHost) return;

            photonRoomWrapper.Destroy(currentMessageHub);
        }
    }
}