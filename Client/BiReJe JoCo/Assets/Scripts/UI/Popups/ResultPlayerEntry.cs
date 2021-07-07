using UnityEngine;
using UnityEngine.UI;

namespace BiReJeJoCo.UI
{
    public class ResultPlayerEntry : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] Text playerName;

        public void Initialize(string playerName)
        {
            this.playerName.text = playerName;
        }
    }
}