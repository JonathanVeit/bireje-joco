using UnityEngine;
using UnityEngine.UI;

namespace BiReJeJoCo.UI
{
    public class InteractionFloaty : FloatingElement
    {
        [SerializeField] Text description;

        public void Initialize(string description)
        {
            this.description.text = description;
        }
    }
}
