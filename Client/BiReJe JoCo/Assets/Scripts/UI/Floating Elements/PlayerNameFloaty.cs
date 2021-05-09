using UnityEngine;
using UnityEngine.UI;

namespace BiReJeJoCo.UI
{
    public class PlayerNameFloaty : FloatingElement
    {
        [SerializeField] Text playerName;

        public void Initialize(string name)
        {
            playerName.text = name;
        }
    }
}
