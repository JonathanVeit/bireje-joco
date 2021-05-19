using System;
using System.Collections;
using BiReJeJoCo.Backend;
using BiReJeJoCo.UI;
using UnityEngine;

namespace BiReJeJoCo.Map
{
    public class TriggeredElevator : SynchronizedTrigger
    {
        [Header("Elevator Settings")]
        [SerializeField] PlattformTarget lowerPosition;
        [SerializeField] PlattformTarget upperPosition;
        [SerializeField] [Range (0, 1)] byte startPointIndex;
        [SerializeField] ElevatorPlattform board;
        [SerializeField] new Collider[] collider;

        private byte currentPointIndex;

        protected override void SetupAsActive()
        {
            base.SetupAsActive();

            OnTriggerInteracted(startPointIndex);
        }

        protected override void OnTriggerInteracted(byte pointId)
        {
            if (!board.ReachedTarget) return;

            switch (pointId)
            {
                // go down 
                case 0:
                    board.SetTarget(lowerPosition.target, () => UnblockEntries());
                    currentPointIndex = 0;
                    break;

                // go up
                case 1:
                    board.SetTarget(upperPosition.target, () => UnblockEntries());
                    currentPointIndex = 1;
                    break;

                // toggle on plattform 
                case 2:
                    if (currentPointIndex == 0)
                    {
                        OnTriggerInteracted(1);
                        return;
                    }
                    else
                    {
                        OnTriggerInteracted(0);
                        return;
                    }
            }

            BlockEntries();
        }

        protected override void OnFloatySpawned(int pointId, InteractionFloaty floaty)
        {
            floaty.SetDescription("Elevator");
        }
        protected override IEnumerator CoolDown(TriggerSetup trigger)
        {
            trigger.isCoolingDown = true;
            TryHideFloaty(trigger);
            yield return new WaitUntil(() => board.ReachedTarget);
            TryUnhideFloaty(trigger);
            trigger.isCoolingDown = false;
        }

        #region Helper
        [Serializable]
        public struct PlattformTarget
        {
            public Transform target;
        }

        private void BlockEntries() 
        {
            foreach (var curCollider in collider)
                curCollider.enabled = true;
        }

        private void UnblockEntries() 
        {
            foreach (var curCollider in collider)
                curCollider.enabled = false;
        }
        #endregion
    }
}
