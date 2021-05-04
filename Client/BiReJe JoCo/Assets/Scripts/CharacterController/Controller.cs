﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BiReJeJoCo.Character
{
	//This abstract class is the base for all other controller components (such as 'AdvancedWalkerController');
	//It can be extended to create a custom controller class;
	public abstract class Controller : TickBehaviour {

		//Getters;
		public abstract Vector3 GetVelocity();
		public abstract Vector3 GetMovementVelocity();
		public abstract bool IsGrounded();

		//Events;
		public delegate void VectorEvent(Vector3 v);
		public VectorEvent OnJump;
		public VectorEvent OnLand;

	}
}
