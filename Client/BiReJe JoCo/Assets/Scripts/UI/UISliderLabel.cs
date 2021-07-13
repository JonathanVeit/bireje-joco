using System;
using UnityEngine;
using UnityEngine.UI;

namespace BiReJeJoCo.UI
{
    [RequireComponent(typeof(Text))]
    public class UISliderLabel : TickBehaviour
    {
        [Header("Settings")]
        [SerializeField] Slider target;
        [SerializeField] int maxDigits;
        [SerializeField] float visibileDuration = 0.1f;
        [SerializeField] bool skipFirstUpdate = true;

        private Text text;
        private float counter;
        private bool isFirst = true;

        protected override void OnSystemsInitialized()
        {
            base.OnSystemsInitialized();
            text = GetComponent<Text>();
            text.gameObject.SetActive(false);
            counter = visibileDuration;

            target.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(float value)
        {
            if (skipFirstUpdate && isFirst)
            {
                isFirst = false;
                return;
            }

            text.text = Math.Round(value, maxDigits).ToString();
            text.gameObject.SetActive(true);
            counter = 0;
        }

        public override void Tick(float deltaTime)
        {
            if (counter >= visibileDuration)
                return;
         
            counter += deltaTime;
            if (counter >= visibileDuration)
                text.gameObject.SetActive(false);
        }
    }
}