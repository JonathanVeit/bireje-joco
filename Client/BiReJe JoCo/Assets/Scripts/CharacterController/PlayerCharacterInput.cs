using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BiReJeJoCo.Character
{
    public class PlayerCharacterInput : MonoBehaviour
    {
        private Vector2 moveInput;
        private Vector2 lookInput;
        private bool jump;

        //Thoughts
        //key action  .started .performed .canceled

        #region Set Input (PlayerInput Component)
        // assigns the new input system values to a vector and gives that vector back
        // call Player_Input.MovementInput to get a Vector3 
        public void SetMovementInput(InputAction.CallbackContext inputValue)
        {
            Vector2 inputMovement = inputValue.ReadValue<Vector2>();
            moveInput = inputMovement;
        }

        public void SetLookInput(InputAction.CallbackContext inputValue)
        {
            Vector2 inputMovement = inputValue.ReadValue<Vector2>();
            lookInput = inputMovement;
        }

        public void SetJumpInput(InputAction.CallbackContext inputValue)
        {
            if (inputValue.started)
            {
                jump = true;
            }
            else
            {
                jump = false;
            }
        }

        #endregion

        public void AdjustSensitivityController()
        {
            //TODO adjust camera sensitivity for controller
        }

        #region Get Movement Input
        public Vector2 GetMovementInput()
        {
            return moveInput;
        }

        public float GetHorizontalMovementInput()
        {
            return moveInput.x;
        }

        public float GetVerticalMovementInput()
        {
            return moveInput.y;
        }
		#endregion

		#region Get Look Input
		public Vector2 GetLookInput()
        {
            return lookInput;
        }
        public float GetHorizontalLookInput()
        {
            return lookInput.x;
        }

        public float GetVerticalLookInput()
        {
            return lookInput.y;
        }
		#endregion

        public bool GetJumpInput()
        {
            return jump;
        }
	}
}

