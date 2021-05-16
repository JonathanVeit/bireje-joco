using UnityEngine;
using BiReJeJoCo.Backend;
using BiReJeJoCo.UI;
using System.Collections;

namespace BiReJeJoCo.Map
{
    public class TriggeredDoor : SynchronizedTrigger
    {
        private enum Direction
        {
            Right, 
            Forward,
        }

        [Header("Door Settings")]
        [SerializeField] GameObject door;
        [SerializeField] Direction direction;
        [SerializeField] float doorOffset; // closed -> open
        [SerializeField] float moveSpeed = 5;

        [Header("Runtime")]
        [SerializeField] private bool isOpen;

        private Vector3 targetDoorPosition;
        private bool doorIsMoving => Vector3.Distance(door.transform.position, targetDoorPosition) > 0.01f;

        protected override void SetupAsActive()
        {
            base.SetupAsActive();
            targetDoorPosition = door.transform.position;
        }

        protected override void OnTriggerInteracted(byte pointId)
        {
            if (doorIsMoving)
                return;

            isOpen = !isOpen;

            switch (direction)
            {
                case Direction.Right:
                    targetDoorPosition = door.transform.position + door.transform.right * (doorOffset * (isOpen ? -1 : 1));
                    break;
                case Direction.Forward:
                    targetDoorPosition = door.transform.position + door.transform.forward * (doorOffset * (isOpen ? -1 : 1));
                    break;
            }
        }
        public void Update()
        {
            if (doorIsMoving)
            {
                door.transform.position = Vector3.MoveTowards(door.transform.position, targetDoorPosition, moveSpeed * Time.deltaTime);
            }
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
            yield return new WaitUntil(() => !doorIsMoving);
            TryUnhideFloaty(trigger);
            trigger.isCoolingDown = false;
        }

        protected override void OnFloatySpawned(int pointId, InteractionFloaty floaty)
        {
           floaty.SetDescription("Door");
        }
    }
}
