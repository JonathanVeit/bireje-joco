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
        [SerializeField] GameObject lowerEntry;
        [SerializeField] GameObject upperEntry;
        [SerializeField] ElevatorSign[] signs;

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
                    board.SetTarget(lowerPosition.target, () =>
                    {
                        SetLowerEntry(true);
                        ResetSigns();
                    });
                    currentPointIndex = 0;
                    UpdateSigns(false);
                    break;

                // go up
                case 1:
                    board.SetTarget(upperPosition.target, () =>
                    {
                        SetUpperEntry(true);
                        ResetSigns();
                    });
                    currentPointIndex = 1;
                    UpdateSigns(true);
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

            SetLowerEntry(false);
            SetUpperEntry(false);
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

        private void SetLowerEntry(bool open)
        {
            lowerEntry.SetActive(!open);
        }
        private void SetUpperEntry(bool open) 
        {
            upperEntry.SetActive(!open);
        }

        private void UpdateSigns(bool up) 
        {
            foreach (var sign in signs)
                sign.OnElevatorTargetSet(up);
        }
        private void ResetSigns()
        {
            foreach (var sign in signs)
                sign.Reset();
        }
        #endregion
    }
}
