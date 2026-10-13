

using UnityEngine;
using Utilities;

namespace Aircraft
{

    [RequireComponent(typeof(Rigidbody))]
    public class PlaneController : MonoBehaviour
    {

        private AeroBody body;
        private Rigidbody rb;



        [SerializeField] private float maxManeuveringSpeed = 500;
        [SerializeField] private float minManeuveringSpeed = 50;
        [SerializeField] private float maxDeflectionAnglePitch;
        [SerializeField] private float minDeflectionAngePitch;
        [SerializeField] private float maxDeflectionAngleYaw;
        [SerializeField] private float minDeflectionAngeYae;
        [SerializeField] private float maxDeflectionAngleRoll;
        [SerializeField] private float minDeflectionAngeRoll;

        [SerializeField] private float maxThrust;

        [SerializeField] private float throttleSpeed;
        private float pitchInputAngle;
        private float yawInput;
        private float rollInput;
        private float currentThrust;
        private float throttle;
        private float deflectionAnglePitch;
        private float deflectionAngleYaw;
        private float deflectionAngleRoll;

 
     
        

           
           



        private void PitchYawRoll()
        {
            float speed = Mathf.InverseLerp(minManeuveringSpeed, maxManeuveringSpeed, body.GetLinearVelocity().magnitude);

            deflectionAnglePitch = Mathf.Lerp(maxDeflectionAnglePitch, minDeflectionAngePitch, speed);
            pitchInputAngle = Mathf.Clamp(pitchInputAngle, -deflectionAnglePitch, deflectionAnglePitch);

            float pitchForce = pitchInputAngle * body.GetPressure;
            Debug.Log(pitchForce);
            rb.AddRelativeTorque(Vector3.right * pitchForce);
        }
        



        public void SetThrottle(float input)
        {
            throttle += input / 100;
            throttle = Mathf.Clamp01(throttle);




            currentThrust = Mathf.Lerp(currentThrust, maxThrust * throttle, throttleSpeed * Time.deltaTime);


        }




        public void SetPitch(float input)
        {

            if (Mathf.Abs(input) <= 0)
            {
                pitchInputAngle = 0;
            }
            else
            {
                pitchInputAngle += input;
            }


        }
        public void SetRoll(float input)
        {
            rollInput = input;
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

          
            ApplyThrust();
            PitchYawRoll();


        }










    }
}