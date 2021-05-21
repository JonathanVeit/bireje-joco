using System.Collections.Generic;
using BiReJeJoCo.Backend;
using UnityEngine;

namespace BiReJeJoCo.Character
{
    public class CharacterControllerSetup : SystemBehaviour, IPlayerObserved
    {
        [Header("Components")]
        public Transform characterRoot;
        public Transform modelRoot;
        public Rigidbody rigidBody;
        public Collider mainCollider;
        public PlayerCharacterInput characterInput;
        public Mover mover;

        public Player Owner => controller.Player;
        private PlayerControlled controller;

        #region Initialization
        public void Initialize(PlayerControlled controller)
        {
            this.controller = controller;
        }
        #endregion
    }
}