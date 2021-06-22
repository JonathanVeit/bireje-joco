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
        [SerializeField] ElevatorSign[] signs;
        [SerializeField] ElevatorDoorController doorController;

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
                    doorController.Close(ElevatorDoorController.ElevatorDoorPoint.UpperDoors,
                        () => 
                    {
                        board.SetTarget(lowerPosition.target, () =>
                        {
                            ResetSigns();
                            doorController.Open(ElevatorDoorController.ElevatorDoorPoint.LowerDoors, null);
                        });
                        currentPointIndex = 0;
                        UpdateSigns(false);
                    });

                    break;

                // go up
                case 1:
                    doorController.Close(ElevatorDoorController.ElevatorDoorPoint.LowerDoors, 
                        () =>
                    {
                        board.SetTarget(upperPosition.target, () =>
                        {
                            ResetSigns();
                            doorController.Open(ElevatorDoorController.ElevatorDoorPoint.UpperDoors, null);
                        });
                        currentPointIndex = 1;
                        UpdateSigns(true);
                    });

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
        }

        protected override void OnFloatySpawned(int pointId, InteractionFloaty floaty)
        {
            if (pointId == 0 ||
                pointId == 1)
            {
                floaty.SetDescription("Call Elevator");
                return;
            }

            floaty.SetDescription(currentPointIndex == 0? "Up" : "Down");
        }
        protected override IEnumerator CoolDown(TriggerSetup trigger)
        {
            trigger.isCoolingDown = true;
            TryHideFloaty(trigger);
            yield return new WaitUntil(() => board.ReachedTarget && doorController.DoorsAreOpen);
            TryUnhideFloaty(trigger);
            trigger.isCoolingDown = false;

            if (trigger.Id == 2 &&
                floaties.ContainsKey(trigger.Id) &&
                floaties[trigger.Id] != null)
                floaties[trigger.Id].SetDescription(currentPointIndex == 0 ? "Up" : "Down");
        }


        #region Helper
        [Serializable]
        public struct PlattformTarget
        {
            public Transform target;
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

        protected override bool PlayerIsInArea(TriggerSetup trigger)
        {
            if (trigger.Id == currentPointIndex) return false;
            return base.PlayerIsInArea(trigger);
        }
        #endregion
    }
}
