using Aircraft;
using UnityEngine;
namespace AircraftData
{

    [RequireComponent(typeof(Rigidbody))]
    public class PlaneController : MonoBehaviour
    {

        /*
         * 
         * I was pressed for time so I had to either use a more arcade flight model but still has stuff like induced drag(airspeed bleed if the aircraft makes sharp turns at low speeds) angle of attack (how much the nose is pitching to relative wingspeed in this case forward direciton)
         * I originally wanted a full wing simulation that had localized forces on wings and torn off wings effected flight. Maybe later?
         */


        [SerializeField] AircraftSpecifications airSpecifications;
        private AircraftThrust aircraftThrust;
        private Rigidbody rb;
        private AerodynamicParameters aerodynamicParameters;

        private ControlParameters controlParameters;
        private EngineParameters engineParameters;

        private float cl;
        private Vector3 prevAngularVelocity;
       
        private float ControlAuthority()
        {
            return q * EvaluateAOACurve();
        }
        private float yawControlAuthority;
        private Vector3 flightVelocity
        {
           get{ return rb.linearVelocity; }
        }
        private Vector3 localVelocity
        {
            get{ return transform.InverseTransformDirection(flightVelocity); }
        }
        private float q
        {

            get {return 0.5f * localVelocity.sqrMagnitude ; }
        }

        private void ApplyTorque(Vector3 torque)
        {
           
            Vector3 damp = prevAngularVelocity * rb.mass;
            prevAngularVelocity = rb.angularVelocity;
            rb.AddRelativeTorque(torque);
        }
        public void ApplyThrottle(float throttle)
        {
            Debug.Log(aircraftThrust.ApplyThrust(throttle, engineParameters));
         rb.AddRelativeForce(  aircraftThrust.ApplyThrust(throttle,engineParameters),ForceMode.Impulse);
        }
        public void ApplyPitch(float input)
        {
            float pitch = input;
            float gForce = ( Vector3.Cross(transform.forward, Physics.gravity.normalized)).magnitude;
            gForce = Mathf.Clamp(gForce, -0.25f, 0.25f);

            float glimitPitch =Mathf.Clamp(pitch, -gForce, gForce) * controlParameters.pitchStrength;
          
            Vector3 pitchAxis = glimitPitch * Vector3.right * ControlAuthority();
            ApplyTorque(pitchAxis);
        }
        private float CalculateAOADegress()
        {
            float y = Mathf.Min(0, -localVelocity.y);
            float aoa = Mathf.Atan2(y, localVelocity.z);
            
            return aoa * Mathf.Rad2Deg;
        }
        private float EvaluateAOACurve()
        {
            return aerodynamicParameters.aoaCurve.Evaluate(CalculateAOADegress());
        }
        private float EvaluateInducedDragCurve()
        {
            float speed = Mathf.Max(0,flightVelocity.magnitude);
            float aoaMultiplier = EvaluateAOACurve();
           
            return aerodynamicParameters.inducedDragCurve.Evaluate(speed) * aerodynamicParameters.inducedDragPower * aoaMultiplier;
        }
       public Vector3 dragDirection;
       
        
       public Vector3 liftDirection;

       void ApplyNoseStall()
        {
            Quaternion targetStallRotation = Quaternion.FromToRotation(localVelocity.normalized,-Vector3.up);
            Debug.Log(EvaluateAOACurve());
            if(EvaluateAOACurve() <= 0 && localVelocity.z < aerodynamicParameters.stallSpeed)
            {
                rb.MoveRotation( Quaternion.Slerp(transform.rotation, targetStallRotation, (1- EvaluateAOACurve()) * Time.fixedDeltaTime));
               
            }
            
        }
       void ApplyInducedDrag()
        {
           Vector3 inducedDragDirection = -Vector3.Cross(liftDirection, transform.right);
           Vector3 inducedDragForce = inducedDragDirection *  EvaluateInducedDragCurve();
        
           rb.AddForce(inducedDragForce);
        }
        void CalculateAndApplyLift()
        {
            if (localVelocity.z < 0) return;
            float drag = 0.5f * q * aerodynamicParameters.dragPower;
            cl = q * aerodynamicParameters.liftPower * EvaluateAOACurve();
            cl = Mathf.Clamp(cl, 0, Mathf.Abs(Physics.gravity.y) * rb.mass);
            dragDirection = -localVelocity.normalized;
            Vector3 dragForce =drag * dragDirection;
            liftDirection = Vector3.Cross(Vector3.Cross(localVelocity, dragDirection).normalized, -transform.right).normalized;
            Vector3 force = dragForce + (liftDirection * cl);
         
            Debug.DrawRay(transform.position, force);
            rb.AddRelativeForce(force * rb.mass * 0.001f);
        }
   
        void ApplyTorqueDrag()
        {
            float drag = -q * rb.angularVelocity.sqrMagnitude;

            rb.AddRelativeTorque(drag * rb.angularVelocity.normalized);
        }
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
         
            rb.mass = airSpecifications.aircraftMass;
            controlParameters = airSpecifications.controlParameters;
            aerodynamicParameters = airSpecifications.aerodynamicParameters;
            engineParameters = airSpecifications.engineParameters;
            aircraftThrust = GetComponent<AircraftThrust>();
            
        }

        private void FixedUpdate()
        {
            
            CalculateAndApplyLift();
            ApplyInducedDrag();
            ApplyNoseStall();
           // ApplyTorqueDrag();
        }
    }
}