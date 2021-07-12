using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BiReJeJoCo.UI
{
    public class UIOutlineController : SystemBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Settings")]
        [SerializeField] Image target;
        [SerializeField] bool playSounds = true;

        private void Start()
        {
            if (target == null)
                target = GetComponent<Image>();

            SetAlpha(0);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetAlpha(1);
            if (playSounds)
                soundEffectManager.Play("ui_button_enter", transform.position, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetAlpha(0);
            if (playSounds)
                soundEffectManager.Play("ui_button_exit", transform.position, true);
        }
        public void OnPointerClick()
        {
            if (playSounds)
                soundEffectManager.Play("ui_button_click", transform.position, true);
        }

        private void SetAlpha(float value)
        {
            var col = target.color;
            col.a = value;
            target.color = col;
        }

    }
}