using System;
using UnityEngine;


namespace BiReJeJoCo.Character
{
    public class AnimationEventCatcher : MonoBehaviour
    {
        public event Action<string> onAnimationEventTriggered;

        public void OnAnimationEvent(string args)
        {
            Debug.Log(args);
            onAnimationEventTriggered?.Invoke(args);
        }
    }
}