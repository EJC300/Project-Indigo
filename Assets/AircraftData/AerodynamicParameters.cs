using UnityEngine;
namespace AircraftData
{
    [System.Serializable]
    public class AerodynamicParameters
    {

        public AnimationCurve aoaCurve;
        public AnimationCurve dragCurve;
        public AnimationCurve inducedDragCurve;
        public AnimationCurve stallCurve;
        public float adverseYawFactor;

        public float dragPower;

        public float sideDragPower;

        public float liftPower;

        public float stallAngle;

        public float inducedDragPower;

      
    }
}
