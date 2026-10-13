

using UnityEngine;
using Utilities;

namespace Aircraft
{

    [RequireComponent(typeof(Rigidbody))]
    public class PlaneController : MonoBehaviour
    {

        private AeroBody body;
        private Rigidbody rb;



        [SerializeField] private float turnAcceleration;
        [SerializeField] private float pitchStrength;
        [SerializeField] private float yawStrength;
        [SerializeField] private float rollStrength;


        [SerializeField] private float maxThrust;

        [SerializeField] private float throttleSpeed;
        private float pitchInput;
        private float yawInput;
        private float rollInput;
        private float currentThrust;
        private float throttle;

  
        float pressure;
        float angleOfAttack;
     
        

           
           




        



        public void SetThrottle(float input)
        {
            throttle += input / 100;
            throttle = Mathf.Clamp01(throttle);




            currentThrust = Mathf.Lerp(currentThrust, maxThrust * throttle, throttleSpeed * Time.deltaTime);


        }


        void ApplySteer()
        {
            Vector3 prevAngular = Vector3.zero;
            float pitchVelocity = CalculateSteer(pitchInput * pitchStrength, body.GetAngularVelocity().x, body.GetAngleOfAttack());
            Vector3 damp = -body.GetAngularVelocity().normalized * body.GetAngularVelocity().sqrMagnitude * 5;
            if (Mathf.Abs (pitchVelocity)> Mathf.Epsilon)
            {
                rb.AddRelativeTorque(pitchVelocity * Vector3.right,ForceMode.VelocityChange);
              
            }

            float roll = CalculateSteer(rollInput * rollStrength, body.GetAngularVelocity().z, body.GetAngleOfAttack());
          
            if (Mathf.Abs(roll) > Mathf.Epsilon)
            {
                rb.AddRelativeTorque(roll* Vector3.forward,ForceMode.VelocityChange);

            }
            rb.AddRelativeTorque(damp, ForceMode.VelocityChange);
            prevAngular = body.GetAngularVelocity();
        }

        public void SetPitch(float input)
        {
            pitchInput = input;

        }
        public void SetRoll(float input)
        {
            rollInput  = input;
        }

        float CalculateSteer(float targetVelocity, float currentVelocity, float aoa)
        {
            float minStallSpeed = body.GetMinStallSpeed();
            float maxStallSpeed = body.GetMaxStallSpeed();

            float controlAuthority = (Mathf.InverseLerp(0,maxStallSpeed,body.GetLinearVelocity().z)) * aoa;
            Debug.Log(controlAuthority);

            float error = (targetVelocity ) - currentVelocity;
            float acceleration = turnAcceleration * Time.deltaTime;

            return Mathf.Clamp(error, -acceleration, acceleration) ;

        }


        void ApplyThrust()
        {
            Vector3 thrust = Vector3.forward * currentThrust;

         


            rb.AddRelativeForce(thrust);
            

        }


        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            body = GetComponent<AeroBody>();
        }

        private void FixedUpdate()
        {

            ApplySteer();
            ApplyThrust();
          


        }










    }
}