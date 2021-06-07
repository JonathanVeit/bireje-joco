using UnityEngine;
using BiReJeJoCo.Backend;
using BiReJeJoCo.UI;
using System.Collections;

namespace BiReJeJoCo.Map
{
    public class TriggeredDoor : SynchronizedTrigger
    {
        [Header("Door Settings")]
        private Animator anim;
        [SerializeField] float coolDownTime = 1;

        [Header("Runtime")]
        [SerializeField] private bool isOpen;

        private Vector3 targetDoorPosition;

        protected override void SetupAsActive()
        {
            base.SetupAsActive();
            anim = this.GetComponent<Animator>();
            anim.SetBool("isOpen", isOpen);
        }

        protected override void OnTriggerInteracted(byte pointId)
        {
            isOpen = !isOpen;
            anim.SetBool("isOpen", isOpen);
        }

        public void Update()
        {
            
        }

        protected override void OnSychronizedTriggerReceived(PhotonMessage msg)
        {
            var castedMsg = msg as TriggerPointInteractedPhoMsg;

            if (castedMsg.i == triggerId)
            {
                OnTriggerInteracted(castedMsg.pi);

                foreach(var curTrigger in triggerPoints)
                    StartCoroutine(CoolDown(curTrigger));
            }
        }

        
        protected override IEnumerator CoolDown(TriggerSetup trigger)
        {
            trigger.isCoolingDown = true;
            TryHideFloaty(trigger);
            yield return new WaitForSecondsRealtime(coolDownTime);
            TryUnhideFloaty(trigger);
            trigger.isCoolingDown = false;
        }
        

        protected override void OnFloatySpawned(int pointId, InteractionFloaty floaty)
        {
           floaty.SetDescription("Door");
        }
    }
}
