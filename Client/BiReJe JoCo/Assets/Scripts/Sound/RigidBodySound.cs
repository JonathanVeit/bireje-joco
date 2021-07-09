using UnityEngine;

namespace BiReJeJoCo.Audio
{
    public class RigidBodySound : SystemBehaviour
    {
        [Header("Settings")]
        [SerializeField] string[] clips;
        [SerializeField] float minVelocity = 0.1f; 
        [SerializeField] float minDelay = 0.1f;
        [SerializeField] bool skipFirst;

        private Rigidbody rb;
        private int hitCount = 0;
        private float counter = 0;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            counter = minDelay;
        }
        private void Update()
        {
            counter += Time.deltaTime;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (rb == null)
                return;

            hitCount++;
            if (skipFirst && hitCount == 1)
                return;

            var vel = rb.velocity.magnitude;
            if (vel < minVelocity)
                return;

            if (counter < minDelay)
                return;

            soundEffectManager.Play(clips[Random.Range(0, clips.Length)], transform);
            counter = 0;
        }
    }
}