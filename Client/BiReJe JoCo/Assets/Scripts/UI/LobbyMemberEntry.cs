using UnityEngine;
using UnityEngine.UI;
using JoVei.Base.UI;

namespace BiReJeJoCo.UI
{
    [System.Serializable]
    public class LobbyMemberEntry : UIElement
    {
        [SerializeField] Text memberName;

        public void Initialize(string memberName, bool isHost= false)
        {
            this.memberName.text = memberName;

            if (isHost)
                this.memberName.text += " (host)";
        }
    }
}