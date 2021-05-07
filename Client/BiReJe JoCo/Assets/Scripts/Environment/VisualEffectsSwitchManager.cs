using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System;
using BiReJeJoCo.Character;

namespace BiReJeJoCo
{
    public class VisualEffectsSwitchManager : SystemBehaviour
    {
        bool isUpstairs;

        [SerializeField] GameObject upstairsPPS;
        Volume upstairsPPSVol;
        [SerializeField] GameObject upstairsMainLight;
        [SerializeField] GameObject upstairsFog;
        [SerializeField] GameObject snowEffects;
        [Space(10)]
        [SerializeField] GameObject downstairsPPS;
        Volume downstairsPPSVol;
        [SerializeField] GameObject downstairsMainLight;
        [SerializeField] GameObject downstairsFog;

        public event Action onGoingDown;
        public event Action onGoingUp;

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

            messageHub.RegisterReceiver<PPSUpstairsMsg>(this, HandleGoingUpTriggered);
            messageHub.RegisterReceiver<PPSDownstairsMsg>(this, HandleGoingDownTriggered);
            messageHub.RegisterReceiver<PlayerCharacterSpawnedMsg>(this, VisualEffectsSetup);
        }

        private void VisualEffectsSetup(PlayerCharacterSpawnedMsg obj)
        {
            switch (localPlayer.Role)
            {
                case Backend.PlayerRole.Hunted:

                    upstairsFog = localPlayer.PlayerCharacter.GetComponent<CharacterOnlineSetup>().fogUpstairsHunted;
                    downstairsFog = localPlayer.PlayerCharacter.GetComponent<CharacterOnlineSetup>().fogDownstairsHunted;

                    break;
                case Backend.PlayerRole.Hunter:

                    upstairsFog = localPlayer.PlayerCharacter.GetComponent<CharacterOnlineSetup>().fogUpstairs;
                    downstairsFog = localPlayer.PlayerCharacter.GetComponent<CharacterOnlineSetup>().fogDownstairs;

                    break;
            }
        }

        protected virtual void HandleGoingUpTriggered(PPSUpstairsMsg msg)
        {
            if (isUpstairs)
                return;

            //Enable all upstairs effects
            upstairsMainLight.SetActive(true);
            upstairsFog.SetActive(true);
            snowEffects.SetActive(true);

            //Disable all downstairs effects
            downstairsMainLight.SetActive(false);
            downstairsFog.SetActive(false);

            //switch PPS
            StartCoroutine(SmoothPPSWeight(upstairsPPSVol, downstairsPPSVol));
            downstairsPPSVol.weight = 1f;
            upstairsPPSVol.weight = 0f;
            isUpstairs = true;
        }

        protected virtual void HandleGoingDownTriggered(PPSDownstairsMsg msg)
        {
            if (!isUpstairs)
                return;

            //Disable all upstairs effects
            upstairsMainLight.SetActive(false);
            upstairsFog.SetActive(false);
            snowEffects.SetActive(false);

            //Enable all downstairs effects
            downstairsMainLight.SetActive(true);
            downstairsFog.SetActive(true);

            //switch PPS
            StartCoroutine(SmoothPPSWeight(downstairsPPSVol, upstairsPPSVol));
            downstairsPPSVol.weight = 0f;
            upstairsPPSVol.weight = 1f;
            isUpstairs = false;
        }

        IEnumerator SmoothPPSWeight(Volume _ppsVolOn, Volume _ppsVolOff)
        {
            float startTime = Time.time;
            while (Time.time - startTime < 1f)
            {
                _ppsVolOn.weight += 1 * Time.deltaTime;
                _ppsVolOff.weight -= 1 * Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
        }

        private void OnDestroy()
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
