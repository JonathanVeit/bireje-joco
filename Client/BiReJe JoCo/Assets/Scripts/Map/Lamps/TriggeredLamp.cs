using UnityEngine;
using BiReJeJoCo.Backend;
using BiReJeJoCo.UI;
using System.Collections;

namespace BiReJeJoCo.Map
{
    public class TriggeredLamp : SynchronizedTrigger
    {
        [Header("Lamp Settings")]
        [SerializeField] Light[] mainLight;
        [SerializeField] GameObject[] additionalObjects;

        [SerializeField] bool isOn;

        protected override void SetupAsActive()
        {
            base.SetupAsActive();

            foreach (Light _l in mainLight)
            {
                _l.enabled = isOn;
            }

            foreach (var curObject in additionalObjects)
                curObject.SetActive(isOn);
        }

        protected override void OnTriggerInteracted(byte pointId)
        {
            isOn = !isOn;
            foreach (Light _l in mainLight)
            {
                _l.enabled = isOn;
            }

            foreach (var curObject in additionalObjects)
                curObject.SetActive(isOn);
        }

        protected override void OnFloatySpawned(int pointId, InteractionFloaty floaty)
        {
            floaty.SetDescription(isOn? "Light off" : "Light on");
        }

        protected override IEnumerator CoolDown(TriggerSetup trigger)
        {
            yield return base.CoolDown(trigger);

            if (floaties.ContainsKey(trigger.Id) &&
                floaties[trigger.Id] != null)
                floaties[trigger.Id].SetDescription(isOn ? "Light off" : "Light on");
        }
    }
}
