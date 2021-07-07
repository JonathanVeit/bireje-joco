using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BiReJeJoCo.UI
{
    public class UIOutlineController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Settings")]
        [SerializeField] Image target;

        private void Start()
        {
            if (target == null)
                target = GetComponent<Image>();

            OnPointerExit(null);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetAlpha(1);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetAlpha(0);
        }

        private void SetAlpha(float value)
        {
            var col = target.color;
            col.a = value;
            target.color = col;
        }
    }
}