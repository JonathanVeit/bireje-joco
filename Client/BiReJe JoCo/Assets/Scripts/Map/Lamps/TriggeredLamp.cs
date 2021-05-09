using UnityEngine;
using BiReJeJoCo.Backend;
using BiReJeJoCo.UI;

namespace BiReJeJoCo.Map
{
    public class TriggeredLamp : SynchronizedTrigger
    {
        [Header("Lamp Settings")]
        [SerializeField] Light mainLight;
        [SerializeField] GameObject[] additionalObjects;

        private bool isOn;

        protected override void SetupAsActive()
        {
            base.SetupAsActive();
            isOn = mainLight.enabled;
        }

        protected override void OnTriggerInteracted(byte pointId)
        {
            isOn = !isOn;
            mainLight.enabled = isOn;
            foreach (var curObject in additionalObjects)
                curObject.SetActive(isOn);
        }

        protected override void OnFloatySpawned(int pointId, FloatingElement floaty)
        {
            (floaty as InteractionFloaty).Initialize("Lamp");
        }
    }
}
