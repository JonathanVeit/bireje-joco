using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BiReJeJoCo.UI
{
    public class UIColorAnimation : TickBehaviour
    {
        [Header("Settings")]
        [SerializeField] Graphic target;
        [SerializeField] bool startMax;
        [SerializeField] float speed;
        [SerializeField] [Range(0, 1)] float minAlpha;
        [SerializeField] [Range(0, 1)] float maxAlpha;

        bool fill;
        float delta;

        protected override void OnSystemsInitialized()
        {
            base.OnSystemsInitialized();

            if (target == null)
            {
                target = GetComponent<Image>();
            }

            fill = startMax;
            delta = fill ? 1 : 0;
            UpdateAlpha(Mathf.Lerp(minAlpha, maxAlpha, delta));
        }

        public override void Tick(float deltaTime)
        {
            if (fill)
            {
                delta += deltaTime;
                var alpha = Mathf.Lerp(minAlpha, maxAlpha, delta);
                UpdateAlpha(alpha);
                
                if (delta >= 1)
                    fill = false;
            }
            else
            {
                delta -= deltaTime;
                
                var alpha = Mathf.Lerp(minAlpha, maxAlpha, delta);
                UpdateAlpha(alpha);

                if (delta <= 0)
                    fill = true;
            }
        }

        private void UpdateAlpha(float value)
        {
            var color = target.color;
            color.a = value;
            target.color = color;
        }
    }
}