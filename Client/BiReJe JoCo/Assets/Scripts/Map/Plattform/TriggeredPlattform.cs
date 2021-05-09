using System;
using System.Collections;
using System.Collections.Generic;
using BiReJeJoCo.Backend;
using BiReJeJoCo.UI;
using UnityEngine;

namespace BiReJeJoCo.Map
{
    public class TriggeredPlattform : SynchronizedTrigger
    {
        [Header("Plattform Settings")]
        [SerializeField] List<PlattformTarget> targets;
        [SerializeField] byte plattformStartIndex;
        [SerializeField] PlattformBoard board;

        protected override void SetupAsActive()
        {
            base.SetupAsActive();
        }

        protected override void OnTriggerInteracted(byte pointId)
        {
            if (!board.ReachedTarget) return;

            board.SetTarget (targets.Find(x => x.triggerPoint == pointId).target);
        }

        protected override void OnFloatySpawned(int pointId, FloatingElement floaty)
        {
            (floaty as InteractionFloaty).Initialize("Elevator");
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
            public byte triggerPoint;
        }
        #endregion
    }
}
