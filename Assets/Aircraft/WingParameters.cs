using UnityEngine;
namespace AircraftData
{
    [System.Serializable]
    public class WingParameters
    {
        
        public Vector3 offset;
        public float durability;
        //Up wings right is tail
        public Vector3 liftAxis;
        //leave zero if not intended to control
        public float maxRotationAngle;

        public float liftAmount;

        public float maxStallAngle;
        //Used to calculate not only a specific drag to slow down 
        public float inducedDragAmount;


    }
}
