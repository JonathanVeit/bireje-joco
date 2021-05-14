using BiReJeJoCo.Backend;
using JoVei.Base;
using UnityEngine;
using UnityEngine.UI;

namespace BiReJeJoCo.UI
{
    public class HunterPingFloaty : FloatingElement
    {
        [Header("Settings")]
        [SerializeField] Text distanceLabel;

        private LocalPlayer localPlayer => DIContainer.GetImplementationFor<PlayerManager>().LocalPlayer;

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            distanceLabel.text = Mathf.CeilToInt(Vector3.Distance(Config.Target.position, localPlayer.PlayerCharacter.modelRoot.position)).ToString() + "m";
        }
    }
}
