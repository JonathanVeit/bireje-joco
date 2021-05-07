using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace BiReJeJoCo
{
    public class SnowFollow : SystemBehaviour
    {
        [SerializeField] GameObject player;
        bool gameStart = false;

        [SerializeField] float offset;

        protected override void OnSystemsInitialized()
        {
            messageHub.RegisterReceiver<PlayerCharacterSpawnedMsg>(this,SnowSetup);messageHub.RegisterReceiver<PlayerCharacterSpawnedMsg>(this,SnowSetup);
        }

        private void SnowSetup(PlayerCharacterSpawnedMsg obj)
        {
            player = localPlayer.PlayerCharacter.gameObject;
            gameStart = true;
        }

        void Update()
        {
            if (gameStart)
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
