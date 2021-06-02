using BiReJeJoCo.Backend;
using BiReJeJoCo.UI;
using UnityEngine;
using JoVei.Base.Helper;
using System.Collections;
using BiReJeJoCo.Character;
using JoVei.Base.UI;

namespace BiReJeJoCo.Items
{
    public class HunterTrap : LocalTrigger, IPlayerObserved
    {
        [Header("Trap Settings")]
        public Rigidbody rigidBody;
        [SerializeField] Timer startDelay;
        [SerializeField] Timer catchDuration;
        [SerializeField] Transform centerPoint;
        [SerializeField] LayerMask huntedLayer;
        [SerializeField] float showFloatyAt;

        [Header("Catch Settings")]
        [SerializeField] Vector3 catchArea;
        [SerializeField] Vector3 catchAreaOffset;

        [Header("Suction")]
        [SerializeField] Vector3 suctionArea;
        [SerializeField] Vector3 suctionAreaOffset;
        [SerializeField] float suctionStrength;
        [SerializeField] float minSuctionDistance;

        [Header("SFX")]
        [SerializeField] Light light;
        [SerializeField] float lightSpeed;
        [SerializeField] ParticleSystem[] particleSystems;
        
        private bool isBlocked = true;
        private float lightIntensity;
        private FloatingElement locationFloaty;
        private Coroutine setLightCoroutine;

        public Player Owner => controller.Player;
        private PlayerControlled controller;

        #region Initialization
        private void Awake()
        {
            lightIntensity = light.intensity;
        }

        public void Initialize(PlayerControlled controller)
        {
            this.controller = controller;
            light.intensity = 0;
            SetSFX(false);

            startDelay.Start(() =>
            {
                StartCatch();
                isBlocked = false;
            });

            if (!Owner.IsLocalPlayer)
            {
                rigidBody.isKinematic = true;
            }
        }

        protected override void SetupAsActive()
        {
            if (!Owner.IsLocalPlayer)
            {
                tickSystem.Unregister(this);
            }
        }

        protected override void OnBeforeDestroy()
        {
            if (!Owner.IsLocalPlayer)
                DisconnectEvents();
        }
        protected override void DisconnectEvents()
        {
            base.DisconnectEvents();

            startDelay.Stop();
            catchDuration.Stop();
            
            if (locationFloaty)
                locationFloaty.RequestDestroyFloaty();
        }
        #endregion

        #region Catching
        private void StartCatch() 
        {
            SetSFX(true);

            catchDuration.Start(
            () => // update
            {
                if (localPlayer.Role == PlayerRole.Hunted)
                    TryCatchLocal();
            },
            () => // finish
            {
                SetSFX(false);
            });
        }
        private void TryCatchLocal()
        {
            var suctionHits = BoxCast(centerPoint, suctionArea, suctionAreaOffset, huntedLayer);
            DrawBox(centerPoint, suctionArea, suctionAreaOffset);
            if (suctionHits.Length != 0)
            {
                var force = CalculateSuctionForce();
                localPlayer.PlayerCharacter.ControllerSetup.WalkController.SetMomentum(force);
            }
        
            var catchHits = BoxCast(centerPoint, catchArea, catchAreaOffset, huntedLayer);
            DrawBox(centerPoint, catchArea, catchAreaOffset);
            if (catchHits.Length != 0)
            {
                localPlayer.PlayerCharacter.ControllerSetup.GetBehaviourAs<HuntedBehaviour>().ResistanceMechanic.TryCatch();
            }
        }
        private Vector3 CalculateSuctionForce()
        {
            var huntedPos = localPlayer.PlayerCharacter.ControllerSetup.CharacterRoot.position;
            var huntedPos2D = new Vector2(huntedPos.x, huntedPos.z);
            var centerPos2D = new Vector2(centerPoint.position.x, centerPoint.position.z);

            if (Vector2.Distance(huntedPos2D, centerPos2D) < minSuctionDistance)
                return Vector2.zero;

            var dir2D = (centerPos2D - huntedPos2D).normalized;
            return new Vector3 (dir2D.x, 0, dir2D.y) * suctionStrength;
        }

        private void SetSFX(bool show)
        {
            if (setLightCoroutine != null)
                StopCoroutine(setLightCoroutine);

            setLightCoroutine = StartCoroutine(SetLight(show));
            foreach (var pS in particleSystems)
                pS.enableEmission = show;
        }
        private IEnumerator SetLight(bool show) 
        {
            float target = show ? lightIntensity : 0;

            while (light.intensity != target)
            {
                light.intensity = Mathf.MoveTowards(light.intensity, target, lightSpeed * Time.deltaTime);
                yield return null;
            }
        }
        #endregion
 
        #region Trigger Stuff
        protected override void OnTriggerHold(float duration)
        {
            foreach (var curTrigger in triggerPoints)
            {
                if (PlayerIsInArea(curTrigger) &&
                    !curTrigger.isCoolingDown)
                {
                    if (curTrigger.pressDuration <= duration)
                    {
                        OnTriggerInteracted(curTrigger.Id);
                    }
                    else
                    {
                        UpdateTriggerProgress(curTrigger, duration);
                    }
                }
            }
        }
        protected override void OnTriggerInteracted(byte pointId)
        {
            DisconnectEvents();
            foreach (var floaty in floaties)
            {
                if (floaty.Value)
                    floaty.Value.RequestDestroyFloaty();
            }

            messageHub.ShoutMessage<PlayerCollectedTrapMsg>(this);
        }
        protected override void OnFloatySpawned(int pointId, InteractionFloaty floaty)
        {
            floaty.SetDescription ("Collect trap");
        }
        protected override bool PlayerIsInArea(TriggerSetup trigger)
        {
            if (isBlocked)
                return false;

            return base.PlayerIsInArea(trigger);
        }
        #endregion
    }
}