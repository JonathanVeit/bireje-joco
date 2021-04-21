using UnityEngine;
using Photon.Pun;

namespace BiReJeJoCo.Backend
{
    [RequireComponent(typeof(PhotonView))]
    public class PlayerControlled : SystemBehaviour
    {
        public PhotonView PhotonView { get; private set; }
        public Player Player { get; private set; }

        protected override void OnSystemsInitialized()
        {
            PhotonView = GetComponent<PhotonView>();
            Player = playerManager.GetPlayer(PhotonView.Controller.UserId);
        }
    }
}