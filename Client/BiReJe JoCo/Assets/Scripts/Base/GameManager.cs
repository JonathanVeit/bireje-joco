using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using JoVei.Base;
using JoVei.Base.MessageSystem;

namespace BiReJeJoCo
{
    public class GameManager : SystemBehaviour, IInitializable
    {
        #region Initialization
        public IEnumerator Initialize(object[] parameters)
        {
            DIContainer.RegisterImplementation<GameManager>(this);
            ConnecteEvents();
            yield return null;
        }

        public void CleanUp() 
        {
            DisconnectEvents();
        }

        private void ConnecteEvents() 
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            messageHub.RegisterReceiver<OnJoinedLobbyMsg> (this, OnJoinedLobby);
        }

        private void DisconnectEvents() 
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            messageHub.UnregisterReceiver(this);
        }
        #endregion

        public void OpenMainMenu() 
        {
            SceneManager.LoadScene("main_menu_scene");
        }

        public void OpenLobby()
        {
            SceneManager.LoadScene("lobby_scene");
        }

        public void Quit()
        {
            Application.Quit();
        }


        #region Events
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name.StartsWith("game_"))
            {
                StartCoroutine(ShoutMessageDelayed<LoadedGameSceneMsg>(new LoadedGameSceneMsg(), 5));
            }
            if (scene.name.StartsWith("lobby_"))
            {
                StartCoroutine(ShoutMessageDelayed<LoadedLobbySceneMsg>(new LoadedLobbySceneMsg(), 5));
            }
        }

        private void OnJoinedLobby(OnJoinedLobbyMsg msg) 
        {
            GameObject matchHandlerPrefab;
            if (localPlayer.IsHost)
                matchHandlerPrefab = MatchPrefabMapping.GetMapping().GetElementForKey("host_match_handler");
            else
                matchHandlerPrefab = MatchPrefabMapping.GetMapping().GetElementForKey("match_handler");

            Instantiate(matchHandlerPrefab);
        }
        #endregion

        #region Helper
        private IEnumerator ShoutMessageDelayed<TMessage>(TMessage message, int frames)
            where TMessage : IMessage
        {
            for (int i = 0; i < frames; i++)
            {
                yield return new WaitForEndOfFrame();
            }

            messageHub.ShoutMessage<TMessage>(this, message);
        }
        #endregion
    }
}