using UnityEngine;
using BiReJeJoCo.Backend;
using System;

namespace BiReJeJoCo.Map
{
    public class ElevatorSign : TickBehaviour
    {
        [Header("Settings")]
        [SerializeField] MeshRenderer upArrow;
        [SerializeField] MeshRenderer downArrow;
        [SerializeField] Material defaultMat;
        [SerializeField] Material greenMat;

        public void OnElevatorTargetSet(bool up) 
        {
            upArrow.material = up ? greenMat : defaultMat;
            downArrow.material = up ? defaultMat : greenMat;
        }

        public void Reset()
        {
            upArrow.material = defaultMat;
            downArrow.material = defaultMat;
        }
    }
}
