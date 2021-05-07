using JoVei.Base;
using JoVei.Base.Helper;
using UnityEngine;
using System.Collections;

namespace BiReJeJoCo.Backend
{
    public class SyncVarHubLoader : SystemAccessor, IInitializable
    {
        public IEnumerator Initialize(object[] parameters)
        {
            messageHub.RegisterReceiver<OnJoinedLobbyMsg>(this, OnJoinedLobby);
            messageHub.RegisterReceiver<LeftLobbyMsg>(this, OnLeftLobby);
            yield return null;
        }

        public void CleanUp()
        {
            messageHub.UnregisterReceiver(this);
        }

        private void OnJoinedLobby(OnJoinedLobbyMsg msg)
        {
            var result = Resources.LoadAll<SyncVarHub>("");

            if (result.Length > 0)
            {
                GameObject.Instantiate(result[0], Vector3.zero, Quaternion.identity);
            }
            else
            {
                DebugHelper.Print("PhotonVariableHubLoader cannot find PhotonVariableHub prefab in resources.");
            }
        }

        private void OnLeftLobby(LeftLobbyMsg msg)
        {
            if (syncVarHub != null)
                GameObject.Destroy(syncVarHub.gameObject);
        }
    }
}