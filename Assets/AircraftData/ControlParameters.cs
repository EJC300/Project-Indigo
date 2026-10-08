using UnityEngine;
namespace AircraftData
{
    [System.Serializable]
    public class ControlParameters
    {
        public float maxGlimit = 5.0f;

        public float minGlimit = 3.0f;

        public float pitchStrength, yawStrength, rollStrength;

        public float targetRates;

        public float brakeStrength;

        public float maxAOA;
    }
}
