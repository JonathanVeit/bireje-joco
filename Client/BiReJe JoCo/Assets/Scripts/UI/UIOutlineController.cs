using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BiReJeJoCo.UI
{
    public class UIOutlineController : SystemBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Settings")]
        [SerializeField] Image target;
        [SerializeField] Button targetButton;

        [SerializeField] bool playSounds = true;

        private void Start()
        {
            if (target == null)
                target = GetComponent<Image>();
            if (targetButton == null)
                targetButton = GetComponent<Button>();

            SetAlpha(0);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (targetButton &&
               !targetButton.interactable)
                return;

            SetAlpha(1);
            if (playSounds)
                soundEffectManager.Play("ui_button_enter", transform.position, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (targetButton &&
                !targetButton.interactable)
                return;

            SetAlpha(0);
            if (playSounds)
                soundEffectManager.Play("ui_button_exit", transform.position, true);
        }
        public void OnPointerClick()
        {
            if (targetButton &&
                !targetButton.interactable)
                return;

            if (playSounds)
                soundEffectManager.Play("ui_button_click", transform.position, true);
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