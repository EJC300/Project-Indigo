using UnityEngine;
using Aircraft;
using Utilities;
namespace Gameplay
{
    public class MissileGuidance : MonoBehaviour
    {

        [SerializeField] private SimplePID pitchPID;
        [SerializeField] private SimplePID yawPID;
        [SerializeField] private MissileController missileController;
        public GameObject target;
       [SerializeField] Rigidbody rb;
        [SerializeField] float secondsToActivate;

        [SerializeField] float proportionalGain = 5.0f;
        [SerializeField] float angularDamping = 0.5f;

        [SerializeField] private float navConstant = 3.0f;


        [SerializeField] private Vector3 targetPosition;

        [SerializeField] private Vector3 previousTargetPosition;

        [SerializeField] private Vector3 targetVelocity;

        [SerializeField] private Vector3 previousTargetVelocity;

        [SerializeField] private Vector3 targetAcceleration;

        [SerializeField] private Vector3 previousTargetAcceleration;
 
        Vector3 toTarget;
        Vector3 velocity;
        Vector3 direction;
        private void Update()
        {
            targetPosition = target.transform.position;
        }
        private void FixedUpdate()
        {
            TargetAcceleration();
            SteerMissile();
            CalculateDirection();
           
            previousTargetPosition = targetPosition;

        }

        private void Start()
        {
            rb = GetComponent<Rigidbody>();

        }
        void CalculateDirection()
        {
            Vector3 toTarget = target.transform.position - rb.position;
            Vector3 velocity =  transform.InverseTransformDirection(AccelerationCommand());
            direction = velocity;
           

        }
        void TargetAcceleration()
        {
            targetVelocity = (previousTargetPosition - targetPosition) * Time.fixedDeltaTime;

            targetAcceleration = (previousTargetVelocity - targetVelocity) * Time.fixedDeltaTime;

            previousTargetVelocity = targetVelocity;
        }

       

        Vector3 AccelerationCommand()
        {
               if (target == null) return Vector3.zero;

    Vector3 currentPosition = transform.position;
    Vector3 targetPosition = target.transform.position;

    Vector3 lineOfSightVector = targetPosition - currentPosition;
    
    // Use the actual distance (magnitude) for standard ProNav math
    float distance = lineOfSightVector.magnitude; 

    // SAFETY 1: Close-range cutoff to prevent division by zero or infinite spikes
    if (distance < 1.5f) 
    {
        // Freeze guidance commands completely right before impact 
        // to let the missile's momentum carry it through the target.
        return Vector3.zero; 
    }

    Vector3 losNormalized = lineOfSightVector / distance;

    // Fetch the target's relative velocity safely
    Vector3 targetVel = target.GetComponent<Rigidbody>()?.linearVelocity ?? Vector3.zero;
    Vector3 relativeVelocity = targetVel - rb.linearVelocity;

    // SAFETY 2: Overshoot check. If the target moves behind the missile's nose, stop guiding.
    if (Vector3.Dot(transform.forward, losNormalized) < 0f)
    {
        return Vector3.zero;
    }

    // Standard ProNav uses the linear distance denominator for the true angular LOS rate
    Vector3 losRate = Vector3.Cross(losNormalized, relativeVelocity) / distance;

    float closingVelocity = Vector3.Dot(relativeVelocity, losNormalized);

    // Final global acceleration command vector
    return closingVelocity * losRate * navConstant;

        }
        void SteerMissile()
        {

            float pitchInput = direction.y;

            float yawInput = direction.x;

            float pitchMag = pitchInput;


            float yawMag = -yawInput;
         
      
            Vector3 localAngularVel = transform.InverseTransformDirection(rb.angularVelocity);
            float pitch = pitchPID.CalculatePID(localAngularVel.x, pitchMag, Time.deltaTime)/200;

            float yaw = yawPID.CalculatePID(localAngularVel.y, yawMag, Time.deltaTime)/200;
            float normalizedPitch = pitch;
            float normalizedYaw = yaw;
            normalizedPitch = Mathf.Clamp(normalizedPitch, -1, 1);
            normalizedYaw = -Mathf.Clamp(normalizedYaw, -1, 1);

            if (Vector3.Distance(transform.position, target.transform.position) < 500f) // Adjust distance to your "halfway" point
            {
                Debug.Log($"[Missile Log] Target Yaw Accel: {yawMag:F2} | Current Yaw Rate: {localAngularVel.x:F2} | Yaw PID Output: {normalizedYaw:F2}");
            }


            //Really need to use a PID controller
            missileController.SetPitch(normalizedPitch);

            missileController.SetYaw(normalizedYaw);
        }
        float AccelerationBias()
        {
            float n = navConstant / 2;
            float acceleration = targetAcceleration.magnitude;
            float speed = targetVelocity.magnitude;
            if (speed > 0)
            {
                float delta = acceleration / speed;
                return n * delta;
            }
            else
            {
                return n;
            }

        }
    }
}