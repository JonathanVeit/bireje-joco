using BiReJeJoCo.UI;
using JoVei.Base.Helper;
using System;
using UnityEngine;

namespace BiReJeJoCo.Character
{
    public class SpeedUpMechanic : BaseBehaviourMechanic<HuntedBehaviour>
    {
        [Header("Settings")]
        [SerializeField] float speedUpMultiplier = 1.2f;
        [SerializeField] Timer speedUpDurationTimer;
        [SerializeField] Timer speedUpCooldownTimer;

        #region Initialization
        protected override void OnInitializeLocal()
        {
            speedUpCooldownTimer.Start(() => // update 
            {
                gameUI.UpdateSpeedUpBar(speedUpCooldownTimer.RelativeProgress);
            }, null);

            ConnectEvents();
        }
        protected override void OnInitializeRemote()
        {
        }
        protected override void OnBeforeDestroy()
        {
            speedUpDurationTimer.Stop();
            speedUpCooldownTimer.Stop();

            DisconnectEvents();
        }

        private void ConnectEvents()
        {
        }
        private void DisconnectEvents()
        {
        }
        #endregion

        public void UseSpeed()
        {
            if (speedUpCooldownTimer.State != TimerState.Finished ||
                speedUpDurationTimer.State == TimerState.Counting) return;

            var multiplier = new MovementMultiplier(speedUpMultiplier);
            localPlayer.PlayerCharacter.ControllerSetup.WalkController.AddMultiplier(multiplier);

            speedUpDurationTimer.Start(
                () => // update 
                {
                    gameUI.UpdateSpeedUpBar(1 - speedUpDurationTimer.RelativeProgress);
                },
                () => // finish
                {
                    localPlayer.PlayerCharacter.ControllerSetup.WalkController.RemoveMultiplier(multiplier);

                    speedUpCooldownTimer.Start(() => // update 
                    {
                        gameUI.UpdateSpeedUpBar(speedUpCooldownTimer.RelativeProgress);
                    }, null);
                });
        }
    }
}