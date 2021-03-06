using UnityEngine;
using BiReJeJoCo.Backend;
using BiReJeJoCo.UI;
using System.Collections;

namespace BiReJeJoCo.Map
{
    public class TriggeredDoor : SynchronizedTrigger
    {
        [Header("Door Settings")]
        [SerializeField] Animator anim;
        [SerializeField] float coolDownTime = 1;

        [Header("Runtime")]
        [SerializeField] private bool isOpen;

        protected override void SetupAsActive()
        {
            base.SetupAsActive();
            anim.SetBool("isOpen", isOpen);
        }

        protected override void OnTriggerInteracted(byte pointId)
        {
            isOpen = !isOpen;
            anim.SetBool("isOpen", isOpen);
        }

        protected override void OnSychronizedTriggerReceived(PhotonMessage msg)
        {
            var castedMsg = msg as TriggerPointInteractedPhoMsg;

            if (castedMsg.i == triggerId)
            {
                OnTriggerInteracted(castedMsg.ti);

                foreach(var curTrigger in triggerPoints)
                    StartCoroutine(CoolDown(curTrigger));
            }
        }

        
        protected override IEnumerator CoolDown(TriggerSetup trigger)
        {
            trigger.isCoolingDown = true;
            DestroyTriggerFloaty(trigger); 
            yield return new WaitForSecondsRealtime(coolDownTime);
            trigger.isCoolingDown = false;


            if (trigger.Id == 2 &&
                floaties.ContainsKey(trigger.Id) && 
                floaties[trigger.Id] != null)
                floaties[trigger.Id].SetDescription(isOpen ? "Close Door" : "Open Door");
        }
        

        protected override void OnFloatySpawned(int pointId, InteractionFloaty floaty)
        {
           floaty.SetDescription(isOpen? "Close Door" : "Open Door");
        }
    }
}