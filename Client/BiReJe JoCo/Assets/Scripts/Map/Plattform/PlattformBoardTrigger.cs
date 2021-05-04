using UnityEngine;
using System.Collections.Generic;
using BiReJeJoCo.Character;
using BiReJeJoCo.Backend;

namespace BiReJeJoCo.Map
{
    public class PlattformBoardTrigger : SystemBehaviour
    {
        public List<AdvancedWalkerController> User { get; private set; }
            = new List<AdvancedWalkerController>();
        public List<AdvancedWalkerController> User2 = new List<AdvancedWalkerController>();

        private void OnTriggerEnter(Collider collision)
        {
            var controlled = collision.GetComponentsInParent<PlayerControlled>();

            if (controlled[0].Player.IsLocalPlayer)
            {
                User.Add(collision.GetComponent<AdvancedWalkerController>());
                User2.Add(collision.GetComponent<AdvancedWalkerController>());
            }
        }

        private void OnTriggerExit(Collider collision)
        {
            var controlled = collision.GetComponentsInParent<PlayerControlled>();

            if (controlled[0].Player.IsLocalPlayer)
            {
                User.Remove(collision.GetComponent<AdvancedWalkerController>());
                User2.Remove(collision.GetComponent<AdvancedWalkerController>());
            }
        }
    }
}
