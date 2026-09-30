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
        private ControlParameters controlParameters;
    
        private float yawControlAuthority;
        private float rollControlAuthority;
        private float AOA;
        private float stallTorque;
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            thrust = GetComponent<AircraftThrust>();
            aerodynamicParameters = aircraftSpecifications.aerodynamicParameters;
            controlParameters = aircraftSpecifications.controlParameters;
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

      
        void ApplyPitch()
        {
            float forwardVelocity = Vector3.Dot(LocalAircraftVelocity(),transform.forward);
            float forwardVelocityHalfSquared = (forwardVelocity * forwardVelocity) * 0.5f;
            float controlAuthority = forwardVelocityHalfSquared * controlParameters.pitchStrength;
            rb.AddRelativeTorque(Vector3.right * controlAuthority * Input.GetAxis("Horizontal"));
        }
        

        float CalculateLift()
        {
            float aoa = AngleOfAttack();

            float appliedAOA = Mathf.Clamp(aoa,-aerodynamicParameters.stallAngle,aerodynamicParameters.stallAngle);
             
            float speedSquared = AirCraftLinearVelocity().sqrMagnitude;

            float halfSpeed = 0.5f * speedSquared;
            float liftPowerTimesSpeed = halfSpeed * aerodynamicParameters.liftPower;
            float liftCoefficient = liftPowerTimesSpeed * appliedAOA;
            float maxLift = aerodynamicParameters.liftPower * rb.mass * 0.5f;

            liftCoefficient = Mathf.Clamp(liftCoefficient, -maxLift, maxLift);
            AOA = appliedAOA;
            return liftCoefficient;
        }
        float CalculateInducedDrag()
        {
            float liftCoef = Mathf.Sqrt( CalculateLift() * CalculateLift());
           ;
            float inducedDrag = liftCoef * aerodynamicParameters.inducedDragFactor;
        
            return inducedDrag;
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
            Vector3 inducedDragDirection = Vector3.Cross(liftDirection, transform.right).normalized;
            Vector3 inducedDragForce = inducedDragDirection * CalculateInducedDrag();
            rb.AddRelativeForce(inducedDragForce);
        }
        void ApplyLift()
        {
            if (LocalAircraftVelocity().z > 0)
            {


                Vector3 flightDirection = (AirCraftLinearVelocity()).normalized;
                Vector3 liftDirection = Vector3.Cross(flightDirection, -transform.right).normalized;
                float liftCoef = CalculateLift();

                Vector3 liftForce = liftDirection * liftCoef;

                rb.AddForce(liftForce);

                //Add Some Torque based on liftForce
                Vector3 liftTorque = Vector3.Cross(liftForce.normalized, transform.up);

                rb.AddTorque(liftTorque);
                stallTorque = liftCoef * 0.005f;
                Debug.Log(liftCoef);
                if (LocalAircraftVelocity().z < 25 & AOA< aerodynamicParameters.stallAngle)
                {
                    
                    //rb.AddTorque(-stallTorque * Vector3.right);
                }
            }
        }
        private void FixedUpdate()
        {
         
            ApplyDrag();
            ApplyInducedDrag();
            ApplyLift();
            ApplyPitch();
        }

    }
}
