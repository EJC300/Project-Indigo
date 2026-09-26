using UnityEngine;
namespace AircraftData
{
    [System.Serializable]
    public class EngineParameters
    {

        public float totalThrust;
        public float engineSpoolSpeed;
        public float throttleSpeed;
        public float afterBurnerThrustRatio;
        public float idleThrustRatio;
    }
}
