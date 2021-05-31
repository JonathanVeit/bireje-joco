using UnityEngine;
using UnityEngine.UI;

namespace BiReJeJoCo.UI
{
    public class UIButtonSetup : SystemBehaviour
    {
        [Header("Settings")]
        [SerializeField] Image target;
        [SerializeField] string keyboard;
        [SerializeField] string gamePad;

        private void OnEnable()
        {
            var scheme = localPlayer.PlayerCharacter.ControllerSetup.PlayerInput.currentControlScheme;
            
            if (scheme == "Keyboard")
            {
                target.sprite = SpriteMapping.GetMapping().GetElementForKey(keyboard);
            }
            else if (scheme == "GamePad")
            {
                target.sprite = SpriteMapping.GetMapping().GetElementForKey(gamePad);
            }
        }
    }
}