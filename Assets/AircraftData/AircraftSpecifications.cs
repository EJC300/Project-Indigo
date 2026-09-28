using UnityEngine;
using Aircraft;
namespace AircraftData {
    [CreateAssetMenu(fileName = "AircraftSpecifications", menuName = "Aircraft/AircraftSpecifications")]
    public class AircraftSpecifications : ScriptableObject
    {
        public float aircraftMass;
        public EngineParameters engineParameters;
        public AerodynamicParameters aerodynamicParameters;
        public ControlParameters controlParameters;
    }
}