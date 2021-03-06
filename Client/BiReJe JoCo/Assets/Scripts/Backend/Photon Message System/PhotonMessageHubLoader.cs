using JoVei.Base;
using UnityEngine;
using System.Collections;

namespace BiReJeJoCo.Backend
{
    public class PhotonMessageHubLoader : SystemAccessor, IInitializable
    {
        public IEnumerator Initialize(object[] parameters)
        {
            messageHub.RegisterReceiver<JoinedLobbyMsg>(this, OnJoinedLobby);

            yield return null;
        }

        public void CleanUp()
        {
            messageHub.UnregisterReceiver<JoinedLobbyMsg>(this, OnJoinedLobby);
        }

        private void OnJoinedLobby(JoinedLobbyMsg msg)
        {
            if (!localPlayer.IsHost) return;

            var result = Resources.LoadAll<PhotonMessageHub>("");

            if (result.Length > 0)
            {
                photonRoomWrapper.Instantiate(result[0].name, Vector3.zero, Quaternion.identity, true);           
            }
            else
            {
                Debug.LogError("Cannot find PhotonMessageHub prefab in resources.");
            }
        }
    }
}