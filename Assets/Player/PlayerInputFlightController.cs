using Aircraft;
using GameInfrastructure;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Player
{
    public class PlayerInputFlightController : MonoBehaviour
    {
        public InputActionAsset inputAsset;

        private InputActionMap inputActions;

        public GameObject pauseWindow;

        [SerializeField] private PlaneController aircraftController;

        [SerializeField] private ThirdPersonCamera thirdPersonCamera;

        [SerializeField] string lookName;
        [SerializeField] string pitchName;
        [SerializeField] string yawName;
        [SerializeField] string rollName;
        [SerializeField] string throttleName;
        [SerializeField] InputAction lookAction;
        private InputAction pitchAction;
        private InputAction rollAction;
        private InputAction yawAction;
        private InputAction throttleAction;
        private bool playerDead;
        private void Awake()
        {
            InputActionMap inputActions = inputAsset.FindActionMap("FlightControls");
            lookAction = inputActions.FindAction(lookName);
            pitchAction = inputActions.FindAction(pitchName);
            yawAction = inputActions.FindAction(yawName);
            rollAction = inputActions.FindAction(rollName);
            throttleAction = inputActions.FindAction(throttleName);
        }

        public void OnEnable()
        {
            InputActionMap inputActions = inputAsset.FindActionMap("FlightControls");
            inputActions.Enable();
        }
        public void OnDisable()
        {

           
            
        }
        private void LateUpdate()
        {
         
        }
        private void Update()
        {
            if (aircraftController != null)
            {
                SetLookAxis();
               
            }
            else
            {
                playerDead = true;
            }
             if (playerDead)
            {
                playerDead = false;
                pauseWindow.gameObject.SetActive(true);
            }
        
        }
        private void FixedUpdate()
        {
            if (aircraftController != null)
            {

                SetThrottle();
                RollInput();
                PitchInput();
                YawInput();
            }
        }
        
        void SetLookAxis()
        {
          Vector3 lookAxis = lookAction.ReadValue<Vector2>();
       

           Vector3 axis = new Vector3(lookAxis.y, lookAxis.x, 0);
           
           thirdPersonCamera.LookCamera(axis);
        }
        void SetThrottle()
        {
            float throttleInput=  throttleAction.ReadValue<float>();
           
            aircraftController.SetThrottle(throttleInput);
        }

        void RollInput()
        {
            float roll = -rollAction.ReadValue<float>();
            aircraftController.SetRoll(roll);
         
        }
        void PitchInput()
        {
            float pitch = pitchAction.ReadValue<float>();
            aircraftController.SetPitch(pitch);
        }

        void YawInput()
        {
            float yaw = yawAction.ReadValue<float>();
           aircraftController.SetYaw(yaw);

        }

    }
}
