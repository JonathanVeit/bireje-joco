using System.Collections.Generic;
using UnityEngine;

namespace BiReJeJoCo.UI
{
    public class AnimatedBackround : SystemBehaviour
    {
        [SerializeField] List<Layer> layers;
        [SerializeField] float lerpSpeed;

        static List<Layer> cachedLayer = new List<Layer>();

        private void Awake()
        {
            if (cachedLayer.Count == 0)
            {
                foreach (var layer in layers)
                {
                    layer.origin = layer.target.anchoredPosition;
                    layer.curInterpolatedPosition = layer.target.anchoredPosition;
                    layer.curTargetPosition = CalculateLaterTarget(layer);
                    cachedLayer.Add(layer);
                }

                return;
            }

            for (int i = 0; i < layers.Count; i++)
            {
                cachedLayer[i].target = layers[i].target;
                cachedLayer[i].target.anchoredPosition = cachedLayer[i].cachedTargetPosition;
            }
        }

        private void Update()
        {
            foreach (var layer in cachedLayer)
            {
                layer.curInterpolatedPosition = Vector2.MoveTowards(layer.curInterpolatedPosition, layer.curTargetPosition, layer.speed * Time.deltaTime);
                layer.target.anchoredPosition = Vector3.Lerp(layer.target.anchoredPosition, layer.curInterpolatedPosition, lerpSpeed * Time.deltaTime);
                layer.cachedTargetPosition = layer.target.anchoredPosition;

                if (Vector3.Distance(layer.target.anchoredPosition, layer.curTargetPosition) <= 0.1f)
                    layer.curTargetPosition = CalculateLaterTarget(layer);
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
            [HideInInspector] public Vector3 cachedTargetPosition;
            [HideInInspector] public Vector3 curInterpolatedPosition;
            [HideInInspector] public Vector2 curTargetPosition;
        }
    }
}