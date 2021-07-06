using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BiReJeJoCo
{
    public class WaterLid : MonoBehaviour
    {
        [SerializeField] GameObject water;
        [SerializeField] GameObject floor;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("LocalPlayer"))
            {
                water.SetActive(false);
                floor.SetActive(false);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("LocalPlayer"))
            {
                water.SetActive(true);
                floor.SetActive(true);
            }
        }
    }
}
