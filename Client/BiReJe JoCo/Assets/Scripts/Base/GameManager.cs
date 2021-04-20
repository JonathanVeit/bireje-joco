using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using JoVei.Base;

namespace BiReJeJoCo
{
    public class GameManager : SystemBehaviour, IInitializable
    {
        #region Initialization
        public IEnumerator Initialize(object[] parameters)
        {
            DIContainer.RegisterImplementation<GameManager>(this);
            yield return null;
        }

        public void CleanUp() { }
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
    }
}