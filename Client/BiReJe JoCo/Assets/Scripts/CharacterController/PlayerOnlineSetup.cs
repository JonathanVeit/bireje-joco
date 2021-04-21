using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BiReJeJoCo.Character
{
    public class PlayerOnlineSetup : SystemBehaviour
    {
        [SerializeField] private GameObject cam;
        [SerializeField] private GameObject cinemachineObject;
        [SerializeField] private GameObject characterController;



        protected override void OnSystemsInitialized()
        {
            Setup();
        }

        void Setup()
        {
            //TODO
            //disables not needed components if not local player
            /*
            if (!localPlayer)
            {
                cam.SetActive(false);
                cinemachineObject.SetActive(false);
            }
            */
        }

    }

}
