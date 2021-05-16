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
            foreach (var curTrigger in triggerPoints)
            {
                if (curTrigger.pressDuration == 0 &&
                    PlayerIsInArea(curTrigger) && 
                    !curTrigger.isCoolingDown)
                {
                    photonMessageHub.ShoutMessage(new TriggerPointInteractedPhoMsg(triggerId, curTrigger.Id, localPlayer.NumberInRoom), messageTarget);
                    curTrigger.isCoolingDown = true;
                }
            }
        }
        protected sealed override void OnTriggerHold(float duration)
        {
            foreach (var curTrigger in triggerPoints)
            {
                if (PlayerIsInArea(curTrigger) &&
                    !curTrigger.isCoolingDown)
                {
                    if (curTrigger.pressDuration <= duration)
                    {
                        photonMessageHub.ShoutMessage(new TriggerPointInteractedPhoMsg(triggerId, curTrigger.Id, localPlayer.NumberInRoom), messageTarget);
                        curTrigger.isCoolingDown = true;
                        floaties[curTrigger.Id].UpdateProgress(0);
                    }
                    else
                    {
                        floaties[curTrigger.Id].UpdateProgress(duration / curTrigger.pressDuration);
                    }
                }
            }
        }

        protected virtual void OnSychronizedTriggerReceived(PhotonMessage msg)
        {
            var castedMsg = msg as TriggerPointInteractedPhoMsg;

            if (castedMsg.i == triggerId)
            {
                OnTriggerInteracted(castedMsg.pi);
                StartCoroutine(CoolDown(triggerPoints.Find(x => x.Id == castedMsg.pi)));
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
