using AircraftData;
using UnityEngine;
namespace Aircraft
{
    public class AircraftWing
    {
        private ControlledWingParameters wing;

        public ControlledWingParameters getWing { get { return wing; } }
       
        public void SetControlledWing(ControlledWingParameters wingToSet)
        {
            wingToSet = wing;
        }
        
        public float CalculateLift(float Velocity)
        {
            return 0;
        }

        public float CalculateInducedDrag()
        {
            return 0;
        }
    }
}
