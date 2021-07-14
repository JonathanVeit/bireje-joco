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
        [SerializeField] float lightSpeed;
        [SerializeField] ParticleSystem[] particleSystems;
        [SerializeField] Animator[] animators;
        [SerializeField] Transform catchSign;
        [SerializeField] float catchSignSize;
        [SerializeField] Vector3 catchSignStartPoint;
        [SerializeField] Vector3 catchSignEndPoint;

        private bool isBlocked = true;
        private FloatingElement locationFloaty;
        private float curCatchDuration;

        public Player Owner => controller.Player;
        private PlayerControlled controller;

        #region Initialization
        public void Initialize(PlayerControlled controller)
        {
            this.controller = controller;
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

                var catchHits = BoxCast(centerPoint, catchArea, catchAreaOffset, huntedLayer);
                DrawBox(centerPoint, catchArea, catchAreaOffset);
                if (catchHits.Length != 0)
                {
                    var hunted = playerManager.GetAllPlayer(x => x.Role == PlayerRole.Hunted)[0];
                    var catchProgress = hunted.PlayerCharacter.ControllerSetup.GetBehaviourAs<HuntedBehaviour>().ResistanceMechanic.RelativeCatchProgress.GetValue();
                    catchSign.transform.localScale = new Vector3(catchProgress * catchSignSize, catchProgress * catchSignSize, catchProgress* catchSignSize);
                    catchSign.transform.localPosition = Vector3.Lerp(catchSignStartPoint, catchSignEndPoint, catchProgress);
                }
                else
                    catchSign.transform.localScale = new Vector3(0, 1, 0);
            },
            () => // finish
            {
                SetSFX(false);
                catchSign.transform.localScale = new Vector3(0, 1, 0);
                catchSign.transform.localPosition = catchSignStartPoint;
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
                curCatchDuration += Time.deltaTime;
                localPlayer.PlayerCharacter.ControllerSetup.GetBehaviourAs<HuntedBehaviour>().ResistanceMechanic.TryCatch(curCatchDuration);
            }
            else
                curCatchDuration = 0;
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
            foreach (var pS in particleSystems)
                pS.enableEmission = show;

            foreach (var anim in animators)
                anim.SetTrigger(show ? "start" : "stop");
        }
        #endregion
 
        #region Trigger Stuff
        protected override void OnTriggerHold(float duration)
        {
            if (DisplayedTrigger.pressDuration <= duration)
            {
                OnTriggerInteracted(DisplayedTrigger.Id);
                ResetActiveInstance();
            }
            else
            {
                UpdateTriggerProgress(DisplayedTrigger, duration);
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

            photonMessageHub.ShoutMessage<HunterCollectedTrapPhoMsg>(PhotonMessageTarget.All, Owner.NumberInRoom);
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

        #region Floaty
        protected override void OnTicked()
        {
            if (!localPlayer.PlayerCharacter)
                return;

            if (Vector3.Distance(rigidBody.position, localPlayer.PlayerCharacter.ControllerSetup.CharacterRoot.position) >= showFloatyAt)
            {
                if (!locationFloaty)
                {
                    var config = new FloatingElementConfig("trap_location", uiManager.GetInstanceOf<GameUI>().floatingElementGrid, transform);
                    locationFloaty = floatingManager.GetElementAs<FloatingElement>(config);
                }
            }
            else if (locationFloaty)
            {
                locationFloaty.RequestDestroyFloaty();
                locationFloaty = null;
            }
        }
        #endregion
    }
}