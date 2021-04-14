using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace JoVei.Base.UI
{
    /// <summary>
    /// Can make any ui element draggable
    /// </summary>
    public class UIDraggable : EventTrigger
    {
        public bool IsOn { get; set; } = true;

        public event Action<UIDraggable, Vector2> onStartDragging;
        public event Action<UIDraggable, Vector2> onStopDragging;
        public event Action<UIDraggable, Vector2> onDragging;

        public void SetToDefaultPosition() 
        {
            transform.position = defaultPosition;
        }

        #region Dragging   
        private bool isDragging;
        private Vector2 offset;
        private Vector2 defaultPosition;

        public virtual void Start() 
        {
            defaultPosition = transform.position;
        }

        public virtual void Update()
        {
            if (isDragging && IsOn)
            {
                transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y) + offset;
                onDragging?.Invoke(this, transform.position);
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true;
            offset =  transform.position - Input.mousePosition;
            onStartDragging?.Invoke(this, transform.position);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false;
            onStopDragging?.Invoke(this, transform.position);
        }
        #endregion
    }
}