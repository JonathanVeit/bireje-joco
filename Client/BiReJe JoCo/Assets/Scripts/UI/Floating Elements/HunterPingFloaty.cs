using BiReJeJoCo.Backend;
using JoVei.Base;
using UnityEngine;
using UnityEngine.UI;

namespace BiReJeJoCo.UI
{
    public class HunterPingFloaty : FloatingElement
    {
        [Header("Settings")]
        [SerializeField] Image icon;
        [SerializeField] Text distanceLabel;

        private LocalPlayer localPlayer => DIContainer.GetImplementationFor<PlayerManager>().LocalPlayer;

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);

            if (localPlayer.PlayerCharacter != null && 
                Config.Target != null)
                distanceLabel.text = Mathf.CeilToInt(Vector3.Distance(Config.Target.position, localPlayer.PlayerCharacter.controllerSetup.modelRoot.position)).ToString() + "m";
        }

        public void SetAlpha(float alpha) 
        {
            var iconColor = icon.color;
            iconColor.a = alpha;
            icon.color = iconColor;

            var labelColor = distanceLabel.color;
            labelColor.a = alpha;
            distanceLabel.color = labelColor;
        }
    }
}
