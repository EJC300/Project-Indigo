using UnityEngine;
using AircraftData;
namespace Aircraft
{
    [RequireComponent (typeof(Rigidbody))]
    public class AircraftController : MonoBehaviour
    {
    private Rigidbody rb;
    [SerializeField] private AircraftSpecifications specifications;
        //Aircraft Thrust
        //Aircraft Control Authority
     [SerializeField] private AircraftThrust aircraftThrust;
   
     private Vector3 aircraftEngineThrustForce;
        private void Start()
        {
           rb= GetComponent<Rigidbody>();
           rb.mass = specifications.aircraftMass;
        }

      
        public void ApplyPitch(float pitch)
        {

     
        }

    public void ApplyYaw(float yaw) 
    { 
        
        
    }
     public void AppRoll(float roll) 
     {
      
      
     }

    public void ApplyThrottle(float thrust)
    {
        aircraftEngineThrustForce = aircraftThrust.ApplyThrust(thrust,specifications.engineParameters);
        Debug.Log(aircraftEngineThrustForce.ToString());
    }

    void ApplyAircraftForces()
    {
      Vector3 aircraftForces = aircraftEngineThrustForce;
      rb.AddRelativeForce(aircraftForces);
    }
        private void FixedUpdate()
        {
            ApplyAircraftForces();
        }

    }
}
