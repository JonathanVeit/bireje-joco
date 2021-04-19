using UnityEngine;
using UnityEngine.UI;
using JoVei.Base.UI;

namespace BiReJeJoCo.UI
{
    [System.Serializable]
    public class RoomMemberEntry : UIElement
    {
        [SerializeField] Text memberName;

        public void Initialize(string memberName)
        {
            this.memberName.text = memberName;
        }
    }
}