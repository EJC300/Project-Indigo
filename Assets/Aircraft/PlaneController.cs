

using UnityEngine;
using UnityEngine.Windows;
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
        private float yawInputAngle;
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
        public float newPitchInput;

        private Vector3 lastVelocity;
        private float gravityMagnitude = 9.81f;

       
        private Vector3 CalculateGForce()
        {
       
            Vector3 currentVelocity = rb.linearVelocity;
            Vector3 acceleration = (currentVelocity - lastVelocity) / Time.fixedDeltaTime;
           

           
            acceleration -= Physics.gravity;

            lastVelocity = currentVelocity;
            Vector3 localAcceleration = transform.InverseTransformDirection(acceleration);
           
        
            return localAcceleration / gravityMagnitude;
        }
        public float GetMaxAOA
        {
            get { return maxAOA; }
        }
        
        public float GetGforce()
        {
            return currentG;
        }
        private void GLimit(float input)
        {

        
            float gForce = CalculateGForce().y;
            currentG = gForce;

            
         

     
            if (Mathf.Abs(currentG) > minG)
            {
               
                if (Mathf.Sign(input) == Mathf.Sign(currentG))
                {
                   
                    float controlMultiplier = Mathf.InverseLerp(maxG, minG, Mathf.Abs(currentG));

                    newPitchInput = controlMultiplier;
                }
            }




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
            deflectionAngleYaw = Mathf.Lerp(maxDeflectionAngleYaw, minDeflectionAngelYae, speed);
            yawInputAngle = Mathf.Clamp(yawInputAngle, -deflectionAngleYaw, deflectionAngleYaw);
            float yawForce = yawInputAngle * body.GetPressure;
            rb.AddRelativeTorque(Vector3.up * yawForce);
            rb.AddRelativeTorque(Vector3.right * pitchForce);
            rb.AddRelativeTorque(Vector3.forward * rollForce);
        }
        
        public float GetThrust()
        {
            return currentThrust;
        }
        public bool TriggerAfterBurnerEffect()
        {
            return currentThrust > maxThrust * 0.80f;
        }
        public float MilThrustAmount()
        {
            return maxThrust * 0.80f;
        }
        public float FullAfterBurnerAmount()
        {
            return maxThrust;
        }
        public void SetThrottle(float input)
        {
            throttle += input / 100;
            throttle = Mathf.Clamp01(throttle);




            currentThrust = Mathf.Lerp(currentThrust, maxThrust * throttle, throttleSpeed * Time.deltaTime);


        }

        public void SetYaw(float input)
        {
            if (Mathf.Abs(input) <= 0)
            {
                yawInputAngle = 0;
            }
            else
            {
                yawInputAngle += input* 45 * Time.fixedDeltaTime;
            }
         
        }


        public void SetPitch(float input)
        {
            GLimit(input);
           

            if ((input) == 0)
            {
                pitchTarget = 0;
            }
            else
            {
                pitchTarget += input * 45 * Time.fixedDeltaTime;
            }
            float maxAllowedAngle = pitchTarget * newPitchInput;

            if (pitchTarget > 0) pitchTarget = Mathf.Min(pitchTarget, maxAllowedAngle);
            else pitchTarget = Mathf.Max(pitchTarget, maxAllowedAngle);
            pitchInputAngle = Mathf.MoveTowards(pitchInputAngle,  pitchTarget, Time.fixedDeltaTime * 15);
                
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

            lastVelocity = rb.linearVelocity;
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