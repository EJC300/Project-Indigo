using Aircraft;
using UnityEngine;
namespace AircraftData {

    [RequireComponent(typeof(Rigidbody))]
    public class Aerobody : MonoBehaviour
    {
        private Rigidbody rb; 
        [SerializeField] AircraftSpecifications aircraftSpecifications;
        private AircraftThrust thrust;

        private AerodynamicParameters aerodynamicParameters;


        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            thrust = GetComponent<AircraftThrust>();
            aerodynamicParameters = aircraftSpecifications.aerodynamicParameters;
            thrust.SetRigidBody(rb);
            rb.mass = aircraftSpecifications.aircraftMass;
            
            
       
           
      
        }

        private Vector3 AirCraftLinearVelocity()
        {
            return rb.linearVelocity;
        }
        private float AngleOfAttack()
        {
            return Mathf.Atan2(AirCraftLinearVelocity().y, AirCraftLinearVelocity().z) * Mathf.Rad2Deg;
        }
        float CalculateDrag()
        {
            float speedSquared = AirCraftLinearVelocity().sqrMagnitude;

            float halfSpeed = 0.5f * speedSquared;

            float dragAmountTimesMass = aerodynamicParameters.dragPower * rb.mass;

            return -halfSpeed * dragAmountTimesMass;

        }
        void ApplyDrag()
        {
            Vector3 flightDirection = AirCraftLinearVelocity().normalized;
            Vector3 dragForce = flightDirection * CalculateDrag();

            rb.AddForce(dragForce);
        }
        private void FixedUpdate()
        {
            ApplyDrag();
        }

    }
}
