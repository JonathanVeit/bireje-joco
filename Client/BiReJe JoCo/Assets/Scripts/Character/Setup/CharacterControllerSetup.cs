using BiReJeJoCo.Backend;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BiReJeJoCo.Character
{
    public class CharacterControllerSetup : SystemBehaviour, IPlayerObserved
    {
        [Header("Components")]
        [SerializeField] BaseBehaviour behaviour;
        [SerializeField] Transform characterRoot;
        [SerializeField] Transform modelRoot;
        [SerializeField] Rigidbody rigidBody;
        [SerializeField] Collider mainCollider;
        [SerializeField] PlayerInput playerInput;
        [SerializeField] PlayerCharacterInput characterInput;
        [SerializeField] AdvancedWalkerController walkController;
        [SerializeField] Mover mover;

        public Player Owner => controller.Player;
        private PlayerControlled controller;

        #region Initialization
        public void Initialize(PlayerControlled controller)
        {
            this.controller = controller;
        }
        #endregion

        #region Access 
        public Transform CharacterRoot => characterRoot;
        public Transform ModelRoot => modelRoot;
        public Rigidbody RigidBody => rigidBody;
        public Collider MainCollider => mainCollider;
        public PlayerInput PlayerInput => playerInput;
        public PlayerCharacterInput CharacterInput => characterInput;
        public AdvancedWalkerController WalkController => walkController;
        public Mover Mover => mover;
        public BaseBehaviour Behaviour => behaviour;
        public TBehaviour GetBehaviourAs<TBehaviour>()
            where TBehaviour : BaseBehaviour
        {
            return Behaviour as TBehaviour;
        }
        #endregion
    }
}