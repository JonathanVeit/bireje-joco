using JoVei.Base.UI;
using UnityEngine;
using UnityEngine.UI;
using BiReJeJoCo.Backend;

namespace BiReJeJoCo.UI
{
    public class PlayerNameFloaty : FloatingElement
    {
        [SerializeField] Text playerName;
        [SerializeField] GameObject healthBarGO;
        [SerializeField] UIBarHandler healthBar;

        private SyncVar<int> healthVar;

        public void Initialize(string name)
        {
            playerName.text = name;
        }

        public void ShowHealthBar(SyncVar<int> healthVar) 
        {
            healthBarGO.SetActive(true);
            healthBar.SetValue(1);
            healthBar.OverrideValue(healthVar.GetValue());
            this.healthVar = healthVar;
        }

        public void HideHealthbar()
        {
            healthBarGO.SetActive(false);
            this.healthVar = null;
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            if (healthVar != null)
            {
                base.Tick(deltaTime);
                healthBar.SetValue(healthVar.GetValue() / 100);
            }
        }
    }
}
