using UnityEngine;
using System.Collections.Generic;

namespace BiReJeJoCo.Backend
{
    public abstract class SynchronizedTrigger : BaseTrigger
    {
        [Header("Synchronized Trigger Setting")]
        [SerializeField] protected byte triggerId;
        [SerializeField] PhotonMessageTarget messageTarget;

        private static Dictionary<byte, SynchronizedTrigger> instances
            = new Dictionary<byte, SynchronizedTrigger>();


        #region Initialization
        protected override void SetupAsActive()
        {
            photonMessageHub.RegisterReceiver<TriggerPointInteractedPhoMsg>(this, OnSychronizedTriggerReceived);

            if (instances.ContainsKey(triggerId))
            {
                throw new System.ArgumentException($"There is already SynchronizedTrigger for id {triggerId}.");
            }

            instances.Add(triggerId, this);
        }

        protected override void OnBeforeDestroy()
        {
            if (photonMessageHub)
                photonMessageHub.UnregisterReceiver(this);
            instances.Remove(triggerId);
            base.OnBeforeDestroy();
        }

        public static SynchronizedTrigger Find(byte triggerId)
        {
            if (instances.ContainsKey(triggerId)) return instances[triggerId];
            return null;
        }
        #endregion

        protected sealed override void OnTriggerPressed()
        {
            photonMessageHub.ShoutMessage(new TriggerPointInteractedPhoMsg(triggerId, DisplayedTrigger.Id, localPlayer.NumberInRoom), messageTarget);
            DisplayedTrigger.isCoolingDown = true;
            ResetActiveInstance();
        }
        protected sealed override void OnTriggerHold(float duration)
        {
            if (DisplayedTrigger.pressDuration <= duration)
            {
                photonMessageHub.ShoutMessage(new TriggerPointInteractedPhoMsg(triggerId, DisplayedTrigger.Id, localPlayer.NumberInRoom), messageTarget);
                DisplayedTrigger.isCoolingDown = true;

                UpdateTriggerProgress(DisplayedTrigger, 0);
                ResetActiveInstance();
            }
            else
            {
                UpdateTriggerProgress(DisplayedTrigger, duration);
            }
        }
        protected sealed override void OnTriggerReleased()
        {
            foreach (var curTrigger in triggerPoints)
            {
                UpdateTriggerProgress(curTrigger, 0);
            }
        }

        protected virtual void OnSychronizedTriggerReceived(PhotonMessage msg)
        {
            var castedMsg = msg as TriggerPointInteractedPhoMsg;

            if (castedMsg.i == triggerId)
            {
                OnTriggerInteracted(castedMsg.ti);
                StartCoroutine(CoolDown(triggerPoints.Find(x => x.Id == castedMsg.ti)));
            }
        }

        protected abstract override void OnTriggerInteracted(byte pointId);

        #region Access
        public void SetTriggerId(byte id) 
        {
            triggerId = id;
        }
        #endregion
    }
}