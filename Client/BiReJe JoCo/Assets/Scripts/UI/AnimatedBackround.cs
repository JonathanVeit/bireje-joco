using System.Collections.Generic;
using UnityEngine;

namespace BiReJeJoCo.UI
{
    public class AnimatedBackround : SystemBehaviour
    {
        [SerializeField] List<Layer> layers;
        [SerializeField] float lerpSpeed;

        private void Start()
        {
            foreach (var layer in layers)
            {
                layer.origin = layer.target.anchoredPosition;
                layer.curPosition = layer.target.anchoredPosition;
                layer.curTarget = CalculateLaterTarget(layer);
            }
        }

        private void Update()
        {
            foreach (var layer in layers)
            {
                layer.curPosition = Vector2.MoveTowards(layer.curPosition, layer.curTarget, layer.speed * Time.deltaTime);
                layer.target.anchoredPosition = Vector3.Lerp(layer.target.anchoredPosition, layer.curPosition, lerpSpeed * Time.deltaTime);

                if (Vector3.Distance(layer.target.anchoredPosition, layer.curTarget) <= 0.1f)
                    layer.curTarget = CalculateLaterTarget(layer);
            }
        }

        private Vector2 CalculateLaterTarget(Layer layer)
        {
            var result = new Vector2();
            result.x = layer.origin.x + Random.Range(-layer.range.x, layer.range.x);
            result.y = layer.origin.y + Random.Range(-layer.range.y, layer.range.y);

            return result;
        }

        [System.Serializable]
        public class Layer
        {
            public string name;
            public RectTransform target;
            public float speed;
            public Vector2 range;

            [HideInInspector] public Vector3 origin;
            [HideInInspector] public Vector3 curPosition;
            [HideInInspector] public Vector2 curTarget;
        }
    }
}