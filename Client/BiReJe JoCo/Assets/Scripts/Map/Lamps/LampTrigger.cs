using UnityEngine;
using BiReJeJoCo.Backend;

namespace BiReJeJoCo.Map
{
    public class LampTrigger : SynchronizedTrigger
    {
        [Header("Lamp Trigger Settings")]
        [SerializeField] Light mainLight;
        [SerializeField] GameObject[] additionalObjects;

        private bool isOn;

        protected override void OnSetupActive()
        {
            base.OnSetupActive();
            isOn = mainLight.enabled;
        }

        protected override void OnTriggerInteracted(byte pointId)
        {
            isOn = !isOn;
            mainLight.enabled = isOn;
            foreach (var curObject in additionalObjects)
                curObject.SetActive(isOn);
        }
    }
}
