using UnityEngine;
namespace AircraftData
{
    [System.Serializable]
    public class ControlParameters
    {
        public float pitchStrength, yawStrength, rollStrength;

        public float targetRates;

        public float brakeStrength;

        public float maxAOA;
    }
}
