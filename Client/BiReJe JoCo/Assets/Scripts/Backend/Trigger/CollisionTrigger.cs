using UnityEngine;
using System;
using System.Collections.Generic;

namespace BiReJeJoCo.Backend
{
    public class CollisionTrigger : SystemBehaviour
    {
        public event Action<GameObject> OnRemoteEntered;
        public event Action<GameObject> OnRemoteLeft;

        public event Action<GameObject> OnLocaEntered;
        public event Action<GameObject> OnLocalLeft;

        public List<GameObject> Remote { get; private set; }
            = new List<GameObject>();
        public GameObject Local { get; private set; }


        private void OnTriggerEnter(Collider collider)
        {
            HandleEntered(collider.gameObject);
        }

        private void OnTriggerExit(Collider collider)
        {
            HandleLeft(collider.gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            HandleEntered(collision.collider.gameObject);
        }

        private void OnCollisionExit(Collision collision)
        {
            HandleLeft(collision.collider.gameObject);
        }


        private void HandleEntered(GameObject go)
        {
            var controlled = go.GetComponentsInParent<PlayerControlled>();

            if (!controlled[0].Player.IsLocalPlayer)
            {
                Remote.Add(go);
                OnRemoteEntered?.Invoke(go);
            }
            else
            {
                Local = go;
                OnLocaEntered?.Invoke(go);
            }
        }

        private void HandleLeft(GameObject go)
        {
            var controlled = go.GetComponentsInParent<PlayerControlled>();

            if (!controlled[0].Player.IsLocalPlayer)
            {
                Remote.Remove(go);
                OnRemoteLeft?.Invoke(go);
            }
            else
            {
                Local = null;
                OnLocalLeft?.Invoke(go);
            }
        }
    }
}
