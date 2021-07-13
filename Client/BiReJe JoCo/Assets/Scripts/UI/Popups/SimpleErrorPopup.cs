using System;
using UnityEngine;
using UnityEngine.UI;

namespace BiReJeJoCo.UI
{
    public class SimpleErrorPopup : Popup
    {
        [Header("Settings")]
        [SerializeField] Text title;
        [SerializeField] Text caption;
        [SerializeField] Text subCaption;
        [SerializeField] Button button1;
        [SerializeField] Button button2;

        private Action onButton1Pressed;
        private Action onButton2Pressed;

        public void Show(string title, string caption, string button1Caption, Action onButton1Callback, string subCaption = null, string button2Caption = null, Action onButton2Callback = null)
        {
            base.Show();

            this.title.text = title;
            this.caption.text = caption;
            button1.GetComponentInChildren<Text>().text = button1Caption;
            onButton1Pressed = onButton1Callback;

            if (!string.IsNullOrEmpty(button2Caption))
            {
                button2.gameObject.SetActive(true);
                button2.GetComponentInChildren<Text>().text = button2Caption;
                onButton2Pressed = onButton2Callback;
            }
            else
                button2.gameObject.SetActive(false);

            if (!string.IsNullOrEmpty(subCaption))
            {
                this.subCaption.gameObject.SetActive(true);
                this.subCaption.text = subCaption;
            }
            else
                this.subCaption.gameObject.SetActive(false);
        }

        public void OnButton1Clicked() 
        {
            onButton1Pressed?.Invoke();

            onButton1Pressed = null;
            onButton2Pressed = null;

            Hide();
        }

        public void OnButton2Clicked() 
        {
            onButton2Pressed?.Invoke();

            onButton1Pressed = null;
            onButton2Pressed = null;

            Hide();
        }
    }
}