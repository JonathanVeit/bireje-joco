using JoVei.Base.Helper;
using UnityEngine;

namespace BiReJeJoCo.Character
{
    public class TrapMechanic : BaseBehaviourMechanic<HunterBehaviour>
    {
        [Header("Settings")]
        [SerializeField] Transform trapSpawnPoint;
        [SerializeField] LayerMask targetLayer;
        [SerializeField] Timer coolDownTimer;
        [SerializeField] float throwForce;
        [SerializeField] Vector3 extraThrowForce;
        [SerializeField] float trapThrowRange;
        [SerializeField] float trapTorque;

        private GameObject thrownTrap;

        #region Initialization
        protected override void OnInitializeLocal()
        {
            ConnectEvents();
        }
        protected override void OnInitializeRemote()
        {
        }
        protected override void OnBeforeDestroy()
        {
            DisconnectEvents();
        }

        private void ConnectEvents()
        {
            messageHub.RegisterReceiver<PlayerCollectedTrapMsg>(this, OnTrapCollected);
        }
        private void DisconnectEvents() 
        {
            messageHub.UnregisterReceiver(this);
            coolDownTimer.Stop();
        }
        #endregion

        public void ThrowTrap()
        {
            if (coolDownTimer.State == TimerState.Counting) return;

            var ray = new Ray()
            {
                origin = Camera.main.transform.position,
                direction = Camera.main.transform.forward,
            };

            var trapTarget = CalculateTrapTarget(ray);
            thrownTrap = photonRoomWrapper.Instantiate("hunter_trap", trapSpawnPoint.position, trapSpawnPoint.rotation);

            var force = (trapTarget - trapSpawnPoint.position) * throwForce + extraThrowForce;
            thrownTrap.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
            thrownTrap.GetComponent<Rigidbody>().AddTorque(thrownTrap.transform.up * trapTorque);

            coolDownTimer.Start(
                () => 
                {
                    gameUI.UpdateTrapIcon(coolDownTimer.RelativeProgress);
                }, // update
                () =>
                {
                    photonRoomWrapper.Destroy(thrownTrap);
                    thrownTrap = null;
                }); // finish
        }
        private Vector3 CalculateTrapTarget(Ray ray)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, trapThrowRange, targetLayer, QueryTriggerInteraction.Ignore))
            {
                return hit.point;
            }

            return Camera.main.transform.position + Camera.main.transform.forward * trapThrowRange;
        }

        private void OnTrapCollected(PlayerCollectedTrapMsg msg)
        {
            if (thrownTrap == null) return;
            gameUI.UpdateTrapIcon(1);
            coolDownTimer.Stop(true);

        }
    }
}