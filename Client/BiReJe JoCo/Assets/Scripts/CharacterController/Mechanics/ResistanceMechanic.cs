using BiReJeJoCo.Backend;
using BiReJeJoCo.Items;
using BiReJeJoCo.UI;
using System.Linq;
using UnityEngine;

namespace BiReJeJoCo.Character
{
    public class ResistanceMechanic : BaseBehaviourMechanic<HuntedBehaviour>
    {
        [Header("Settings")]
        [SerializeField] float maxResistance = 100f;
        [SerializeField] float minResistance = 20f;
        [SerializeField] float resistanceRegenerationRate = 1;
        [SerializeField] float resistanceLossRate = 1;
        [SerializeField] [Range(0, 1)] float resistanceLossByAmount = 1;
        [SerializeField] [Range(0, 1)] float maxResistanceSlowdown = 1;

        [Space(10)]
        [SerializeField] int catchDifficulty;
        [SerializeField] AnimationCurve difficultyOverResistance;

        public float CurrentResistance { get; private set; }
        public bool IsDecreasing { get; private set; }

        private MovementMultiplier hitMultiplier;
        private bool catchSucceed;

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

                if (hunter.PlayerCharacter.ControllerSetup.GetBehaviourAs<HunterBehaviour>().ShockMechanic.isHitting.GetValue())
                    hittingHunter++;
            }

            if (hittingHunter == 0) return 0;

            return (float)hittingHunter / (float)allHunter.Count;
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

            // lose till how much?
            var appliedMinResistance = minResistance;

            // rate depending on amount of hitting hunters  
            var lossRate = resistanceLossRate * hitPercentage;

            // how strong is the influence of hunter?
            var appliedLossRate = Mathf.Lerp(lossRate, resistanceLossRate, resistanceLossByAmount);

            // move resistance
            CurrentResistance = Mathf.MoveTowards(CurrentResistance, appliedMinResistance, appliedLossRate * Time.deltaTime);
        }
        private void UpdateResistanceInfluence()
        {
            // how much resistance has been lost compared to max?
            var resistancePercentage = Mathf.InverseLerp(minResistance, maxResistance, CurrentResistance); // -> 0

            // update ui
            gameUI.UpdateHitOverlay(1 - resistancePercentage);

            // max resistance lost? -> max slow 
            var negResistancePercentage = 1 - resistancePercentage; // -> 1
            hitMultiplier.Set(1 - (maxResistanceSlowdown * negResistancePercentage));

            //Debug.Log("Resistance: " + Resistance);
        }
        #endregion

        #region Catching
        public void TryCatch()
        {
            if (catchSucceed) return;

            var resistancePercentage = Mathf.InverseLerp(minResistance, maxResistance, CurrentResistance); // -> 0
            var negResistancePercentage = 1 - resistancePercentage; // -> 1

            var multiplier = difficultyOverResistance.Evaluate(negResistancePercentage);

            var decision = Random.Range(0, (int)(catchDifficulty * multiplier));
            
            if (decision == 0)
            {
                photonMessageHub.ShoutMessage<HuntedCatchedPhoMsg>(PhotonMessageTarget.MasterClient);
                catchSucceed = true;
            }
        }
        #endregion

        #region Events
        void OnPlayerCharacterSpawned(PlayerCharacterSpawnedMsg msg)
        {
            hitMultiplier = new MovementMultiplier(1);
            localPlayer.PlayerCharacter.ControllerSetup.WalkController.AddMultiplier(hitMultiplier);
        }
        #endregion
    }
}