using BiReJeJoCo.Character;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace BiReJeJoCo.Backend
{
	public class SynchedAnimator : TickBehaviour, IPlayerObserved
	{
		[Header("Settings")]
		[SerializeField] Animator anim;
		[SerializeField] float defaultMoveSpeed;
		[SerializeField] float moveSpeedSmoothness;
		[SerializeField] TriggerReset[] triggerResets;
		[SerializeField] AnimationEventCatcher eventCatcher;

		private IMovementVelocitySource velocitySource;
		private float curMoveSpeed;
		private SyncVar<string> syncedTrigger = new SyncVar<string>(9, true);
		private SyncVar<string[]> syncedFloat = new SyncVar<string[]>(10, true);

		public event Action<string> onAnimationEvent;

		private PlayerControlled controller;
		public Player Owner => controller.Player;

		private List<string> blockedParameters
			= new List<string>();

		#region Initialization
		public void Initialize(PlayerControlled controller)
		{
			this.controller = controller;

			if (Owner.IsLocalPlayer)
			{
				velocitySource = Owner.PlayerCharacter.ControllerSetup.WalkController;
				eventCatcher.onAnimationEventTriggered += OnAnimationEventTriggered;
			}
			else
			{
				velocitySource = Owner.PlayerCharacter.SyncedTransform;
				syncedTrigger.OnValueReceived += OnTriggerReceived;
				syncedFloat.OnValueReceived += OnFloatReceived;
			}
		}

        protected override void OnBeforeDestroy()
        {
			if (syncVarHub)
			{
				syncVarHub.UnregisterSyncVar(syncedTrigger);
				syncVarHub.UnregisterSyncVar(syncedFloat);
			}
			tickSystem.Unregister(this);
        }
        #endregion

        public override void Tick(float deltaTime)
        {
			CalculateMoveSpeed(deltaTime);
			anim.SetFloat("move_speed", curMoveSpeed);
        }

		private void CalculateMoveSpeed(float deltaTime)
		{
			var magnitude = GetTransformedMagnitude();
			var targetSped = magnitude / defaultMoveSpeed;
			curMoveSpeed = Mathf.Lerp(curMoveSpeed, targetSped, moveSpeedSmoothness * deltaTime);
		}
		private float GetTransformedMagnitude() 
		{
			var vel = velocitySource.GetMovementVelocity();
			vel.y = 0;
			var magnitude = vel.magnitude;

			return magnitude;
		}

		#region Animator Parameters
		public void BlockParameters(params string[] names)
		{
			foreach (var entry in names)
			{
				if (!blockedParameters.Contains(entry))
					blockedParameters.Add(entry);
			}
		}

		public void UnblockParameters(params string[] names)
		{
			foreach (var entry in names)
			{
				if (blockedParameters.Contains(entry))
					blockedParameters.Remove(entry);
			}
		}

        public void SetTrigger(string name)
		{
			if (blockedParameters.Contains(name))
				return;

			syncedTrigger.SetValue(name);
			syncedTrigger.ForceSend();
			OnTriggerReceived(syncedTrigger.GetValue());
		}
		private void OnTriggerReceived(string trigger)
		{
			foreach (var entry in triggerResets)
			{
				if (entry.triggerName == trigger)
				{
					foreach (var curTrigger in entry.resetTrigger)
						anim.ResetTrigger(curTrigger);
				}
			}

			anim.ResetTrigger(trigger);
			anim.SetTrigger(trigger);
		}

		public void SetFloat(string name, float value)
		{
			if (blockedParameters.Contains(name))
				return;

			syncedFloat.SetValue(new string[2] { name, value.ToString() });
			syncedFloat.ForceSend();
			OnFloatReceived(syncedFloat.GetValue());
		}
		private void OnFloatReceived(string[] parameters)
		{
			string name = parameters[0];
			float value = float.Parse(parameters[1]);

			anim.SetFloat(name, value);
		}
		#endregion

		#region Events
		private void OnAnimationEventTriggered(string args)
		{
			onAnimationEvent?.Invoke(args);
		}
		#endregion

		#region Helper
		[Serializable]
		struct TriggerReset
		{
			public string triggerName;
			public string[] resetTrigger;
		}
		#endregion
    }
}