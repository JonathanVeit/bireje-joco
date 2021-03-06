using System;
using System.Collections;
using UnityEngine;

namespace BiReJeJoCo.Map
{
    public class ElevatorDoorController : SystemBehaviour
    {
        public enum ElevatorDoorPoint
        {
            UpperDoors = 0,
            LowerDoors = 1,
        }

        [Header("Settings")]
        [SerializeField] Animator[] plattformAnim;
        [SerializeField] Animator[] upperAnim;
        [SerializeField] Animator[] lowerAnim;

        public bool DoorsAreOpen { get; private set; }
        public bool DoorsAreMoving { get; private set; }

        public void Open(ElevatorDoorPoint point, Action onFinishCallback) 
        {
            DoorsAreMoving = true;
            ResetAllTrigger("Close");
            TriggerAnimators(point, "Open");
            StartCoroutine(AwaitAnimations(() => 
            {
                DoorsAreOpen = true;
                onFinishCallback?.Invoke();
                DoorsAreMoving = false;
            }));
        }
        public void Close(ElevatorDoorPoint point, Action onFinishCallback) 
        {
            DoorsAreMoving = true;
            DoorsAreOpen = false;
            ResetAllTrigger("Open");
            TriggerAnimators(point, "Close");
            StartCoroutine(AwaitAnimations(() =>
            {
                onFinishCallback?.Invoke();
                DoorsAreMoving = false;
            }));
        }

        private void TriggerAnimators(ElevatorDoorPoint point, string trigger)
        {
            TriggerAnimtors(plattformAnim, trigger);

            switch (point)
            {
                case ElevatorDoorPoint.UpperDoors:
                    TriggerAnimtors(upperAnim, trigger);
                    break;
                case ElevatorDoorPoint.LowerDoors:
                    TriggerAnimtors(lowerAnim, trigger);
                    break;
            }
        }
        private void TriggerAnimtors(Animator[] anim, string trigger)
        {
            foreach (var curAnim in anim)
            {
                curAnim.ResetTrigger(trigger);
                curAnim.SetTrigger(trigger);
            }
        }
        private void ResetAllTrigger(string trigger)
        {
            foreach (var curAnim in plattformAnim)
                curAnim.ResetTrigger(trigger);
            foreach (var curAnim in upperAnim)
                curAnim.ResetTrigger(trigger);
            foreach (var curAnim in lowerAnim)
                curAnim.ResetTrigger(trigger);
        }

        private IEnumerator AwaitAnimations(Action callback)
        {
            var waiter = new WaitForEndOfFrame();
            var delay = new WaitForSeconds(0.5f);

            yield return delay;
            while (true)
            {
                if (AnimatorsFinished())
                {
                    callback?.Invoke();
                    break;
                }

                yield return waiter;
            }
        }
        private bool AnimatorsFinished()
        {
            return AnimatorsAreFinished(plattformAnim) && AnimatorsAreFinished(upperAnim) && AnimatorsAreFinished(lowerAnim);
        }
        private bool AnimatorsAreFinished(Animator[] anim) 
        {
            foreach (var curAnim in anim)
            {
                if (curAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 ||
                    curAnim.IsInTransition(0))
                {
                    return false;
                }
            }

            return true;
        }
    }
}