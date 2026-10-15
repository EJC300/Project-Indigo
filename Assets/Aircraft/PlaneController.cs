

using UnityEngine;
using Utilities;

namespace Aircraft
{

    [RequireComponent(typeof(Rigidbody))]
    public class PlaneController : MonoBehaviour
    {

        private AeroBody body;
        private Rigidbody rb;
        [SerializeField] private float maxAOA;
        [SerializeField] private float maxG = 9.8f;
        [SerializeField] private float minG = 3.0f;
        [SerializeField] private ControlSurfaceAnimatable aileronLeft;
        [SerializeField] private ControlSurfaceAnimatable aileronRight;
        [SerializeField] private ControlSurfaceAnimatable elevatorLeft;
        [SerializeField] private ControlSurfaceAnimatable elevatorRight;
        [SerializeField] private float maxManeuveringSpeed = 500;
        [SerializeField] private float minManeuveringSpeed = 50;
        [SerializeField] private float maxDeflectionAnglePitch;
        [SerializeField] private float minDeflectionAnglePitch;
        [SerializeField] private float maxDeflectionAngleYaw;
        [SerializeField] private float minDeflectionAngelYae;
        [SerializeField] private float maxDeflectionAngleRoll;
        [SerializeField] private float minDeflectionAngleRoll;
        [SerializeField] private float speedBrakeDragCoef = 0.5f;
        [SerializeField] private float maxThrust;

        [SerializeField] private float throttleSpeed;
        private float pitchInputAngle;
        private float rollInputAngle;
        float pitchTarget = 0;
        private bool speedBrakes;
        private float acceleration;
        private float currentG;
        private float previousG;
        private float yawInput;
        private float rollInput;
        private float currentThrust;
        private float throttle;
        private float deflectionAnglePitch;
        private float deflectionAngleYaw;
        private float deflectionAngleRoll;
        



        private Vector3 CalculateGForce()
        {
           Vector3 globalVelocity = transform.TransformDirection(body.GetLinearVelocity());
            Vector3 globalAngularVelocity = transform.TransformDirection(body.GetAngularVelocity());
            return Vector3.Cross(globalAngularVelocity,globalVelocity) * 9.8f;
        }
        private float GLimit(float input)
        {
            float gForce = CalculateGForce().magnitude;
           
            currentG =  input*  gForce;
            acceleration = previousG - currentG/Time.fixedDeltaTime;
            float newInput = Utilities.MathUtilities.UnclampedInverseLerp(-minG, maxG, currentG);
         
            if(currentG > maxG )
            {
                return newInput;
            }
            else if (currentG < -minG)
            {
                return newInput;
            }
            else if (currentG== 0)
            {
                return input;
            }
                previousG = currentG;
            return input;
        }
        void AnimateControlSurfaces()
        {
        
          
            
            elevatorLeft.RotateToTargetAngle(Vector3.right, pitchInputAngle);

         
            elevatorRight.RotateToTargetAngle(Vector3.right,  pitchInputAngle);

            aileronLeft.RotateToTargetAngle(Vector3.right, rollInputAngle);

            aileronRight.RotateToTargetAngle(Vector3.right, -rollInputAngle);
        }
        private void SpeedBrakes()
        {
            if (speedBrakes)
            {
                Vector3 force = speedBrakeDragCoef * body.GetPressure * body.GetLinearVelocity().normalized;
                Vector3 forceDirection = transform.forward * force.z;
                rb.AddForce(-forceDirection);
            }
        }
        private void PitchYawRoll()
        {
            float speed = Mathf.InverseLerp( maxManeuveringSpeed, minManeuveringSpeed, body.GetLinearVelocity().z);
          
            deflectionAnglePitch = Mathf.Lerp(minDeflectionAnglePitch, maxDeflectionAnglePitch, speed);
            pitchInputAngle = Mathf.Clamp(pitchInputAngle, -deflectionAnglePitch, deflectionAnglePitch);

            float pitchForce = pitchInputAngle * body.GetPressure;

         

            deflectionAngleRoll = Mathf.Lerp(maxDeflectionAngleRoll, minDeflectionAngleRoll, speed);
            rollInputAngle = Mathf.Clamp(rollInputAngle, -deflectionAngleRoll, deflectionAngleRoll);
        
            float rollForce = rollInputAngle * body.GetPressure;

            rb.AddRelativeTorque(Vector3.right * pitchForce);
            rb.AddRelativeTorque(Vector3.forward * rollForce);
        }
        



        public void SetThrottle(float input)
        {
            throttle += input / 100;
            throttle = Mathf.Clamp01(throttle);




            currentThrust = Mathf.Lerp(currentThrust, maxThrust * throttle, throttleSpeed * Time.deltaTime);


        }




        public void SetPitch(float input)
        {
            float glimit = GLimit(input);
            Debug.Log(glimit);
        
            if (Mathf.Abs(input) <= 0)
            {
                pitchTarget = 0;
            }
            else
            {
                pitchTarget +=  45 * Time.fixedDeltaTime;
            }

                pitchInputAngle = Mathf.MoveTowards(pitchInputAngle, pitchTarget * glimit, Time.fixedDeltaTime  * 15) ;
                
        }
        public void SetRoll(float input)
        {
    
            if (Mathf.Abs(input) <= 0)
            {
                rollInputAngle = 0;
            }
            else
            {
               rollInputAngle += input * Time.fixedDeltaTime * 45;
            }
        }

        public void SetSpreadBrakes()
        {
            speedBrakes = throttle <= 0;
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

        private void Update()
        {
            SetSpreadBrakes();
        }
        private void FixedUpdate()
        {

          
            ApplyThrust();
            PitchYawRoll();
            SpeedBrakes();
            AnimateControlSurfaces();


        }










    }
}