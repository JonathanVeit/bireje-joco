using System;
using System.Collections;
using System.Collections.Generic;
using BiReJeJoCo.Backend;
using UnityEngine;

namespace BiReJeJoCo.Map
{
    public class TriggeredPlattform : SynchronizedTrigger
    {
        [Header("Plattform Settings")]
        [SerializeField] List<PlattformTarget> targets;
        [SerializeField] byte plattformStartIndex;
        [SerializeField] PlattformBoard board;

        protected override void OnSetupActive()
        {
            base.OnSetupActive();
        }

        protected override void OnTriggerInteracted(byte pointId)
        {
            if (!board.ReachedTarget) return;

            board.SetTarget (targets.Find(x => x.triggerPoint == pointId).target);
        }

        protected override IEnumerator CoolDown(TriggerSetup trigger)
        {
            trigger.isCoolingDown = true;
            yield return new WaitUntil(() => board.ReachedTarget);
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
