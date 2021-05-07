using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BiReJeJoCo
{
    [RequireComponent(typeof(Collider))]
    public class VisualEffectsTrigger : SystemBehaviour
    {
        Collider col;

        protected override void OnSystemsInitialized()
        {
            base.OnSystemsInitialized();

            col = this.GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("LocalPlayer"))
            {
                messageHub.ShoutMessage<PPSSwitchMsg>(this);
            }
        }
    }
}
