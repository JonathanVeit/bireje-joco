using UnityEngine;
using JoVei.Base.UI;
using BiReJeJoCo.Character;
using BiReJeJoCo.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JoVei.Base;

namespace BiReJeJoCo.Backend
{
    public abstract class BaseTrigger : SystemBehaviour, ITickable
    {
        [Header("Base Trigger Settings")]
        [SerializeField] protected List<TriggerSetup> triggerPoints;
        [SerializeField] protected LayerMask playerLayer;
        [SerializeField] bool drawGizmos;

        protected Transform playerTransform;
        protected Dictionary<byte, InteractionFloaty> floaties;

        protected static BaseTrigger DisplayedInstance { get; private set; }
        protected static TriggerSetup DisplayedTrigger { get; private set; }

        #region Initialization
        protected sealed override void OnSystemsInitialized()
        {
            if (!PlayerRoleMatchesTarget(localPlayer.Role)) return;

            floaties = new Dictionary<byte, InteractionFloaty>();
            foreach (var curTrigger in triggerPoints)
                floaties.Add(curTrigger.Id, null);

            ConnectEvents();
            SetupAsActive();
        }
        protected virtual void SetupAsActive() { }

        protected virtual void ConnectEvents()
        {
            if (localPlayer.PlayerCharacter == null)
            {
                messageHub.RegisterReceiver<PlayerCharacterSpawnedMsg>(this, OnPlayerCharacterSpawned);
            }
            else
            {
                OnPlayerCharacterSpawned(null);
            }
            photonMessageHub.RegisterReceiver<CloseMatchPhoMsg>(this, OnCloseMatch);
        }
        protected virtual void DisconnectEvents() 
        {
            tickSystem.Unregister(this);
            messageHub.UnregisterReceiver(this);

            if (photonMessageHub)
                photonMessageHub.UnregisterReceiver(this);

            if (localPlayer.PlayerCharacter)
            {
                localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onTriggerPressed -= OnTriggerPressedInternal;
                localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onTriggerHold -= OnTriggerHoldInternal;
                localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onTriggerReleased -= OnTriggerReleasedInternal;
            }
        }
        #endregion

        public virtual void Tick(float deltaTime)
        {
            if (DisplayedInstance != this && DisplayedInstance != null)
                return;

            foreach (var curTrigger in triggerPoints)
            {
                if (PlayerIsInArea(curTrigger))
                {
                    if (floaties[curTrigger.Id] == null &&
                        !curTrigger.isCoolingDown &&
                        DisplayedInstance == null)
                    {
                        SpawnTriggerFloaty(curTrigger);
                        SetDisplayed(this, curTrigger);
                    }
                }
                else 
                {
                    DestroyTriggerFloaty(curTrigger);

                    if (curTrigger == DisplayedTrigger)
                    {
                        ResetDisplayed();
                    }
                }
            }

            OnTicked();
        }
        protected virtual void OnTicked() { }

        protected virtual void SpawnTriggerFloaty(TriggerSetup trigger)
        {
            var config = new FloatingElementConfig(trigger.floatingElementId, uiManager.GetInstanceOf<GameUI> ().floatingElementGrid, trigger.floatingElementTarget, trigger.floatingElementOffset);

            var floaty = floatingManager.GetElementAs<InteractionFloaty>(config);
            floaties[trigger.Id] = floaty;
            UpdateTriggerProgress(trigger, 0);
            OnFloatySpawned(trigger.Id, floaty);
        }
        protected virtual void DestroyTriggerFloaty(TriggerSetup trigger)
        {
            if (floaties[trigger.Id])
            {
                floatingManager.DestroyElement(floaties[trigger.Id]);
                floaties[trigger.Id] = null;
            }
        }

        protected abstract void OnTriggerInteracted(byte pointId);
        protected virtual void OnFloatySpawned(int pointId, InteractionFloaty floaty) { }

        #region Events
        private void OnTriggerPressedInternal()
        {
            if (DisplayedInstance != this)
                return;

            if (DisplayedTrigger.pressDuration == 0)
            {
                OnTriggerPressed();
            }
        }
        protected virtual void OnTriggerPressed()
        {
                OnTriggerInteracted(DisplayedTrigger.Id);
                StartCoroutine(CoolDown(DisplayedTrigger));
                ResetDisplayed();
        }

        private void OnTriggerHoldInternal(float duration)
        {
            if (DisplayedInstance != this)
                return;

            if (DisplayedTrigger.pressDuration > 0)
            {
                OnTriggerHold(duration);
            }
        }
        protected virtual void OnTriggerHold(float duration)
        {
            if (DisplayedTrigger.pressDuration <= duration)
            {
                OnTriggerInteracted(DisplayedTrigger.Id);
                StartCoroutine(CoolDown(DisplayedTrigger));
                UpdateTriggerProgress(DisplayedTrigger, 0);
                ResetDisplayed();
            }
            else
            {
                UpdateTriggerProgress(DisplayedTrigger, duration);
            }
        }

        private void OnTriggerReleasedInternal()
        {
            if (DisplayedInstance != this)
                return;

            OnTriggerReleased();
        }
        protected virtual void OnTriggerReleased() 
        {
            foreach (var curTrigger in triggerPoints)
            {
                UpdateTriggerProgress(curTrigger, 0);
            }
        }

        protected virtual void OnPlayerCharacterSpawned(PlayerCharacterSpawnedMsg msg)
        {
            tickSystem.Register(this, "update_quarter_second");
            playerTransform = localPlayer.PlayerCharacter.ControllerSetup.CharacterRoot;
            localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onTriggerPressed += OnTriggerPressedInternal;
            localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onTriggerHold += OnTriggerHoldInternal;
            localPlayer.PlayerCharacter.ControllerSetup.CharacterInput.onTriggerReleased += OnTriggerReleasedInternal;
        }
        protected virtual void OnCloseMatch(PhotonMessage msg)
        {
            DisconnectEvents();
        }
        #endregion

        #region Helper
        protected virtual bool PlayerRoleMatchesTarget(PlayerRole role)
        {
            foreach (var curTrigger in triggerPoints)
            {
                if (curTrigger.target == TriggerTarget.AllPlayer) return true;

                switch (role)
                {
                    case PlayerRole.Hunted:
                        return curTrigger.target == TriggerTarget.Hunted;
                    case PlayerRole.Hunter:
                        return curTrigger.target == TriggerTarget.Hunter;

                    default:
                        continue;
                }
            }

            return false;
        }
       
        protected virtual bool PlayerIsInArea(TriggerSetup trigger)
        {
            if (playerTransform == null ||
                Vector3.Distance(trigger.root.position, playerTransform.position) > trigger.areaSize.magnitude)
                return false;

            var offset = trigger.root.TransformDirection(trigger.areaOffset);
            var collisions = BoxCast(trigger.root, trigger.areaSize, trigger.areaOffset, playerLayer);

            if (collisions.Length == 0)
                return false;
            return collisions.ToList().Find(x => x.transform == playerTransform) != null;
        }
        protected Collider[] BoxCast(Transform root, Vector3 areaSize, Vector3 areaOffset, LayerMask mask) 
        {
            var offset = root.TransformDirection(areaOffset);
            return Physics.OverlapBox(root.position + offset, areaSize / 2, root.rotation, mask, QueryTriggerInteraction.Ignore);
        }

        void OnDrawGizmos()
        {
            if (!drawGizmos) return;

            foreach (var curTrigger in triggerPoints)
            {
                DrawBox(curTrigger.root, curTrigger.areaSize, curTrigger.areaOffset);
            }
        }
        protected void DrawBox(Transform root, Vector3 areaSize, Vector3 areaOffset)
        {
            var offset = root.TransformDirection(areaOffset);
            var corner = root.position + offset + (root.right * -0.5f * areaSize.x) + (root.up * -0.5f * areaSize.y) + (root.forward * -0.5f * areaSize.z);
            var color = Color.green;

            // directions 
            var forward = root.forward * areaSize.z;
            var right = root.right * areaSize.x;
            var up = root.up * areaSize.y;

            // bottom part
            Debug.DrawRay(corner, right, color);
            Debug.DrawRay(corner, forward, color);

            Debug.DrawRay(corner + forward, right, color);
            Debug.DrawRay(corner + right, forward, color);

            // top part 
            Debug.DrawRay(corner + up, right, color);
            Debug.DrawRay(corner + up, forward, color);

            Debug.DrawRay(corner + forward + up, right, color);
            Debug.DrawRay(corner + right + up, forward, color);

            // up part
            Debug.DrawRay(corner, up, color);
            Debug.DrawRay(corner + right, up, color);

            Debug.DrawRay(corner + forward, up, color);
            Debug.DrawRay(corner + right + forward, up, color);
        }

        protected virtual IEnumerator CoolDown(TriggerSetup trigger)
        {
            trigger.isCoolingDown = true;
            DestroyTriggerFloaty(trigger);
            yield return new WaitForSeconds(trigger.coolDown);
            trigger.isCoolingDown = false;
        }
        protected virtual void TryHideFloaty(TriggerSetup trigger)
        {
            if (floaties[trigger.Id] != null && trigger.hideInCooldown)
                floaties[trigger.Id].Hide();
        }
        protected virtual void TryUnhideFloaty(TriggerSetup trigger)
        {
            if (floaties[trigger.Id] != null && trigger.hideInCooldown)
                floaties[trigger.Id].Unhide();
        }

        protected virtual void UpdateTriggerProgress(TriggerSetup trigger, float duration) 
        {
            if (floaties[trigger.Id])
                floaties[trigger.Id].UpdateProgress(duration / trigger.pressDuration);
        }

        protected void SetDisplayed(BaseTrigger instance, TriggerSetup trigger)
        {
            DisplayedInstance = instance;
            DisplayedTrigger = trigger;
        }
        protected void ResetDisplayed() 
        {
            DisplayedInstance = null;
            DisplayedTrigger = null;
        }

        [System.Serializable]
        protected class TriggerSetup
        {
            public TriggerTarget target;
            public byte Id;
            public Transform root;

            [Space(10)]
            public Vector3 areaSize;
            public Vector3 areaOffset;
            public float pressDuration = 0f;
            public float coolDown = 1f;

            [Space(10)]
            public string floatingElementId = "default_trigger";
            public bool hideInCooldown = true;
            public Transform floatingElementTarget;
            public Vector2 floatingElementOffset;

            [Header("Debugging")]
            public bool isCoolingDown;
        }
        #endregion
    }
}