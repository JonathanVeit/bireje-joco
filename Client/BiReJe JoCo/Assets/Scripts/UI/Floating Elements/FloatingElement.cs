using JoVei.Base.UI;
using JoVei.Base;
using JoVei.Base.Helper;
using UnityEngine;

namespace BiReJeJoCo.UI
{
    public class FloatingElement : BaseFloatingElement, ITickable
    {
        [Header("Floating Settings")]
        [SerializeField] Vector2 clampSize;

        private bool isHided;
        private bool isClamped;

        private RectTransform rectTransform;

        protected override void OnInitialize()
        {
            tickSystem.Register(this);
            rectTransform = GetComponent<RectTransform>();
        }
        protected override void OnDestroyed()
        {
            tickSystem.Unregister(this);
            isHided = false;
            isClamped = false;
        }
        protected override void OnBeforeDestroy()
        {
            tickSystem.Unregister(this);
        }

        public void SetClamped()
        {
            isClamped = true;
        }

        public virtual void Tick(float deltaTime)
        {
            if (isHided || isClamped || Camera.main == null ||
                Config.Target == null) return;

            var pos = Camera.main.WorldToScreenPoint(Config.Target.position) + (Vector3)Config.Offset;
            if (pos.z < 0 && transform.gameObject.activeSelf)
            {
                transform.gameObject.SetActive(false);
            }
            else if (pos.z >= 0 && !transform.gameObject.activeSelf)
            {
                transform.gameObject.SetActive(true);
            }
        }
        private void LateUpdate()
        {
            if (!isClamped || Camera.main == null) return;

            var pos = Camera.main.WorldToScreenPoint(Config.Target.position) + (Vector3)Config.Offset;
            var heading = Config.Target.transform.position - Camera.main.transform.position;

            // out of screen
            if (Vector3.Dot(Camera.main.transform.forward, heading) < 0) 
            {
                pos = UIHelper.ClampScreenPointToScreenBorder(pos, true);
            }
 
            pos.x = Mathf.Clamp(pos.x, 0 + clampSize.x, Screen.width - clampSize.x);
            pos.y = Mathf.Clamp(pos.y, 0 + clampSize.x, Screen.height - clampSize.y);
            rectTransform.position = pos;
        }

        public void Hide() 
        {
            this.gameObject.SetActive(false);
            isHided = true;
        }
        public void Unhide() 
        {
            this.gameObject.SetActive(true);
            isHided = false;
        }
    }
}