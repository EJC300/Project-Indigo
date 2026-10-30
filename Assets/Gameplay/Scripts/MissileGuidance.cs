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
        Vector3 previousPosition;
        Vector3 toTarget;
        Vector3 velocity;
        Vector3 direction;
        Vector3 previousLos;

        float prevClosingVelocity;
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
            Vector3 velocity = transform.InverseTransformDirection  ( toTarget.normalized + AccelerationCommand());
            direction = rb.linearVelocity + velocity.normalized;
           

        }
        void TargetAcceleration()
        {
            targetVelocity = (previousTargetPosition - targetPosition) / Time.fixedDeltaTime;

            targetAcceleration = (previousTargetVelocity - targetVelocity) / Time.fixedDeltaTime;

            previousTargetVelocity = targetVelocity;
        }

       

        Vector3 AccelerationCommand()
        {
            Vector3 currentPosition = transform.position;
            targetPosition = target.transform.position;

            Vector3 relativePosition = targetPosition - transform.position;

            Vector3 relativeVelocity = targetVelocity - (rb.linearVelocity);

            float timeUntilImpact = (relativeVelocity.magnitude / targetVelocity.magnitude);

            Vector3 zeroEffortMiss = relativePosition + relativeVelocity * timeUntilImpact;

            Vector3 unitLOS = relativePosition.normalized;

            Vector3 zeroEffortMissLOS = Vector3.Project(zeroEffortMiss, unitLOS);

            Vector3 zeroEffortNormal = zeroEffortMiss - zeroEffortMissLOS;

            Vector3 acceleration = navConstant * zeroEffortNormal / (timeUntilImpact * timeUntilImpact);
        
         
            return acceleration;
            




        }
        void SteerMissile()
        {

            float pitchInput = Mathf.Atan2(-direction.y, direction.z);

            float yawInput = Mathf.Atan2(direction.x, direction.z);

         
         
      
            Vector3 localAngularVel = transform.InverseTransformDirection(rb.angularVelocity);
            float pitch = pitchPID.CalculatePID(localAngularVel.x, pitchInput, Time.deltaTime)/100;

            float yaw = yawPID.CalculatePID(localAngularVel.y, yawInput, Time.deltaTime)/100;
            float normalizedPitch = pitch;
            float normalizedYaw = yaw;
            normalizedPitch = Mathf.Clamp(normalizedPitch, -1, 1);
            normalizedYaw = Mathf.Clamp(normalizedYaw, -1, 1);

  


          
            missileController.SetPitch(normalizedPitch);

            missileController.SetYaw(normalizedYaw);
        }
        float AccelerationBias()
        {
            float n = navConstant;
            float acceleration = targetAcceleration.magnitude;
            float speed = targetVelocity.magnitude;
            if (speed > 0)
            {
                float delta = acceleration / speed;
                return ((n * 0.5f) * delta);
            }
            else
            {
                return 0;
            }

        }
    }
}