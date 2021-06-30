using BiReJeJoCo.Character;
using System.Collections.Generic;
using UnityEngine;

namespace BiReJeJoCo.Sound
{

    [RequireComponent(typeof(AnimationEventCatcher))]
    public class AnimationSoundTrigger : SystemBehaviour
    {
        [Header("Settings")]
        [SerializeField] List<SoundEffectMapping> effects;

        protected override void OnSystemsInitialized()
        {
            GetComponent<AnimationEventCatcher>().onAnimationEventTriggered += OnAnimationEventTriggered;
        }

        private void OnAnimationEventTriggered(string trigger)
        {
            foreach (SoundEffectMapping mappedEffect in effects.FindAll(x => x.trigger == trigger))
            {
                if (mappedEffect.target == null)
                {
                    soundEffectManager.Play(mappedEffect.soundEffects[Random.Range(0, mappedEffect.soundEffects.Length)]);
                    continue;
                }

                if (mappedEffect.parent)
                    soundEffectManager.Play(mappedEffect.soundEffects[Random.Range(0, mappedEffect.soundEffects.Length)], mappedEffect.target);
                else
                    soundEffectManager.Play(mappedEffect.soundEffects[Random.Range(0, mappedEffect.soundEffects.Length)], mappedEffect.target.position);
            }
        }

        [System.Serializable]
        private struct SoundEffectMapping 
        {
            public string trigger;
            public Transform target;
            public bool parent;
            public string[] soundEffects;
        }
    }
}