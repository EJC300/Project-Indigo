using Aircraft;
using UnityEngine;
namespace AircraftData
{

    [RequireComponent(typeof(Rigidbody))]
    public class Aerobody : MonoBehaviour
    {

        [SerializeField] AircraftSpecifications airSpecifications;

        private Rigidbody rb;
        private AerodynamicParameters aerodynamicParameters;

        private ControlParameters controlParameters;

        public float cl;

        private float inducedDragCoeff;

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
            Debug.Log(aoaMultiplier);
            return aerodynamicParameters.inducedDragCurve.Evaluate(speed) * aerodynamicParameters.inducedDragPower * aoaMultiplier;
        }
       public Vector3 dragDirection;
       
        
       public Vector3 liftDirection;
       void ApplyInducedDrag()
        {
           Vector3 inducedDragDirection = -Vector3.Cross(liftDirection, transform.right);
           Vector3 inducedDragForce = inducedDragDirection *  EvaluateInducedDragCurve();
        
           rb.AddForce(inducedDragForce);
        }
        void CalculateAndApplyLift()
        {
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
            float drag = -q * rb.angularVelocity.sqrMagnitude * rb.mass;

            rb.AddRelativeTorque(drag * rb.angularVelocity.normalized);
        }
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.mass = airSpecifications.aircraftMass;
            controlParameters = airSpecifications.controlParameters;
            aerodynamicParameters = airSpecifications.aerodynamicParameters;

        }

        private void FixedUpdate()
        {
            
            CalculateAndApplyLift();
            ApplyInducedDrag();
            ApplyTorqueDrag();
        }
    }
}