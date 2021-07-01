using System.Collections.Generic;
using UnityEngine;

namespace BiReJeJoCo.Audio
{
    [RequireComponent(typeof(AnimationEventCatcher))]
    public class AnimationSoundTrigger : SystemBehaviour
    {
        [Header("Settings")]
        [SerializeField] List<SoundEffectMapping> effects;

        private Dictionary<string, List<AudioSourceHandler>> observedEffects
            = new Dictionary<string, List<AudioSourceHandler>>();

        protected override void OnSystemsInitialized()
        {
            GetComponent<AnimationEventCatcher>().onAnimationEventTriggered += OnAnimationEventTriggered;
        }

        private void OnAnimationEventTriggered(string trigger)
        {
            CheckObservedEffects(trigger);

            foreach (SoundEffectMapping mappedEffect in effects.FindAll(x => x.trigger == trigger))
            {
                if (mappedEffect.target == null)
                {
                    var audioSource = soundEffectManager.Play(mappedEffect.soundEffects[Random.Range(0, mappedEffect.soundEffects.Length)]);
                    ObserveSource(mappedEffect, audioSource);
                    continue;
                }

                if (mappedEffect.parent)
                {
                    var audioSource = soundEffectManager.Play(mappedEffect.soundEffects[Random.Range(0, mappedEffect.soundEffects.Length)], mappedEffect.target);
                    ObserveSource(mappedEffect, audioSource);
                }
                else
                {
                    var audioSource = soundEffectManager.Play(mappedEffect.soundEffects[Random.Range(0, mappedEffect.soundEffects.Length)], mappedEffect.target.position);
                    ObserveSource(mappedEffect, audioSource);
                }
            }
        }
        private void ObserveSource(SoundEffectMapping effect, AudioSourceHandler audioSource)
        {
            if (string.IsNullOrEmpty(effect.stopTrigger))
                return;

            if (!observedEffects.ContainsKey(effect.stopTrigger))
                observedEffects.Add(effect.stopTrigger, new List<AudioSourceHandler>());

            observedEffects[effect.stopTrigger].Add(audioSource);
        }

        private void CheckObservedEffects(string trigger) 
        {
            if (!observedEffects.ContainsKey(trigger))
                return;

            foreach (var curHandler in observedEffects[trigger])
            {
                curHandler.AudioSource.Stop();
                curHandler.RequestReturnToPool();
            }

            observedEffects.Remove(trigger);
        }

        [System.Serializable]
        private struct SoundEffectMapping 
        {
            public string trigger;
            public string stopTrigger;
            public Transform target;
            public bool parent;
            public string[] soundEffects;
        }
    }
}