using UnityEngine;
using UnityEngine.UI;

namespace BiReJeJoCo.UI
{
    public class InteractionFloaty : FloatingElement
    {
        [Header("Settings")]
        [SerializeField] Image durationIcon;
        [SerializeField] Text description;

        public void SetDescription(string description)
        {
            this.description.text = description;
        }

        public void UpdateProgress(float progress)
        {
            durationIcon.enabled = progress > 0;
            durationIcon.fillAmount = progress;
        }
    }
}
