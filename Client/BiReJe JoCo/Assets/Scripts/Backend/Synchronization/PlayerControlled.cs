using UnityEngine;
using Photon.Pun;
using BiReJeJoCo.UI;
using JoVei.Base;
using JoVei.Base.UI;

namespace BiReJeJoCo.Backend
{
    [RequireComponent(typeof(PhotonView))]
    public class PlayerControlled : SystemBehaviour
    {
        public PhotonView PhotonView { get; private set; }
        public Player Player { get; private set; }

        private void Awake()
        {
            PhotonView = GetComponent<PhotonView>();
            Player = playerManager.GetPlayer(PhotonView.Controller.UserId);

            if (!Player.IsLocalPlayer)
                SpawnFloaty();
        }


        /// TODO: move somewhere
        [SerializeField] Transform floatingElementTarget;
        private PlayerNameFloaty nameFloaty;

        private void SpawnFloaty()
        {
            var config = new FloatingElementConfig("player_character_name", DIContainer.GetImplementationFor<GameUI> ().floatingElementGrid, floatingElementTarget);
            nameFloaty = floatingManager.GetElementAs<PlayerNameFloaty>(config);
            nameFloaty.Initialize(Player.NickName);
        }

        protected override void OnBeforeDestroy()
        {
            // TODO: should be called before destroy 
            floatingManager.DestroyElement(nameFloaty);
        }
    }
}