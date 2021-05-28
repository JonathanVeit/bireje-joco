﻿using UnityEngine;
using BiReJeJoCo.Backend;
using BiReJeJoCo.UI;

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
            floaty.SetDescription("Lamp");
        }
    }
}
