using System.Collections.Generic;
using UnityEngine;

namespace BiReJeJoCo.Sound
{

    [RequireComponent(typeof(AnimationEventCatcher))]
    public class AmbientSoundManager : TickBehaviour
    {
        [Header("Settings")]
        [SerializeField] List<SourceMapping> audioSources;
        [SerializeField] float transitionThreshold;
        private Transform PlayerTransform => playerManager.LocalPlayer.PlayerCharacter? playerManager.LocalPlayer.PlayerCharacter.ControllerSetup.CharacterRoot.transform : null;
        
        protected override void OnSystemsInitialized()
        {
            GetComponent<AnimationEventCatcher>().onAnimationEventTriggered += OnAnimationEventTriggered;
        }

        private void OnAnimationEventTriggered(string obj)
        {
            throw new System.NotImplementedException();
        }

        public override void Tick(float deltaTime)
        {
            if (PlayerTransform == null)
                return;

            var yPos = PlayerTransform.position.y;
            var previousHeight = int.MaxValue;

            foreach (var mappedSource in audioSources)
            {
                if (mappedSource.height >= yPos &&
                    mappedSource.height < previousHeight - transitionThreshold)
                {
                    mappedSource.audioSource.volume = 1;
                }
                else if (yPos >= mappedSource.height - transitionThreshold)
                {

                }

                previousHeight = mappedSource.height;
            }
        }

        [System.Serializable]
        private struct SourceMapping 
        {
            public AudioSource audioSource;
            public int height;
        }
    }
}