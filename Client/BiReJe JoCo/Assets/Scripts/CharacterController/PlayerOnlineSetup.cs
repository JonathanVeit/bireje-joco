using System.Collections.Generic;
using UnityEngine;
using BiReJeJoCo.Backend;

namespace BiReJeJoCo.Character
{
    public class PlayerOnlineSetup : SystemBehaviour
    {
        [SerializeField] private GameObject cam;
        [SerializeField] private GameObject cinemachineObject;
        [SerializeField] private Rigidbody rb;
        [SerializeField] List<MonoBehaviour> componentsToDisable;

        protected override void OnSystemsInitialized()
        {
            Setup();
        }

        void Setup()
        {
            if (!GetComponent<PlayerControlled> ().Player.IsLocalPlayer)
            {
                cam.SetActive(false);
                cinemachineObject.SetActive(false);

                foreach(var curComponent in componentsToDisable)
                    curComponent.enabled = false;
                rb.isKinematic = true;
            }
        }
    }

}
