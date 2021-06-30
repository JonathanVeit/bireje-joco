using System;
using System.Collections.Generic;
using UnityEngine;


namespace BiReJeJoCo
{
    public class AnimationEventCatcher : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] float minAnimationWeight = 0.5f;
        [SerializeField] List<OverrideWeight> overrideAnimationWeight;

        public event Action<string> onAnimationEventTriggered;

        public void OnAnimationEvent(AnimationEvent args)
        {
            var overrides = overrideAnimationWeight.FindAll(x => x.eventName == args.stringParameter);
            if (overrides.Count > 0 &&
                args.animatorClipInfo.weight >= overrides[0].weight)
            {
                onAnimationEventTriggered?.Invoke(args.stringParameter);
                return;
            }

            if (args.animatorClipInfo.weight >= minAnimationWeight)
                onAnimationEventTriggered?.Invoke(args.stringParameter);
        }

        [System.Serializable]
        public struct OverrideWeight
        {
            public string eventName;
            public float weight;
        }
    }
}