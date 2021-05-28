using BiReJeJoCo.Backend;
using BiReJeJoCo.Items;
using UnityEngine;

namespace BiReJeJoCo.Character
{
    public class TrapMechanic : BaseBehaviourMechanic<HunterBehaviour>
    {
        [Header("Settings")]
        [SerializeField] Transform trapSpawnPoint;
        [SerializeField] LayerMask targetLayer;
        [SerializeField] float throwForce;
        [SerializeField] Vector3 extraThrowForce;
        [SerializeField] float trapThrowRange;

        private SyncVar<object[]> trapPosition = new SyncVar<object[]>(4);
        private GameObject trap;

        #region Initialization
        protected override void OnInitializeLocal()
        {
            ConnectEvents();
        }
        protected override void OnInitializeRemote()
        {
            trapPosition.OnValueReceived += (x => ThrowTrapInternal(x));
        }
        protected override void OnBeforeDestroy()
        {
            DisconnectEvents();
        }

        private void ConnectEvents()
        {
            messageHub.RegisterReceiver<PlayerCollectedTrapMsg>(this, OnTrapCollected);
        }
        private void DisconnectEvents() 
        {
            if (syncVarHub)
                syncVarHub.UnregisterSyncVar(trapPosition);
            messageHub.UnregisterReceiver(this);
        }
        #endregion

        public void ThrowTrap()
        {
            if (trap) return;

            var ray = new Ray()
            {
                origin = Camera.main.transform.position,
                direction = Camera.main.transform.forward,
            };

            var trapTarget = CalculateTrapTarget(ray);
            trapPosition.SetValue(new object[2] { trapSpawnPoint.position, trapTarget });
            ThrowTrapInternal(trapPosition.GetValue());
        }
        private Vector3 CalculateTrapTarget(Ray ray)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, trapThrowRange, targetLayer, QueryTriggerInteraction.Ignore))
            {
                return hit.point;
            }

            return Camera.main.transform.position + Camera.main.transform.forward * trapThrowRange;
        }

        private void ThrowTrapInternal(object[] positions)
        {
            if (positions == null) return;
            if (positions[0] == null)
            {
                Destroy(trap);
                trap = null;
                return;
            }

            var prefab = MatchPrefabMapping.GetMapping().GetElementForKey("hunter_trap");
            trap = Instantiate(prefab, (Vector3)positions[0], Quaternion.identity);
            var comp = trap.GetComponent<HunterTrap>();
            Controller.AddObservedComponent(comp);

            var force = ((Vector3)positions[1] - trapSpawnPoint.position) * throwForce + extraThrowForce;
            comp.rigidBody.AddForce(force, ForceMode.Impulse);
        }
        private void OnTrapCollected(PlayerCollectedTrapMsg msg)
        {
            trapPosition.SetValue(new object[1] { null });
            ThrowTrapInternal(trapPosition.GetValue());
        }
    }
}