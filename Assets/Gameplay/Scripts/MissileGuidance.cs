using UnityEngine;
using Aircraft;
namespace Gameplay
{
    public class MissileGuidance : MonoBehaviour
    {
        [SerializeField] private MissileController missileController;
        public Rigidbody targetTest;
        private Rigidbody rb;
        public float secondsToActivate;
        private float seconds;
        public float navConstant;
        private float distance;
    
        private Vector3 PredictedTargetOffset()
        {
            return transform.up + (targetTest.position + targetTest.linearVelocity).normalized;
        }
        Vector3 CalculateTarget()
        {
            if (distance > 0)
            {
                Vector3 toTarget = transform.TransformDirection(PredictedTargetOffset());
                distance = toTarget.magnitude;
                Vector3 los = toTarget.normalized;
                Vector3 relativeVelocity = rb.linearVelocity - targetTest.linearVelocity;
                float terminalVelocity = Vector3.Dot(relativeVelocity, los);
                Vector3 losRate = Vector3.Cross(los, relativeVelocity) / distance;
                Vector3 accelCommand = navConstant * terminalVelocity * losRate;
                return accelCommand;
            
            }
            return Vector3.zero;
        }

        void GuideMissile()
        {
            //I made my missiles weird :/
            Vector3 direction =   targetTest.position - transform.position;
            Vector3 relativeDirection = transform.InverseTransformDirection(direction);
            Debug.Log(relativeDirection);
            float pitch = direction.normalized.y * 15;
            float pitchError = pitch * 15 - rb.angularVelocity.z;
            Vector3 flatRight = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;

            
            float rollAngle = Vector3.SignedAngle(transform.right, flatRight, transform.forward);
            Debug.Log(pitchError);
            float roll = rollAngle;
            float rollError =  roll * 15 - rb.angularVelocity.x ;
            missileController.SetRoll(rollError);
            //missileController.SetPitch(pitchError);
        }
        private void FixedUpdate()
        {
           
        }
        private void Start()
        {
            seconds = secondsToActivate;
            rb = GetComponent<Rigidbody>();
        }
        private void Update()
        {
            if (seconds > 0)
            {
                seconds -= Time.deltaTime;
            }
            else
            {
                GuideMissile();
            }
        }


    }
}