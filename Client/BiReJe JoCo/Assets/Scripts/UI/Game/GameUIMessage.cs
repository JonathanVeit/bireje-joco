using UnityEngine;
using UnityEngine.UI;

namespace BiReJeJoCo.UI
{
    public class GameUIMessage : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] Text target;
        [SerializeField] Animator anim;

        private float maxCount;
        private float Counter;
        private bool count;

        public void Initialize(string text, float duration, Color color)
        {
            target.text = text;
            target.color = color;
            Counter = maxCount = duration;
            count = true;
            anim.SetTrigger("pulsate");
        }

        // Update is called once per frame
        void Update()
        {
            if (!count)
                return;

            Counter -= Time.deltaTime;
            var color = target.color;
            color.a = Mathf.InverseLerp(0, maxCount, Counter);
            target.color = color;

            if (Counter <= 0)
                Destroy(this.gameObject);
        }
    }
}