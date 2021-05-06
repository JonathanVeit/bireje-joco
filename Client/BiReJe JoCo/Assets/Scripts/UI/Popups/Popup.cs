using System.Collections.Generic;

namespace BiReJeJoCo.UI
{
    /// <summary>
    /// Can be shown & hided
    /// </summary>
    public class Popup : UIElement
    {
        public static List<Popup> OpenPopups { get; set; } = new List<Popup>();
        public static List<Popup> History { get; set; } = new List<Popup>();
        public static Popup LastOpened { get; set; }
        public static Popup LastClosed { get; set; }
        
        public static int OpenCount => OpenPopups.Count;
        public static int HistoryCount => History.Count;
        public static bool PopupIsOpen => OpenPopups.Count > 0;

        public static int HistoryLength = 20;

        protected static List<Popup> cachedPopups;


        public bool IsOpen { get; protected set; }

        public virtual void Show() 
        {
            if (!OpenPopups.Contains(this))
            {
                this.gameObject.SetActive(true);
                OpenPopups.Add(this);
                LastOpened = this;

                History.Insert(0, this);

                if (History.Count > HistoryLength)
                {
                    History.RemoveAt(History.Count - 1);  
                }
            }

            IsOpen = true;
        }

        public virtual void Hide() 
        {
            if (OpenPopups.Contains(this))
            {
                this.gameObject.SetActive(false);
                OpenPopups.Remove(this);
                LastClosed = this;
            }

            IsOpen = false;
        }

        public static void HideAll(bool save = false) 
        {
            cachedPopups = save ? new List<Popup>(OpenPopups) : null;

            foreach (var curPopup in OpenPopups)
            {
                curPopup.Hide();
            }
        }

        public static void UnhideAll() 
        {
            if (cachedPopups != null)
            {
                foreach (var curPopup in cachedPopups)
                    curPopup.Show();

                cachedPopups = null;
            }
        }
    }
}