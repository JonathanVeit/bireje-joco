using UnityEngine;
using JoVei.Base.UI;
using BiReJeJoCo.Character;
using BiReJeJoCo.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BiReJeJoCo.Backend
{
    public abstract class BaseTrigger : TickBehaviour
    {
        [Header("Base Trigger Settings")]
        [SerializeField] protected TriggerSetup[] triggerPoints;
        [SerializeField] LayerMask playerLayer;

        protected Transform playerTransform;
        protected Dictionary<byte, FloatingElement> floaties;

        #region Initialization
        protected sealed override void OnSystemsInitialized()
        {
            if (!PlayerRoleMatchesTarget(localPlayer.Role)) return;

            tickSystem.Register(this, "update_half_second");
            messageHub.RegisterReceiver<PlayerPressedTriggerMsg>(this, OnPressedTrigger);
            messageHub.RegisterReceiver<PlayerCharacterSpawnedMsg>(this, OnPlayerCharacterSpawned);
            
            floaties = new Dictionary<byte, FloatingElement>();
            foreach (var curTrigger in triggerPoints)
                floaties.Add(curTrigger.Id, null);

            SetupAsActive();
        }

        protected virtual void SetupAsActive() { }

        protected override void OnBeforeDestroy()
        {
            base.OnBeforeDestroy();
            messageHub.UnregisterReceiver(this);
        }
        #endregion

        public override void Tick(float deltaTime)
        {
            foreach (var curTrigger in triggerPoints)
            {
                if (PlayerIsInArea(curTrigger))
                {
                    if (floaties[curTrigger.Id] == null)
                    {
                        ShowTriggerPointFloaty(curTrigger);
                    }
                }
                else
                {
                    if (floaties[curTrigger.Id] != null)
                    {
                        floatingManager.DestroyElement(floaties[curTrigger.Id]);
                        floaties[curTrigger.Id] = null;
                    }
                }
            }
        }

        protected virtual void ShowTriggerPointFloaty(TriggerSetup trigger)
        {
            var config = new FloatingElementConfig(trigger.floatingElementId, uiManager.GetInstanceOf<GameUI> ().floatingElementGrid, trigger.floatingElementTarget, trigger.floatingElementOffset);

            var floaty = floatingManager.GetElementAs<FloatingElement>(config);
            floaties[trigger.Id] = floaty;
            floaty.SetVisibleRenderer(trigger.visiblityRenderer);
            OnFloatySpawned(trigger.Id, floaty);
        }

        protected abstract void OnTriggerInteracted(byte pointId);
        protected virtual void OnFloatySpawned(int pointId, FloatingElement floaty) { }

        #region Events
        protected virtual void OnPressedTrigger(PlayerPressedTriggerMsg msg)
        {
            foreach (var curTrigger in triggerPoints)
            {
                if (PlayerIsInArea(curTrigger) && !curTrigger.isCoolingDown)
                {
                    OnTriggerInteracted(curTrigger.Id);
                    StartCoroutine(CoolDown(curTrigger));
                }
            }
        }

        protected virtual void OnPlayerCharacterSpawned(PlayerCharacterSpawnedMsg msg)
        {
            playerTransform = localPlayer.PlayerCharacter.GetComponentInChildren<Mover>().transform;
        }
        #endregion

        #region Helper
        protected bool PlayerRoleMatchesTarget(PlayerRole role)
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
       
        protected bool PlayerIsInArea(TriggerSetup trigger)
        {
            var offset = trigger.root.TransformDirection(trigger.areaOffset);
            var collisions = Physics.OverlapBox(trigger.root.position + offset, trigger.areaSize / 2, trigger.root.rotation, playerLayer, QueryTriggerInteraction.Ignore);
            
            if (collisions.Length == 0 || playerTransform == null) return false;
            return collisions.ToList().Find(x => x.transform == playerTransform) != null;
        }
        void OnDrawGizmos()
        {
            foreach (var curTrigger in triggerPoints)
            {
                var root = curTrigger.root;
                var offset = curTrigger.root.TransformDirection(curTrigger.areaOffset);
                var corner = root.position + offset + (root.right * -0.5f * curTrigger.areaSize.x) + (root.up * -0.5f * curTrigger.areaSize.y) + (root.forward * -0.5f * curTrigger.areaSize.z);
                var color = Color.green;

                // directions 
                var forward = root.forward * curTrigger.areaSize.z;
                var right = root.right * curTrigger.areaSize.x;
                var up = root.up * curTrigger.areaSize.y;

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
        }

        protected virtual IEnumerator CoolDown(TriggerSetup trigger)
        {
            trigger.isCoolingDown = true;
            TryHideFloaty(trigger);
             yield return new WaitForSeconds(trigger.coolDown);
            TryUnhideFloaty(trigger);
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

        [System.Serializable]
        protected class TriggerSetup
        {
            public TriggerTarget target;
            public byte Id;
            public Transform root;

            [Space(10)]
            public Vector3 areaSize;
            public Vector3 areaOffset;
            public float coolDown = 1f;

            [Space(10)]
            public string floatingElementId = "default_trigger";
            public bool hideInCooldown = true;
            public Transform floatingElementTarget;
            public Vector2 floatingElementOffset;
            public MeshRenderer visiblityRenderer;

            [Header("Debugging")]
            public bool isCoolingDown;
        }
        #endregion
    }
}
