using JoVei.Base.Data;
using UnityEngine;
using UnityEngine.UI;

namespace JoVei.Base.UI
{
    /// <summary>
    /// Can be used to display all information about an Instance
    /// Implementations should be able to only display information when a inspector reference has been assigned and skip it if not
    /// </summary>
    [System.Serializable]
    public abstract class UIInstanceDisplay<TInstance, TData> : BaseSystemAccessor
        where TData : ProgressiveBaseData<TData>, new()
        where TInstance : BaseInstance<TData>, new()
    {
        [Tooltip("Aditional GameObjects to show/hide")]
        public Component[] additionalElements;
        public virtual TInstance DisplayedInstance { get; protected set; }

        /// <summary>
        /// Display given instance 
        /// Automatically sets visiblity to true
        /// </summary>
        public virtual void Display(TInstance instance)
        {
            DisplayedInstance = instance;
            SetVisibility(true);
            UpdateUI();
        }

        /// <summary>
        /// Set visibility of all ui elements 
        /// </summary>
        public virtual void SetVisibility(bool isVisible)
        {
            UpdateVisibility(isVisible);
            SetAdditionalElementsVisiblity(isVisible);
        }

        /// <summary>
        /// Set visiblity of all additional assigned elements 
        /// </summary>
        protected virtual void SetAdditionalElementsVisiblity(bool isVisible)
        {
            foreach (var curElement in additionalElements)
                TrySetVisibility(curElement, isVisible);
        }

        #region Abstract Member
        public abstract void UpdateUI();
        protected abstract void UpdateVisibility(bool isVisible);
        #endregion

        #region Helper
        protected bool TryDisplay(Text text, string content)
        {
            if (text)
            {
                text.text = content;
                return true;
            }

            return false;
        }

        protected bool TryDisplay(Image image, Sprite icon)
        {
            if (image)
            {
                image.sprite = icon;
                return true;
            }

            return false;
        }

        protected bool TryDisplay(Image image, float fillAmount)
        {
            if (image)
            {
                image.fillAmount = fillAmount;
                return true;
            }

            return false;
        }

        protected bool TrySetVisibility(Component component, bool isVisible)
        {
            if (component)
            {
                component.gameObject.SetActive(isVisible);
                return true;
            }

            return false;
        }
        #endregion
    }
}