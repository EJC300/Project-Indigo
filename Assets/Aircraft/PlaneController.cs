

using UnityEngine;
using Utilities;

namespace Aircraft
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlaneController : MonoBehaviour
    {

        private Rigidbody rb;

        
        
        [SerializeField] private AnimationCurve dragCurve;
        
        [SerializeField] private AnimationCurve pitchCurve;
        [SerializeField] private AnimationCurve yawCurve;
        [SerializeField] private AnimationCurve rollCurve;

        [SerializeField] private AnimationCurve inducedDragCurve;

        [SerializeField] private AnimationCurve AOAYawCurve;

        [SerializeField] private AnimationCurve AOACurve;

        [SerializeField] private float liftPower;

        [SerializeField] private float inducedDragPower;

        [SerializeField] private float dragStrength;
        [SerializeField] private float rollStrengh;
        [SerializeField] private float pitchStrengh;
        [SerializeField] private float yawStrengh;

        [SerializeField] private float maxThrust;

        [SerializeField] private float throttleSpeed;

        private float yawForce;

        private float currentThrust;
        private float throttle;

        private Vector3 localVelocity;
        private Vector3 localAngleVelocity;


        private void UpdatePhysics()
        {
            localVelocity = Quaternion.Inverse(rb.rotation) *(rb.linearVelocity);
            localAngleVelocity = Quaternion.Inverse(transform.rotation) * rb.angularVelocity;
            float angleOfAttack = Mathf.Atan2(-localVelocity.y, localVelocity.z) * Mathf.Rad2Deg;

            float angleOfAttackYaw = Mathf.Atan2(localVelocity.x,localVelocity.z) * Mathf.Rad2Deg;

          

            CalculateLift(AOACurve.Evaluate( angleOfAttack), Vector3.right);

           
            ApplyThrust();


            StallNoseDown();

        
   
        }



        public void SetThrottle(float input)
        {
            throttle += input / 100;
            throttle = Mathf.Clamp01(throttle);




            currentThrust = Mathf.Lerp(currentThrust, maxThrust * throttle, throttleSpeed * Time.deltaTime);
            Debug.Log(currentThrust);

        }

        void StallNoseDown()
        {
            Vector3 noseDown = Vector3.Project(localVelocity,Vector3.up);
            Vector3 direction = (noseDown - transform.position).normalized;

            Debug.DrawRay(transform.position,direction *20);
        }

        void CalculateLift(float aoa,Vector3 axis)
        {





          
            if (localVelocity.z < 0.1f) return; ;

      
                Vector3 liftVelocity = Vector3.ProjectOnPlane(localVelocity,axis);
                float q = liftVelocity.sqrMagnitude;          
                
                float liftCoef = liftPower * q * AOACurve.Evaluate(aoa)  ;

            
                Vector3 liftDirection = Vector3.Cross(liftVelocity.normalized,axis);
            Vector3 lift = liftDirection *  (0.5f * liftCoef);
                Debug.DrawRay(transform.position, liftDirection);

               float speedFactor = inducedDragCurve.Evaluate(Mathf.Clamp01(localVelocity.magnitude / 100f));
            Vector3 inducedDrag = liftVelocity.sqrMagnitude * inducedDragPower * speedFactor * AOACurve.Evaluate(aoa) * -localVelocity.normalized;

        
            
                rb.AddRelativeForce(lift + inducedDrag);
         

            Debug.Log(localVelocity.magnitude * 3.6);
            




        }

        
        void ApplyThrust()
        {
            Vector3 thrust = Vector3.forward * currentThrust;
       
            Vector3 dragForce = -rb.linearVelocity.normalized * (0.5f * rb.linearVelocity.sqrMagnitude ) * dragStrength;
            rb.AddRelativeForce(thrust);
            rb.AddForce(dragForce);
            
        }
   
    
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {

            UpdatePhysics();
          
           
        }


     







    }
}