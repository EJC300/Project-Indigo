using UnityEngine;
using AircraftData;
namespace Aircraft
{
    public class AircraftThrust : MonoBehaviour
    {
        //Test Thrust Variables delete after testing complete
        [SerializeField] private Rigidbody rb;

        private float throttleAmount;
        private float targetThrustRatio;

        private float currentThrust;
        private float thrustRatio;

  
        public Vector3 ApplyThrust(float input,EngineParameters engineParameters)
        {
            
            throttleAmount = Mathf.Clamp01(throttleAmount + input * Time.fixedDeltaTime);
            float leverPosition = throttleAmount;

            if(leverPosition <= 0f)
            {
                targetThrustRatio = engineParameters. idleThrustRatio;
            }
            else if(leverPosition >= 0.90f)
            {
                float t = Mathf.InverseLerp(0.90f, 1.0f, leverPosition * Time.fixedDeltaTime);
                targetThrustRatio = Mathf.Lerp(targetThrustRatio, engineParameters.afterBurnerThrustRatio, t);

            }
            else
            {
                targetThrustRatio = leverPosition;
            }
          
            float targetThrustForEngine = targetThrustRatio * engineParameters.totalThrust;
           

            thrustRatio = Mathf.MoveTowards(thrustRatio,targetThrustForEngine, engineParameters.throttleSpeed * Time.fixedDeltaTime);
            
           

            currentThrust = Mathf.Lerp(currentThrust,thrustRatio, engineParameters.engineSpoolSpeed * Time.fixedDeltaTime);
          
            float appliedThrust = Mathf.Clamp(currentThrust, engineParameters.totalThrust * engineParameters.idleThrustRatio, engineParameters.totalThrust * engineParameters.afterBurnerThrustRatio);
            
            return Vector3.forward * appliedThrust;
        }
        


    }
}