using BiReJeJoCo.Character;
using System;
using UnityEngine;

namespace BiReJeJoCo.Backend
{
	public class SynchedAnimator : TickBehaviour, IPlayerObserved
	{
		[Header("Settings")]
		[SerializeField] Animator anim;
		[SerializeField] float defaultMoveSpeed;
		[SerializeField] float moveSpeedSmoothness;
		[SerializeField] AnimationEventCatcher eventCatcher;

		private IVelocitySource velocitySource;
		private float curMoveSpeed;
		private SyncVar<string> syncedTrigger = new SyncVar<string>(9, true);
		private SyncVar<string[]> syncedFloat = new SyncVar<string[]>(10, true);

		public event Action onPlaceSporesFinished;

		private PlayerControlled controller;
		public Player Owner => controller.Player;

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
			var vel = velocitySource.GetVelocity();
			vel.y = 0;
			return vel.magnitude;
		}

        #region Animator Parameters
        public void SetTrigger(string trigger)
		{
			syncedTrigger.SetValue(trigger);
			OnTriggerReceived(syncedTrigger.GetValue());
		}
		private void OnTriggerReceived(string trigger)
		{
			anim.ResetTrigger(trigger);
			anim.SetTrigger(trigger);
		}

		public void SetFloat(string name, float value)
		{
			syncedFloat.SetValue(new string[2] { name, value.ToString() });
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
			switch (args) 
			{
				case "place_corals_finished":
					onPlaceSporesFinished?.Invoke();
					break;
			}
		}
		#endregion
	}
}
