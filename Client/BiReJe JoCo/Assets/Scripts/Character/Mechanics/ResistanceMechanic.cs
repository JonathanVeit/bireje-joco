using BiReJeJoCo.Backend;
using BiReJeJoCo.UI;
using System.Linq;
using UnityEngine;

namespace BiReJeJoCo.Character
{
    public class ResistanceMechanic : BaseBehaviourMechanic<HuntedBehaviour>
    {
        [Header("Settings")]
        [SerializeField] float maxResistance = 100f;
        [SerializeField] [Range(0.1f, 100)] float resistanceRegenerationRate = 1;
        [SerializeField] [Range(0.1f, 100)]  float resistanceLossRate = 1;
        [SerializeField] [Range(0, 1)] float resistanceLossByHunterAmount = 1;
        [SerializeField] [Range(0, 1)] float maxResistanceSlowdown = 1;

        [Space(10)]
        [SerializeField] float maxCatchDuration;
        [SerializeField] AnimationCurve catchDurationOverResistance;
        public SyncVar<float> RelativeCatchProgress = new SyncVar<float>(7, 0);

        public float CurrentResistance { get; private set; }
        public float RelativeResistance => CurrentResistance / maxResistance;
        public bool IsDecreasing { get; private set; }

        [Header("Runtime")]
        [SerializeField] float curCatchDuration;
        [SerializeField] bool catchSucceed;
        private SimpleMovementModification hitModification;

        #region Initialization
        protected override void OnInitializeLocal()
        {
            CurrentResistance = maxResistance;
            ConnectEvents();
        }
        protected override void OnInitializeRemote()
        {
        }
        protected override void OnBeforeDestroy()
        {
            DisconnectEvents();
        }

        private void ConnectEvents()
        {
            messageHub.RegisterReceiver< PlayerCharacterSpawnedMsg>(this, OnPlayerCharacterSpawned);
        }
        private void DisconnectEvents() 
        {
            messageHub.UnregisterReceiver(this);
            if (syncVarHub)
                syncVarHub.UnregisterSyncVar(RelativeCatchProgress);
        }
        #endregion

        #region Resistance Calculation
        public void UpdateResistance()
        {
            float hitPercentage = HittingHunterPercentage();
            if (hitPercentage == 0)
            {
                RegenerateResistance();
            }
            else
            {
                DecreaseResistance(hitPercentage);
            }

            UpdateResistanceInfluence();
        }

        private float HittingHunterPercentage()
        {
            var allHunter = playerManager.GetAllPlayer(x => x.Role == PlayerRole.Hunter).ToList();
            if (allHunter.Count == 0) return 0;

            int hittingHunter = 0;
            foreach (var hunter in allHunter)
            {
                if (hunter.PlayerCharacter == null) continue;

                if (hunter.PlayerCharacter.ControllerSetup.GetBehaviourAs<HunterBehaviour>().ShockMechanic.IsHittingHunted)
                    hittingHunter++;
            }

            if (hittingHunter == 0) return 0;

            return (float)hittingHunter / allHunter.Count;
        }

        private void RegenerateResistance()
        {
            CurrentResistance = Mathf.MoveTowards(CurrentResistance, maxResistance, resistanceRegenerationRate * Time.deltaTime);
            IsDecreasing = false;
        }
        private void DecreaseResistance(float hitPercentage)
        {
            IsDecreasing = true;

            if (Behaviour.TransformationMechanic.IsTransformed)
            {
                Behaviour.TransformationMechanic.TransformBack();
            }

            // rate depending on amount of hitting hunters  
            var minResistanceLoss = resistanceLossRate * hitPercentage;

            // how strong is the influence of hunter?
            var appliedResistanceLoss = Mathf.Lerp(minResistanceLoss, resistanceLossRate, 1 - resistanceLossByHunterAmount);

            // move resistance
            CurrentResistance = Mathf.MoveTowards(CurrentResistance, 0, appliedResistanceLoss * Time.deltaTime);
        }
        private void UpdateResistanceInfluence()
        {
            // update ui
            gameUI.UpdateHitOverlay(1 - RelativeResistance);

            // max resistance lost? -> max slow 
            var negRelativeResistance = 1 - RelativeResistance; // -> 1
            hitModification.Set(1 - (maxResistanceSlowdown * negRelativeResistance));
        }
        #endregion

        #region Catching
        public void TryCatch(float trapDuration)
        {
            if (catchSucceed) return;

            var negRelativeResistance = 1 - RelativeResistance; // -> 1
            curCatchDuration = maxCatchDuration * catchDurationOverResistance.Evaluate(negRelativeResistance);

            RelativeCatchProgress.SetValue(trapDuration / curCatchDuration);
            if (trapDuration >= curCatchDuration)
            {
                photonMessageHub.ShoutMessage<HuntedCatchedPhoMsg>(PhotonMessageTarget.AllViaServer);
                catchSucceed = true;
            }
        }
        #endregion

        #region Events
        void OnPlayerCharacterSpawned(PlayerCharacterSpawnedMsg msg)
        {
            hitModification = new SimpleMovementModification(1);
            localPlayer.PlayerCharacter.ControllerSetup.WalkController.AddModification(hitModification);
        }
        #endregion
    }
}