using JoVei.Base;
using System.Collections;
using UnityEngine;
using BiReJeJoCo.Backend;

namespace BiReJeJoCo.Items
{
    public class CollectablesManagerLoader : SystemAccessor, IInitializable
    {
        public IEnumerator Initialize(object[] parameters)
        {
            messageHub.RegisterReceiver<JoinedLobbyMsg>(this, OnJoinedLobby);
            messageHub.RegisterReceiver<LeftLobbyMsg>(this, OnLeftLobby);
            yield return null;
        }

        public void CleanUp()
        {
            messageHub.UnregisterReceiver<JoinedLobbyMsg>(this, OnJoinedLobby);
        }

        private void OnJoinedLobby(JoinedLobbyMsg msg)
        {
            var result = Resources.LoadAll<CollectablesManager>("");

            if (result.Length > 0)
            {
                GameObject.Instantiate(result[0].gameObject, Vector3.zero, Quaternion.identity);
            }
            else
            {
                Debug.LogError("Cannot find CollectablesManager prefab in resources.");
            }
        }


        private void OnLeftLobby(LeftLobbyMsg msg)
        {
            if (collectablesManager != null)
                GameObject.Destroy(collectablesManager.gameObject);
        }
    }
}