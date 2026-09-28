using UnityEngine;
namespace AircraftData
{
    [System.Serializable]
    public class AerodynamicParameters
    {
        public float adverseYawFactor;

        public float dragPower;

        public float sideDragPower;

        public float liftPower;

        public float stallAngle;

        public float postStallLiftFraction;

        public float stallSoftness;

        public float inducedDragFactor;

        public float maxLift;
    }
}
