using BiReJeJoCo.Backend;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JoVei.Base.Helper;

namespace BiReJeJoCo.Items
{
    public class ShockGun : SystemBehaviour, IPlayerObserved
    {
        [Header("Settings")]
        [SerializeField] LineRenderer lineRenderer;
        [SerializeField] ParticleSystem lineParticleSystem;
        [SerializeField] ParticleSystem hitParticleSystem;
        [SerializeField] ParticleSystem damageParticleSystem;
        [SerializeField] Transform rayOrigin;
        [SerializeField] float trailHeight;

        [Header("Appearance")]
        [SerializeField] [Range(.1f, 5f)] float density;
        [SerializeField] Counter frequence;
        [SerializeField] [Range(0f, 2f)] float widthVariance;
        [SerializeField] float minWidth;
        [SerializeField] float maxWidth;
        [SerializeField] LayerMask hitLayerMask;
        [SerializeField] LayerMask huntedLayerMask;

        [Header("Noise")]
        [SerializeField] float noise;
        [SerializeField] int bigNoiseChance;
        [SerializeField] float bigNoise;

        public Transform RayOrigin => rayOrigin;
        private Vector3? currentTarget;
        private TrailRenderer currentTrail;

        private PlayerControlled controller;
        public Player Owner => controller.Player;

        public void Initialize(PlayerControlled controller)
        {
            this.controller = controller;

            if (Owner.IsLocalPlayer)
            {

            }
            //else
            //    playerShot.OnValueReceived += OnShotFired;
        }

        #region Public Methods 
        public void Shoot(Vector3? target)
        {
            currentTarget = target;
        }
        #endregion

        #region Ray 
        void Update() 
        {
            if (currentTarget.HasValue)
            {
                lineRenderer.enabled = true;
                lineParticleSystem.enableEmission = true;

                frequence.CountUp(() => 
                {
                    RefreshRay(currentTarget.Value);
                });
                UpdateSFX();
            }
            else
            {
                lineRenderer.enabled = false;
                lineParticleSystem.enableEmission = false;
                hitParticleSystem.enableEmission = false;
                damageParticleSystem.enableEmission = false;
                DestroyCurrentTrail();
                frequence.SetValue(frequence.MaxValue);
            }
        }

        private void RefreshRay(Vector3 targetPosition)
        {
            var dist = Vector3.Distance(rayOrigin.position, targetPosition);
            var direction = (targetPosition - rayOrigin.position).normalized;
            int amount = Mathf.Clamp(Mathf.FloorToInt(dist * density), 1, int.MaxValue);
            var pointLength = dist / amount;

            List<Vector3> totalPoints = CalculatePoints(targetPosition, direction, amount, pointLength);
            CreateMesh(totalPoints);
            RandomizeWidth();

            lineRenderer.positionCount = totalPoints.Count;
            lineRenderer.SetPositions(totalPoints.ToArray());
        }

        private List<Vector3> CalculatePoints(Vector3 targetPosition, Vector3 direction, int amount, float pointLength)
        {
            List<Vector3> result = new List<Vector3>();
            for (int i = 0; i < amount; i++)
            {
                var currentPoint = rayOrigin.position + (i * pointLength * direction);
                if (i > 1)
                {
                    currentPoint += CreateNoise();
                }

                result.Add(currentPoint);
            }
            result.Add(targetPosition);

            return result;
        }
        
        private Vector3 CreateNoise()
        {
            var newNoise = new Vector3(Random.Range(-noise, noise), Random.Range(-noise, noise), Random.Range(-noise, noise));
            if (Random.Range(0, bigNoiseChance) == 0)
                newNoise *= Random.Range(-bigNoise, bigNoise);

            return newNoise;
        }
        private void CreateMesh(List<Vector3> pointsToTarget)
        {
           
            var sh = lineParticleSystem.shape;
            var mesh = new Mesh()
            {
                vertices = pointsToTarget.Select(x => rayOrigin.InverseTransformPoint(x)).ToArray(),
            };

            sh.mesh = mesh;
        }
        private void RandomizeWidth()
        {
            var keyFrames = new List<Keyframe>();
            var amount = 10 * widthVariance;

            for (int i = 0; i < amount; i++)
            {
                var position = Mathf.InverseLerp(0, 10 * widthVariance, i);
                var value = Random.Range(minWidth, maxWidth);
                if (i == amount - 1)
                {
                    value = 0;
                }
           
                var key = new Keyframe(position, value);
                keyFrames.Add(key);
            }

            var curve = lineRenderer.widthCurve;
            curve.keys = keyFrames.ToArray();
            lineRenderer.widthCurve = curve;
        }

        private void UpdateSFX()
        {
            UpdateHitSFX();
            UpdateDamageSFX();
        }

        private void UpdateHitSFX()
        {
            RaycastHit hit;
            var dir = currentTarget.Value - rayOrigin.position;

            if (Physics.Raycast(rayOrigin.position, dir, out hit, dir.magnitude + 0.1f, hitLayerMask))
            {
                var point = hit.point;
                point += hit.normal * trailHeight;

                hitParticleSystem.enableEmission = true;
                hitParticleSystem.transform.position = point;

                if (!currentTrail)
                {
                    currentTrail = SpawnNewTrail(point);
                    currentTrail.SetPositions(new Vector3[1] { point });
                }
                currentTrail.transform.position = point;
            }
            else
            {
                hitParticleSystem.enableEmission = false;
                DestroyCurrentTrail();
            }
        }
        private void UpdateDamageSFX()
        {
            if (Physics.CheckSphere(currentTarget.Value, 0.1f, huntedLayerMask))
            {
                damageParticleSystem.enableEmission = true;
                damageParticleSystem.transform.position = currentTarget.Value;
            }
            else
            {
                damageParticleSystem.enableEmission = false;
            }
        }

        private TrailRenderer SpawnNewTrail(Vector3 position) 
        {
            var prefab = MatchPrefabMapping.GetMapping().GetElementForKey("shock_gun_trail");
            var instance = poolingManager.PoolInstance(prefab, position, Quaternion.identity);
            var trailRenderer = instance.GetComponent<TrailRenderer>();
            trailRenderer.emitting = true;
            return trailRenderer;
        }
        private void DestroyCurrentTrail()
        {
            if (currentTrail)
            {
                currentTrail.emitting = false;
                currentTrail = null;
            }
        }
        #endregion
    }
}