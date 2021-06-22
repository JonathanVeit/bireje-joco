using System;
using System.Collections;
using UnityEngine;

namespace BiReJeJoCo.Map
{
    public class ElevatorDoorController : SystemBehaviour
    {
        [Header("Settings")]
        [SerializeField] Animator[] animators;

        public bool DoorsAreOpen { get; private set; }

        public void Open(Action onFinishCallback) 
        {
            TriggerAnimators("Open");
            StartCoroutine(AwaitAnimations(() => 
            {
                Debug.Log("Animations Finsihed!");
                DoorsAreOpen = true;
                onFinishCallback?.Invoke(); 
            }));
        }
        public void Close(Action onFinishCallback) 
        {
            DoorsAreOpen = false;
            TriggerAnimators("Close");
            StartCoroutine(AwaitAnimations(() =>
            {
                Debug.Log("Animations Finsihed!");
                onFinishCallback?.Invoke();
            }));
        }

        private void TriggerAnimators(string trigger)
        {
            foreach (var anim in animators)
            {
                anim.ResetTrigger(trigger);
                anim.SetTrigger(trigger);
            }
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
            foreach (var anim in animators)
            {
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 ||
                    anim.IsInTransition(0))
                {
                    return false;
                }
            }

            return true;
        }
    }
}