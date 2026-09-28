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

        float CalculateLift()
        {
            float aoa = AngleOfAttack();

            float appliedAOA = Mathf.Clamp(aoa,-aerodynamicParameters.stallAngle,aerodynamicParameters.stallAngle);
             
            float speedSquared = AirCraftLinearVelocity().sqrMagnitude;

            float halfSpeed = 0.5f * speedSquared;
            float liftPowerTimesSpeed = halfSpeed * aerodynamicParameters.liftPower;
            float liftCoefficient = speedSquared * liftPowerTimesSpeed * appliedAOA;

            return liftCoefficient;
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
        void ApplyLift()
        {
            Vector3 flightDirection = ( AirCraftLinearVelocity()).normalized;
            Vector3 liftDirection = Vector3.Cross(flightDirection, transform.right).normalized;

            Vector3 liftForce = liftDirection * CalculateLift();

            rb.AddForce(liftForce);
        }
        private void FixedUpdate()
        {
            ApplyDrag();
            ApplyLift();
        }

    }
}
