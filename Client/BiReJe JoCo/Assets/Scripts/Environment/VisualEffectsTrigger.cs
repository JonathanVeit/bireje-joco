using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BiReJeJoCo
{
    [RequireComponent(typeof(Collider))]
    public class VisualEffectsTrigger : SystemBehaviour
    {
        [SerializeField] bool isUpstairs;
        VisualEffectsSwitchManager vfxManager;
        Collider col;

        protected override void OnSystemsInitialized()
        {
            base.OnSystemsInitialized();

            vfxManager = this.GetComponentInParent<VisualEffectsSwitchManager>();
            col = this.GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("LocalPlayer"))
            {
                vfxManager.HandlePPSSwitch(isUpstairs);
            }
        }
    }
}
