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
        private Vector3 LocalAircraftVelocity()
        {
            return transform.InverseTransformDirection(rb.linearVelocity);
        }
       
        private float AngleOfAttack()
        {
            return Mathf.Atan2(LocalAircraftVelocity().y, LocalAircraftVelocity().z) * Mathf.Rad2Deg;
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
        float CalculateInducedDrag()
        {
            float liftCoef = CalculateLift();
            float liftSquared = CalculateLift() * CalculateLift();
            float inducedDrag = liftSquared * aerodynamicParameters.inducedDragFactor;

            return -inducedDrag;
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
        void ApplyInducedDrag()
        {
            Vector3 flightDirection = (AirCraftLinearVelocity()).normalized;
            float inducedDragCoef = CalculateInducedDrag();
            Vector3 liftDirection = Vector3.Cross(flightDirection, transform.right).normalized;
            Vector3 inducedDragDirection = Vector3.Cross(liftDirection, transform.up);
            Vector3 inducedDragForce = inducedDragDirection * CalculateInducedDrag();
            rb.AddRelativeForce(inducedDragForce);
        }
        void ApplyLift()
        {
            Vector3 flightDirection = ( AirCraftLinearVelocity()).normalized;
            Vector3 liftDirection = Vector3.Cross(flightDirection, transform.right).normalized;
            float liftCoef = CalculateLift();
            
            Vector3 liftForce = liftDirection * liftCoef;

            rb.AddForce(liftForce);
            //Add Some Torque based on liftForce
            Vector3 liftTorque = Vector3.Cross(liftForce, LocalAircraftVelocity().normalized);
            Debug.Log(liftTorque);
            rb.AddTorque(liftTorque);
        }
        private void FixedUpdate()
        {
            ApplyDrag();
            ApplyInducedDrag();
            ApplyLift();
            
        }

    }
}
