using UnityEngine;
using JoVei.Base.UI;
using BiReJeJoCo.Character;
using BiReJeJoCo.UI;
using System.Collections;
using System.Collections.Generic;

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
            messageHub.RegisterReceiver<OnPlayerPressedTriggerMsg>(this, OnPressedTrigger);
            
            floaties = new Dictionary<byte, FloatingElement>();
            foreach (var curTrigger in triggerPoints)
                floaties.Add(curTrigger.Id, null);

            OnSetupActive();
        }

        protected virtual void OnSetupActive() { }

        protected override void OnBeforeDestroy()
        {
            base.OnBeforeDestroy();
            messageHub.UnregisterReceiver(this);

            foreach(var curEntry in floaties)
            {
                if (curEntry.Value != null)
                    floatingManager.DestroyElement(curEntry.Value);
            }
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
            var config = new FloatingElementConfig(trigger.floatingElementId, gameUI.floatingElementGrid, trigger.floatingElementTarget, trigger.floatingElementOffset);
            if (!floaties.ContainsKey(trigger.Id))
                floaties.Add(trigger.Id, null);

            var floaty = floatingManager.GetElementAs<FloatingElement>(config);
            floaties[trigger.Id] = floaty;
            floaty.SetVisibleRenderer(trigger.visiblityRenderer);
            OnFloatySpawned(trigger.Id, floaty);
        }

        protected virtual void OnPressedTrigger(OnPlayerPressedTriggerMsg msg)
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


        protected abstract void OnTriggerInteracted(byte pointId);
        protected virtual void OnFloatySpawned(int pointId, FloatingElement floaty) { }

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
            if (playerTransform == null && localPlayer.PlayerCharacter != null)
                playerTransform = localPlayer.PlayerCharacter.GetComponentInChildren<Mover>().transform;

            if (collisions.Length == 0 || playerTransform == null) return false;

            return collisions[0].transform == playerTransform;
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
            yield return new WaitForSeconds(trigger.coolDown);
            trigger.isCoolingDown = false;
        }

        [System.Serializable]
        protected class TriggerSetup
        {
            public TriggerTarget target;
            public byte Id;
            public Transform root;

            public Vector3 areaSize;
            public Vector3 areaOffset;
            public float coolDown = 1f;

            public string floatingElementId = "default_trigger";
            public Transform floatingElementTarget;
            public Vector2 floatingElementOffset;
            public MeshRenderer visiblityRenderer;

            [Header("Debugging")]
            public bool isCoolingDown;
        }
        #endregion
    }
}
