using UnityEngine;

namespace BiReJeJoCo.Backend
{
    public abstract class SynchronizedTrigger : BaseTrigger
    {
        [Header("Synchronized Trigger Setting")]
        [SerializeField] int triggerId;
        [SerializeField] PhotonMessageTarget messageTarget;

        #region Initialization
        protected override void OnSetupActive()
        {
            photonMessageHub.RegisterReceiver<OnSynchronizedTriggerPhoMsg>(this, OnSychronizedTriggerReceived);
        }

        protected override void OnBeforeDestroy()
        {
            base.OnBeforeDestroy();
            photonMessageHub.UnregisterReceiver(this);
        }
        #endregion

        protected sealed override void OnPressedTrigger(OnPlayerPressedTriggerMsg msg)
        {
            if (PlayerIsInRange() && !isCoolDown)
            {
                photonMessageHub.ShoutMessage(new OnSynchronizedTriggerPhoMsg(triggerId, localPlayer.NumberInRoom), messageTarget);
                isCoolDown = true;
            }
        }
        private void OnSychronizedTriggerReceived(PhotonMessage msg) 
        {
            var castedMsg = msg as OnSynchronizedTriggerPhoMsg;

            if (castedMsg.i == triggerId)
            {
                OnTriggerInteracted();
                StartCoroutine(CoolDown());
            }
        }

        protected abstract override void OnTriggerInteracted();
    }
}
