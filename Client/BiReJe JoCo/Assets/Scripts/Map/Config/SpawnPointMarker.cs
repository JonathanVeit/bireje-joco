using BiReJeJoCo.Backend;
using UnityEngine;

namespace BiReJeJoCo
{
    public class SpawnPointMarker : MonoBehaviour
    {
        public PlayerRole role;
        public bool isCollectable;

        public void OnDrawGizmos()
        {
            float radius = 0.1f;
            float boxHeight = 0.05f;

            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.position + new Vector3(0, boxHeight/2, 0), new Vector3(0.3f, boxHeight, 0.3f));
            
            if (isCollectable)
                Gizmos.color = Color.blue;
            else if (role == PlayerRole.Hunted)
                Gizmos.color = Color.yellow;
            else
                Gizmos.color = Color.red;

            Gizmos.DrawSphere(transform.position + new Vector3(0, boxHeight + radius/2, 0), radius);
        }
    }
}