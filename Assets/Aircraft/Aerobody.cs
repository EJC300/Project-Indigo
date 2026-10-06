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
            return  rb.linearVelocity;
        }
        void StallForces(float aoa,float z,float y,float lift)
        {float speed = z * z + y * y;
            float stallDrag = speed * Mathf.Sign(aoa);
           
            if (aoa >= aerodynamicParameters.stallAngle-1 && z < 255 )
            {
                Debug.Log(stallDrag);
                rb.AddRelativeForce(-rb.linearVelocity.normalized * stallDrag);
                Vector3 direction = Vector3.Cross(transform.forward, Physics.gravity).normalized;
                rb.AddRelativeTorque(direction * rb.mass);
            }
        }
         float WingForces(float y,float z)
         {
            //Lift the forces with the 

            float speed = z * z + y * y;
          
            float pressure = 0.5f * speed;
            float AOA = Mathf.Atan2(y,z) * Mathf.Rad2Deg;
            AOA = Mathf.Clamp(AOA,-aerodynamicParameters.stallAngle, aerodynamicParameters.stallAngle);
            float AOAdelta = AOA / aerodynamicParameters.stallAngle;
            Debug.Log(AOAdelta);


            float cl =  pressure * aerodynamicParameters.liftPower;
            float lift = cl;
            StallForces(AOA, z, y, cl);
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
            
            
            float liftLeftWing = WingForces(leftVelocity.y, leftVelocity.z);
            
            float liftRightWing = WingForces(rightVelocity.y, rightVelocity.z);

            float totalLift = liftLeftWing + liftRightWing;

            float maxLift = rb.mass;

            totalLift =Mathf.Clamp(totalLift,0,maxLift);
            Vector3 flightDirection =  (AeroVelocity()).normalized;
            Vector3 liftDirection = Vector3.Cross(flightDirection, transform.right).normalized;
            Debug.DrawRay(transform.position, liftDirection * totalLift);
            Debug.Log(totalLift);
            rb.AddForce(totalLift * liftDirection);
            rb.AddTorque(Vector3.Cross(liftDirection.normalized, -flightDirection) * totalLift );
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
