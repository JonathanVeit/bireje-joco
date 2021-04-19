using UnityEngine;
using UnityEngine.UI;
using JoVei.Base.UI;

namespace BiReJeJoCo.UI
{
    public class MainMenuUI : UIElement
    {
        [SerializeField] GameObject loadingOverlay;
        [SerializeField] InputField playerNickNameInput;
        [SerializeField] InputField roomNameInput;

        protected override void OnSystemsInitialized()
        {
            photonRoomWrapper.onJoinedRoom += OnRoomJoined;
            photonRoomWrapper.onJoinRoomFailed += OnJoinRoomFailed;

            playerNickNameInput.text = System.Guid.NewGuid().ToString();
            photonConnectionWrapper.NickName = playerNickNameInput.text;
        }

        protected override void OnBeforeDestroy()
        {
            photonRoomWrapper.onJoinedRoom -= OnRoomJoined;
            photonRoomWrapper.onJoinRoomFailed -= OnJoinRoomFailed;
        }

        private void OnRoomJoined(string roomName)
        {
            gameManager.OpenRoomMenu();
            loadingOverlay.gameObject.SetActive(false);
        }

        private void OnJoinRoomFailed(string reason)
        {
            loadingOverlay.gameObject.SetActive(false);
        }

        #region UI Inputs
        public void SetNickName(string name) 
        {
            photonConnectionWrapper.NickName = name;
        }

        public void HostRoom()
        {
            if (string.IsNullOrEmpty(roomNameInput.text)) return;
            photonClient.HostRoom(roomNameInput.text, 3);
            loadingOverlay.gameObject.SetActive(true);
        }

        public void JoinRoom() 
        {
            if (string.IsNullOrEmpty(roomNameInput.text)) return;

            photonClient.JoinRoom(roomNameInput.text);
            loadingOverlay.gameObject.SetActive(true);
        }

        public void QuitGame() 
        {
            gameManager.Quit();
        }
        #endregion
    }
}