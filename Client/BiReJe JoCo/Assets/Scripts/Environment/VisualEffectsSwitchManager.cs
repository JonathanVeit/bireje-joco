using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System;

namespace BiReJeJoCo
{
    public class VisualEffectsSwitchManager : SystemBehaviour
    {
        [Header("Upstairs Effects")]
        [SerializeField] GameObject upstairsPPS;
        Volume upstairsPPSVol;
        [SerializeField] Light upstairsMainLight;
        [SerializeField] GameObject upstairsFog;
        [SerializeField] GameObject snowEffects;
        [Space(10)]
        [Header("Downstairs Effects")]
        [SerializeField] GameObject downstairsPPS;
        Volume downstairsPPSVol;
        [SerializeField] Light downstairsMainLight;
        [SerializeField] GameObject downstairsFog;

        [Header("Hunted alterations")]
        [SerializeField] VolumeProfile huntedUpstairsPPSProfile;
        [SerializeField] VolumeProfile huntedDownstairsPPSProfile;

        bool isStart = true;
        bool characterSpawned = false;

        [SerializeField] List<GameObject> switchAreas = new List<GameObject>();

        protected override void OnSystemsInitialized()
        {
            base.OnSystemsInitialized();
            if (upstairsPPS && downstairsPPS)
            {
                upstairsPPSVol = upstairsPPS.GetComponent<Volume>();
                downstairsPPSVol = downstairsPPS.GetComponent<Volume>();
            }
            else 
            {
                Debug.LogError("PPS game objects have not been assigned correctly to " + this.gameObject);
            }

            messageHub.RegisterReceiver<PlayerCharacterSpawnedMsg>(this, OnCharacterSpawned);
        }

        private void OnCharacterSpawned(PlayerCharacterSpawnedMsg obj)
        {
            upstairsFog = localPlayer.PlayerCharacter.fogUpstairs;
            downstairsFog = localPlayer.PlayerCharacter.fogDownstairs;

            if (localPlayer.Role == Backend.PlayerRole.Hunted)
            {
                upstairsPPSVol.profile = huntedUpstairsPPSProfile;
                downstairsPPSVol.profile = huntedDownstairsPPSProfile;
            }

            characterSpawned = true;
        }

        private void StartPPS(bool _isUpstairs)
        {
            if (!characterSpawned) return;

            if (_isUpstairs)
            {
                //Enable all upstairs effects
                upstairsMainLight.gameObject.SetActive(true);
                upstairsFog.SetActive(true);
                snowEffects.SetActive(true);

                //Disable all downstairs effects
                downstairsMainLight.gameObject.SetActive(false);
                downstairsFog.SetActive(false);
            }
            else
            {
                //Disable all upstairs effects
                upstairsMainLight.gameObject.SetActive(false);
                upstairsFog.SetActive(false);
                snowEffects.SetActive(false);

                //Enable all downstairs effects
                downstairsMainLight.gameObject.SetActive(true);
                downstairsFog.SetActive(true);
            }
        }


        public void HandlePPSSwitch(bool _isUpstairs)
        {
            if (isStart)
            {
                StartPPS(_isUpstairs);
                isStart = false;
                return;
            }

            if (_isUpstairs)
            {
                HandleUpstairs();
            }
            else
            {
                HandleDownstairs();
            }
        }

        void HandleUpstairs()
        {
            //Enable all upstairs effects
            upstairsFog.SetActive(true);
            snowEffects.SetActive(true);

            //Disable all downstairs effects
            downstairsFog.SetActive(false);

            upstairsMainLight.gameObject.SetActive(false);
            //switch PPS
            StartCoroutine(SmoothLight(upstairsMainLight, downstairsMainLight));
        }

        void HandleDownstairs()
        {
            //Disable all upstairs effects
            upstairsFog.SetActive(false);
            snowEffects.SetActive(false);

            //Enable all downstairs effects
            downstairsFog.SetActive(true);

            downstairsMainLight.gameObject.SetActive(false);
            //switch lights
            StartCoroutine(SmoothLight(downstairsMainLight, upstairsMainLight));
        }

        IEnumerator SmoothLight(Light _lightOn, Light _lightOff)
        {
            float _lightOnIntensSave = _lightOn.intensity;
            float _lightOffIntensSave = _lightOff.intensity;
            Debug.Log("light on intensity :"+ _lightOn.intensity + "| light off intensity :" + _lightOff.intensity);

            _lightOn.intensity = 0;
            _lightOn.gameObject.SetActive(true);

            float startTime = Time.time;
            while (Time.time - startTime < 1f)
            {
                _lightOn.intensity += _lightOnIntensSave * Time.deltaTime;
                _lightOff.intensity -= _lightOffIntensSave * Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }

            _lightOn.intensity = _lightOnIntensSave;

            _lightOff.gameObject.SetActive(false);
            _lightOff.intensity = _lightOffIntensSave;
        }

        protected override void OnBeforeDestroy()
        {
            messageHub.UnregisterReceiver(this);
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;

            foreach (GameObject g in switchAreas)
            {
                Gizmos.DrawWireCube(g.transform.position, g.transform.localScale);
            }
        }

#endif
    }
}
