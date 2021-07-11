using System.Collections;
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
            messageHub.RegisterReceiver<JoinedLobbyMsg> (this, OnJoinedLobby);
            messageHub.RegisterReceiver<LeftLobbyMsg>(this, OnLeftlobby);
        }

        private void DisconnectEvents() 
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            messageHub.UnregisterReceiver(this);
        }
        #endregion

        public void OpenMainMenu() 
        {
            SceneManager.LoadSceneAsync("main_menu_scene");
        }

        public void OpenOptions()
        {
            SceneManager.LoadSceneAsync("option_scene");
        }

        public void OpenLobby()
        {
            SceneManager.LoadSceneAsync("lobby_scene");
        }

        public void Quit()
        {
            Application.Quit();
        }


        #region Events
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name.StartsWith("main_"))
            {
                StartCoroutine(ShoutMessageDelayed<LoadedMainSceneMsg>(new LoadedMainSceneMsg(), 5));
            }
            if (scene.name.StartsWith("game_"))
            {
                StartCoroutine(ShoutMessageDelayed<LoadedGameSceneMsg>(new LoadedGameSceneMsg(), 5));
            }
            if (scene.name.StartsWith("lobby_"))
            {
                StartCoroutine(ShoutMessageDelayed<LoadedLobbySceneMsg>(new LoadedLobbySceneMsg(), 5));
            }
        }

        private void OnJoinedLobby(JoinedLobbyMsg msg) 
        {
            GameObject matchHandlerPrefab;
            if (localPlayer.IsHost)
                matchHandlerPrefab = MatchPrefabMapping.GetMapping().GetElementForKey("host_match_handler");
            else
                matchHandlerPrefab = MatchPrefabMapping.GetMapping().GetElementForKey("match_handler");

            Instantiate(matchHandlerPrefab);
        }
        private void OnLeftlobby(LeftLobbyMsg msg)
        {
            Destroy(matchHandler.gameObject);
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