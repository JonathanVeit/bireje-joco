using UnityEngine;

namespace BiReJeJoCo.Character
{
    public class CameraSwitch : SystemBehaviour
    {
        [Header("Camera Setup")]
        [SerializeField] GameObject cam;
        [SerializeField] GameObject firstPersonCameraTransform;
        [SerializeField] GameObject thirdPersonRig;

        [Header("Model Setup")]
        [SerializeField] GameObject thirdPersonModelRoot;
        [SerializeField] GameObject firstPersonModelRoot;

        private PlayerCharacterInput characterInput;

        private GameObject playerRoot;

        public bool isFirstPerson = false;

        #region Initialization
        protected override void OnSystemsInitialized()
        {
            characterInput = this.GetComponent<PlayerCharacterInput>();
            characterInput.onCameraSwitchPressed += HandleCameraSwitch;
            playerRoot = this.gameObject.transform.parent.gameObject;

            ConnetEvents();
            CameraThirdPersonSetup();
        }
        protected override void OnBeforeDestroy()
        {
            DisconnectEvents();
            if (characterInput)
                characterInput.onCameraSwitchPressed -= HandleCameraSwitch;
        }

        private void ConnetEvents() 
        {
        }
        private void DisconnectEvents() 
        {
        }
        #endregion

        private void HandleCameraSwitch()
        {
            if (isFirstPerson)
            {
                CameraThirdPersonSetup();
            }
            else if (!isFirstPerson)
            {
                CameraFirstPersonSetup();
            }
        }

        private void CameraFirstPersonSetup()
        {
            cam.transform.parent = firstPersonCameraTransform.transform;
            cam.transform.position = firstPersonCameraTransform.transform.position;
            cam.transform.rotation = firstPersonCameraTransform.transform.rotation;
            thirdPersonRig.SetActive(false);

            //model visibility
            firstPersonModelRoot.SetActive(true);
            thirdPersonModelRoot.SetActive(false);

            isFirstPerson = true;
        }

        private void CameraThirdPersonSetup()
        {
            cam.transform.parent = playerRoot.transform;
            thirdPersonRig.SetActive(true);

            //model visibility
            firstPersonModelRoot.SetActive(false);
            thirdPersonModelRoot.SetActive(true);

            isFirstPerson = false;
        }

        public void CameraTransformSetup()
        {
            //model visibility
            firstPersonModelRoot.SetActive(false);
            thirdPersonModelRoot.SetActive(false);
        }

        public void CameraTransformReset()
        {
            if (isFirstPerson)
            {
                CameraThirdPersonSetup();    
            }
            else
            {
                CameraThirdPersonSetup();
            }
        }
    }
}
