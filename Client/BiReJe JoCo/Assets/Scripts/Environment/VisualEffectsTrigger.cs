using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BiReJeJoCo
{
    [RequireComponent(typeof(Collider))]
    public class VisualEffectsTrigger : SystemBehaviour
    {
        [SerializeField] bool goingDown = false;
        [SerializeField] bool goingUp = false;

        Collider col;

        protected override void OnSystemsInitialized()
        {
            base.OnSystemsInitialized();

            if (!goingDown && !goingUp)
            {
                Debug.LogError("Visual effects trigger is neiter 'going down' nor 'going up' ");
            }
            col = this.GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("LocalPlayer"))
            {
                if (goingDown)
                {
                    messageHub.ShoutMessage<PPSDownstairsMsg>(this);
                }

                if (goingUp)
                {
                    messageHub.ShoutMessage<PPSUpstairsMsg>(this);
                }
            }
        }
    }
}
