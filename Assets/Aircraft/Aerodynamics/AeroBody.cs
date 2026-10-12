using UnityEngine;
using Utilities;
namespace Aircraft
{
    [RequireComponent(typeof(Rigidbody))]
    public class AeroBody : MonoBehaviour
    {

        /*
         * Adds Drag and lift to the a rigidbody 
         * 
         * Returns angle of attack and pressure for control authority
         * 
         * can be made for planes or missiles
         */

        //Forces 

        private Rigidbody rb;
        private Vector3 localVelocity;
        private Vector3 localAngularVelocity;
        private Vector3 speed;

        //Lift 
        [SerializeField] private float wingArea;
        [SerializeField] private float wingAreaYaw;
        [Range(0.0f,1.0f)]
        [SerializeField] private float liftCoefficient ;
        [Range(0.0f, 1.0f)]
        [SerializeField] private float liftCoefficientYaw;
        [Range(0.0f,1.0f)]
        [SerializeField] private float rho;
         private float coefficientOfLift;
         private float coefficientOfLiftYaw;
         private float angleOfAttack;
         private float angleOfAttackYaw;
        [Range(0.0f, 360.0f)]
        [SerializeField]
        private float maxAngleAttack;
        [Range(0.0f, 360.0f)]
        [SerializeField]
        private float maxAngleAttackYaw;
         private float pressure;
        //Drag
        [Range(0.0f, 1.0f)]
        [SerializeField] private float aspectRatio;

        [Range(0.0f, 1.0f)]
        [SerializeField] private float efficiencyFactor;
        [Range(0.0f, 1.0f)]
        [SerializeField] private float dragUpCoefficient,dragForwardCoefficient,dragLeftCoefficient;

       

        private void UpdateState()
        {
            localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
            localAngularVelocity = transform.InverseTransformVector(rb.angularVelocity) * Mathf.Deg2Rad;
            CalculateLift();
            CalculateDrag();
        }
        private void CalculateDrag()
        {
            Vector3 dragForce = Vector3.zero;

            dragForce.x = localVelocity.x * -dragLeftCoefficient;
            dragForce.y = localVelocity.y * -dragUpCoefficient;
            dragForce.z = localVelocity.z * -dragForwardCoefficient;

            dragForce *= pressure;
            dragForce = transform.TransformDirection(dragForce);
            rb.AddForce(dragForce);
        }
        private void CalculateLift()
        {

            angleOfAttack = Mathf.Atan2(-localVelocity.y, localVelocity.z) * Mathf.Rad2Deg;

            angleOfAttackYaw = Mathf.Atan2(localVelocity.x, localVelocity.z) * Mathf.Rad2Deg;
       
            coefficientOfLift = MathUtilities.UnclampedInverseLerp(-maxAngleAttack, maxAngleAttack, angleOfAttack) * liftCoefficient;
            coefficientOfLiftYaw = MathUtilities.UnclampedInverseLerp(-maxAngleAttack, maxAngleAttack, angleOfAttackYaw) * liftCoefficientYaw;
          
            Vector3 relativeWind = localVelocity.z * Vector3.up;
            Vector3 relativeWindYaw = localVelocity.z * Vector3. right;

            pressure = rho * rb.linearVelocity.sqrMagnitude;

             

            Vector3 liftForce = relativeWind.normalized * pressure * 0.5f * (coefficientOfLift * coefficientOfLift) * wingArea;
          
            Vector3 liftForceYaw = relativeWindYaw.normalized * pressure * (0.5f * (coefficientOfLift * coefficientOfLift)) * wingArea;

            Vector3 liftTotalForce = (liftForce);
      
            float inducedDragCoefficient = (coefficientOfLift * coefficientOfLift) / Mathf.PI * aspectRatio * efficiencyFactor;
            Vector3 inducedDragForce = -(localVelocity.z * transform.forward).normalized * inducedDragCoefficient;
            Vector3 inducedDragForceYaw = -(localVelocity.z * transform.right).normalized * inducedDragCoefficient;
            Vector3 totalForce =transform.TransformDirection (liftTotalForce + inducedDragForce + inducedDragForceYaw) ;
            Debug.Log(totalForce);
            rb.AddForce(totalForce,ForceMode.Force);


        }


        //Return angle of attack and pressure
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }
        private void FixedUpdate()
        {
            UpdateState();
           
        }


        public float GetAngleOfAttack()
        {
            return angleOfAttack;
        }
        public float GetAngleOfAttackYaw()
        {
            return angleOfAttackYaw;
        }
        

       public float GetPressure
        {
            get { return pressure; }
        }





    }
}
