using UnityEngine;

namespace BiReJeJoCo
{
    public class SnowFollow : SystemBehaviour
    {
        [SerializeField] GameObject player;
        [SerializeField] float offset;

        bool gameStart = false;

        protected override void OnSystemsInitialized()
        {
            messageHub.RegisterReceiver<PlayerCharacterSpawnedMsg>(this,OnCharacterSpawned);messageHub.RegisterReceiver<PlayerCharacterSpawnedMsg>(this,OnCharacterSpawned);
        }

        private void OnCharacterSpawned(PlayerCharacterSpawnedMsg obj)
        {
            player = localPlayer.PlayerCharacter.gameObject;
            gameStart = true;
        }

        void Update()
        {
            if (gameStart && player)
            {
                this.transform.position = player.transform.position + new Vector3(0, offset, 0);
            }
        }

        private void OnDestroy()
        {
            messageHub.UnregisterReceiver(this);
        }
    }
}
