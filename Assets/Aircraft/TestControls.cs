using UnityEngine;
namespace Aircraft
{
    public class TestControls : MonoBehaviour
    {
        //Aircraft Controller
       public AircraftController controller;
       void ControlPitch()
        {

        }
        void ControlRoll()
        {

        }

        void ControlYaw()
        {

        }

        void ControlThrust()
        {
            controller.ApplyThrottle(Input.GetAxis("Vertical"));
        }

      
        private void Update()
        {
            ControlPitch();
            ControlRoll();
            ControlYaw();
            ControlThrust();
        }

    }
}
