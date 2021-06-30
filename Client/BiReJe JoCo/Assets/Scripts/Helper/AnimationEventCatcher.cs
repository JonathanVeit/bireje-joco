using System;
using UnityEngine;


namespace BiReJeJoCo
{
    public class AnimationEventCatcher : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] float minAnimationWeight = 0.5f;

        public event Action<string> onAnimationEventTriggered;

        public void OnAnimationEvent(AnimationEvent args)
        {
            if (args.animatorClipInfo.weight >= minAnimationWeight)
                onAnimationEventTriggered?.Invoke(args.stringParameter);
        }
    }
}