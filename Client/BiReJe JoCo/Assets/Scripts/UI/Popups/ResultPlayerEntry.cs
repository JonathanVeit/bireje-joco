using BiReJeJoCo.Backend;
using UnityEngine;
using UnityEngine.UI;

namespace BiReJeJoCo.UI
{
    public class ResultPlayerEntry : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] Text playerName;
        [SerializeField] GameObject leavedOverlay;

        public Player DisplayedPlayer { get; private set; }

        public void Initialize(Player player)
        {
            this.DisplayedPlayer = player;
            this.playerName.text = DisplayedPlayer.NickName;
        }

        public void SetLeaved() 
        {
            leavedOverlay.gameObject.SetActive(true);
        }
    }
}