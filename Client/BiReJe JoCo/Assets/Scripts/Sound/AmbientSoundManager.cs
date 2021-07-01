using UnityEngine;
using UnityEngine.Audio;

namespace BiReJeJoCo.Audio
{
    public class AmbientSoundManager : TickBehaviour
    {
        [Header("Settings")]
        [SerializeField] GroupMapping[] groups;
        [SerializeField] AudioMixer mixer;
        [SerializeField] AnimationCurve blendCurve;
        [SerializeField] float blendSpeed;

        private Transform PlayerRoot;
        private float posY;

        protected override void OnSystemsInitialized()
        {
            base.OnSystemsInitialized();
            messageHub.RegisterReceiver<PlayerCharacterSpawnedMsg>(this, OnPlayerCharacterSpawned);

            foreach (var mapping in groups)
            {
                mixer.SetFloat(mapping.audioGroup, -80);
            }
        }
        protected override void OnBeforeDestroy()
        {
            base.OnBeforeDestroy();
            messageHub.UnregisterReceiver<PlayerCharacterSpawnedMsg>(this, OnPlayerCharacterSpawned);
        }

        private void OnPlayerCharacterSpawned(PlayerCharacterSpawnedMsg msg)
        {
            PlayerRoot = localPlayer.PlayerCharacter.ControllerSetup.ModelRoot;
            posY = PlayerRoot.transform.position.y;
        }
   
        public override void Tick(float deltaTime)
        {
            if (PlayerRoot == null)
                return;

            posY = Mathf.Lerp(posY, PlayerRoot.position.y, blendSpeed * deltaTime);
            foreach (var mapping in groups)
            {
                var delta = Mathf.InverseLerp(mapping.minHeight, mapping.maxHeight, posY); 
                var volumeDelta = 1 - blendCurve.Evaluate(delta);
                var value = -80f * volumeDelta;

                mixer.SetFloat(mapping.audioGroup, value);
            }
        }

        [System.Serializable]
        private struct GroupMapping 
        {
            public string name;
            public int minHeight;
            public int maxHeight;
            public string audioGroup;
        }
    }
}