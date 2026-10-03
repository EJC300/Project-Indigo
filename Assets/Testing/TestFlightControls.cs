using UnityEngine;
namespace Testing
{
    public class TestFlightControls : MonoBehaviour
    {
        public LiftTest testAircraft;
       public void ControlFlightSurfaces()
        {
            float pitchControl = Input.GetAxis("Vertical");
            float roll = Input.GetAxis("Horizontal");
            
            testAircraft.DirectTorque(pitchControl,roll);
           
            
        }

        private void FixedUpdate()
        {
           ControlFlightSurfaces();
        }
    }
}
