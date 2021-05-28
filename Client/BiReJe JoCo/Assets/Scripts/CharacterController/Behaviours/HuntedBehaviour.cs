using BiReJeJoCo.Backend;
using BiReJeJoCo.UI;
using JoVei.Base;
using JoVei.Base.Helper;
using System;
using System.Linq;
using UnityEngine;

namespace BiReJeJoCo.Character
{
    public class HuntedBehaviour : BaseBehaviour, ITickable
    {
        [Header("Mechanics")]
        [SerializeField] TransformationMechanic transformationMechanic;
        [SerializeField] SpeedUpMechanic speedUpMechanic;
        [SerializeField] ResistanceMechanic resistanceMechanic;

        GameUI gameUI => uiManager.GetInstanceOf<GameUI>();
        private int collectedItems;

        #region Initialization
        protected override void OnBehaviourInitialized()
        {
            gameUI.UpdateHealthBar(1);

            if (Owner.IsLocalPlayer)
            {
                ConnectEvents();
            }
        }
        protected override void OnBeforeDestroy()
        {
            base.OnBeforeDestroy();
            DisconnectEvents();
        }

        void ConnectEvents()
        {
            tickSystem.Register(this, "update");
            messageHub.RegisterReceiver<PlayerCharacterSpawnedMsg>(this, OnPlayerCharacterSpawned);
            messageHub.RegisterReceiver<ItemCollectedByPlayerMsg>(this, OnItemCollected);
        }
        void DisconnectEvents()
        {
            tickSystem.Unregister(this);
            messageHub.UnregisterReceiver(this);
            if (photonMessageHub)
                photonMessageHub.UnregisterReceiver(this);
        }
        #endregion

        #region Mechanics
        private void OnShootPressed()
        {
            transformationMechanic.ToggleTransformation();
        }

        private void OnSpeedUpPressed()
        {
            speedUpMechanic.UseSpeed();
        }

        public void Tick(float deltaTime)
        {
            resistanceMechanic.UpdateResistance();
        }

        #endregion

        #region Events
        void OnPlayerCharacterSpawned(PlayerCharacterSpawnedMsg msg)
        {
            localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onShootPressed += OnShootPressed;
            localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onSpecial1Pressed += OnSpeedUpPressed;
        }

        void OnItemCollected(ItemCollectedByPlayerMsg msg)
        {
            collectedItems++;
            gameUI.UpdateCollectedItemAmount(collectedItems);

            if (collectedItems >= matchHandler.MatchConfig.Mode.huntedCollectables)
            {
                photonMessageHub.ShoutMessage<HuntedFinishedObjectivePhoMsg>(PhotonMessageTarget.MasterClient);
            }
        }
        #endregion
    }
}