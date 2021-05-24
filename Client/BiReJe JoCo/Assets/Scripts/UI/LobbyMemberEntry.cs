using UnityEngine;
using UnityEngine.UI;
using JoVei.Base.UI;

namespace BiReJeJoCo.UI
{
    [System.Serializable]
    public class LobbyMemberEntry : UIElement
    {
        [Header("Settings")]
        [SerializeField] Text memberName;
        [SerializeField] Text memberState;

        public void Initialize(string memberName, bool isHost= false)
        {
            this.memberName.text = memberName;
            this.memberState.text = isHost ? "Host" : "Client";
        }
    }
}