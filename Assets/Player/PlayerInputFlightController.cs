using UnityEngine;
using UnityEngine.InputSystem;
namespace Player
{
    public class PlayerInputFlightController : MonoBehaviour
    {
        public InputActionAsset inputAsset;

        private InputActionMap inputActions;

       
      


        [SerializeField] string pitchName;
        [SerializeField] string yawName;
        [SerializeField] string rollName;
        [SerializeField] string throttleName;
        private InputAction pitchAction;
        private InputAction rollAction;
        private InputAction yawAction;
        private InputAction throttleAction;
        private void Awake()
        {
            InputActionMap inputActions = inputAsset.FindActionMap("FlightControls");
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

        private void Update()
        {
        
            
                SetThrottle();
                RollInput();
                PitchInput();
                YawInput();
            
        }
     
        void SetThrottle()
        {
            float leverInput=  throttleAction.ReadValue<float>();
        
        }

        void RollInput()
        {
            float roll = rollAction.ReadValue<float>();
         
        }
        void PitchInput()
        {
            float pitch = pitchAction.ReadValue<float>();
        
        }

        void YawInput()
        {
            float yaw = yawAction.ReadValue<float>();

        }

    }
}
