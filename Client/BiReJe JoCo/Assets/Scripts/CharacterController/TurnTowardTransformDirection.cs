using UnityEngine;

namespace BiReJeJoCo.Character
{
	//This script rotates an object toward the 'forward' direction of another target transform;
	public class TurnTowardTransformDirection : TickBehaviour {

		[SerializeField] Transform targetTransform;
		[SerializeField] bool mainCamera;

		Transform tr;
		Transform parentTransform;

        //Setup;
        protected override void OnSystemsInitialized()
        {
			tr = transform;
			parentTransform = transform.parent;
			tickSystem.Register(this, "late_update");
			if (targetTransform != null) return;

			if (mainCamera)
			{
				targetTransform = Camera.main.transform;
				return;
			}

			Debug.LogWarning("No target transform has been assigned to this script.", this);
		}

        //Update;
        public override void Tick(float deltaTime)
        {
			if(!targetTransform)
				return;

			//Calculate up and forward direction;
			Vector3 _forwardDirection = Vector3.ProjectOnPlane(targetTransform.forward, parentTransform.up).normalized;
			Vector3 _upDirection = parentTransform.up;

			//Set rotation;
			tr.rotation = Quaternion.LookRotation(_forwardDirection, _upDirection);
		}
	}
}
