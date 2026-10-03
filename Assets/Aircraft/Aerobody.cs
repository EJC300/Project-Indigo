using Aircraft;
using UnityEngine;
namespace AircraftData {

    [RequireComponent(typeof(Rigidbody))]
    public class Aerobody : MonoBehaviour
    {

        [SerializeField] AircraftSpecifications airSpecifications;

        private Rigidbody rb;
        private AerodynamicParameters aerodynamicParameters;

        private ControlParameters controlParameters;



        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.mass = airSpecifications.aircraftMass;
            controlParameters =airSpecifications.controlParameters;
            aerodynamicParameters = airSpecifications.aerodynamicParameters;
            
        }
      
        private Vector3 AeroVelocity()
        {
            return rb.linearVelocity;
        }
        
         float WingForces(float y,float z)
         {
            //Lift the forces with the 

            float speed = z * z + y * y;
            float pressure = 0.5f * speed;
            float AOA = Mathf.Atan2(y,z) * Mathf.Rad2Deg;
            AOA = Mathf.Clamp(AOA,-aerodynamicParameters.stallAngle, aerodynamicParameters.stallAngle);
            float AOAdelta = AOA / aerodynamicParameters.stallAngle;
           
           
           
            float cl =  -pressure * aerodynamicParameters.liftPower *AOAdelta;
            float lift = cl;
            
            return lift;
            
            
         }
        Vector3 GetVelocityAtWing(Vector3 offset)
        {
            return rb.GetPointVelocity(offset);
        }
        void ApplyLiftForces()
        {
            Vector3 leftVelocity =(GetVelocityAtWing(transform.InverseTransformPoint(new Vector3(0.5f, 0, 0.5f))));
            Vector3 rightVelocity = (GetVelocityAtWing(transform.InverseTransformPoint( new Vector3(-0.5f, 0, 0.5f))));
            Debug.Log(rightVelocity);
            Vector3 rightWing = transform.InverseTransformPoint(new Vector3(0.5f, 0, 0.5f));

            Vector3 leftWing = transform.InverseTransformPoint(new Vector3(-0.5f, 0, 0.5f));
            float liftLeftWing = WingForces(leftVelocity.y, leftVelocity.z);
            
            float liftRightWing = WingForces(leftVelocity.y,leftVelocity.z);

            float totalLift = liftLeftWing + liftRightWing;

            float maxLift =  rb.mass * Mathf.Abs(Physics.gravity.y);

            totalLift =Mathf.Clamp(totalLift,-maxLift,maxLift);
            Vector3 flightDirection =  (AeroVelocity()).normalized;
            Vector3 liftDirection = Vector3.Cross(flightDirection, transform.right).normalized;
            Debug.Log(totalLift);
            rb.AddForce(totalLift * liftDirection);
            rb.AddTorque(Vector3.Cross(totalLift * liftDirection, flightDirection).normalized);
        }
         void ApplyForces()
        {

            AeroDrag();
            ApplyLiftForces();
            
        }
        
        void AeroDrag()
        {
            float halfVelocitySquared = 0.5f * AeroVelocity().sqrMagnitude;

            Vector3 dragForce = -halfVelocitySquared * aerodynamicParameters.dragPower * AeroVelocity().normalized;
            dragForce *= rb.mass;
            rb.AddForce(dragForce);
        }

        private void FixedUpdate()
        {
            AeroDrag();
            ApplyForces();
        }
    }
}
