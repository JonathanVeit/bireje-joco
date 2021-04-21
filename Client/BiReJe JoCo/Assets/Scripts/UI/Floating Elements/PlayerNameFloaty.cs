using JoVei.Base.UI;
using UnityEngine;
using UnityEngine.UI;

namespace BiReJeJoCo.UI
{
    public class PlayerNameFloaty : BaseFloatingElement
    {
        [SerializeField] Text playerName;

        public void Initialize(string name)
        {
            playerName.text = name;
        }
    }
}
