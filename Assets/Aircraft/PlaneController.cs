

using UnityEngine;
using Utilities;

namespace Aircraft
{

    [RequireComponent(typeof(Rigidbody))]
    public class PlaneController : MonoBehaviour
    {

        private Rigidbody rb;

        [SerializeField] private float aoaBias = 0.5f;

        [SerializeField] private AnimationCurve dragCurve;

        [SerializeField] private AnimationCurve pitchCurve;
        [SerializeField] private AnimationCurve yawCurve;
        [SerializeField] private AnimationCurve rollCurve;

        [SerializeField] private AnimationCurve inducedDragCurve;

        [SerializeField] private AnimationCurve AOAYawCurve;

        [SerializeField] private AnimationCurve AOACurve;

        [SerializeField] private float liftPower;

        [SerializeField] private float inducedDragPower;

        [SerializeField] private float dragForward;
        [SerializeField] private float dragUp;
        [SerializeField] private float dragLeft;
        [SerializeField] private float rollStrengh;
        [SerializeField] private float pitchStrengh;
        [SerializeField] private float yawStrengh;

        [SerializeField] private float maxThrust;

        [SerializeField] private float throttleSpeed;
        private float pitchInput;
        private float yawForce;
        private float pitchForce;
        private float currentThrust;
        private float throttle;

        private Vector3 localVelocity;
        private Vector3 localAngleVelocity;
        private Vector3 liftVelocity;
        float pressure;
        float angleOfAttack;
        private void UpdatePhysics()
        {
            localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
            localAngleVelocity = Quaternion.Inverse(transform.rotation) * rb.angularVelocity;
            angleOfAttack = AOACurve.Evaluate(Mathf.Atan2(-localVelocity.y, localVelocity.z) + aoaBias * Mathf.Rad2Deg);

            float angleOfAttackYaw = Mathf.Atan2(localVelocity.x, localVelocity.z) * Mathf.Rad2Deg;
            pressure = localVelocity.sqrMagnitude;


            ApplyThrust();
            ApplySteer();
            CalculateLift((angleOfAttack), pressure, Vector3.up);




        }



        public void SetThrottle(float input)
        {
            throttle += input / 100;
            throttle = Mathf.Clamp01(throttle);




            currentThrust = Mathf.Lerp(currentThrust, maxThrust * throttle, throttleSpeed * Time.deltaTime);


        }


        void ApplySteer()
        {
            Vector3 prevAngular = Vector3.zero;
            float pitchVelocity = CalculateSteer(pitchInput * pitchStrengh, localAngleVelocity.x, pressure, angleOfAttack);
            Vector3 damp = -localAngleVelocity.normalized * localAngleVelocity.sqrMagnitude * 5000;
            rb.AddRelativeTorque(pitchVelocity * Vector3.right, ForceMode.Acceleration);
            rb.AddTorque(damp);
            prevAngular = localAngleVelocity;
        }

        public void SetPitch(float input)
        {
            pitchInput = input;

        }

        float CalculateSteer(float targetVelocity, float currentVelocity, float q, float aoa)
        {
            float controlAuthority = Mathf.InverseLerp(0, 98, Mathf.Max(0, localVelocity.z));
            controlAuthority = Mathf.Clamp01(controlAuthority);

            float error = (targetVelocity * controlAuthority) - currentVelocity;


            return Mathf.Clamp(error, Time.fixedDeltaTime * -5, Time.fixedDeltaTime * 5);

        }
        void CalculateLift(float aoa, float q, Vector3 axis)
        {








            liftVelocity = localVelocity.z * axis;


            float liftCoef = localVelocity.z * liftPower * aoa;


            Vector3 liftDirection = liftVelocity.normalized;
            Vector3 lift = liftDirection * (liftCoef) * localVelocity.sqrMagnitude;
            Debug.Log(liftDirection);

            float speedFactor = inducedDragCurve.Evaluate(Mathf.Clamp01(localVelocity.z / 100f));
            Vector3 inducedDrag = liftVelocity.sqrMagnitude * inducedDragPower * speedFactor * aoa * -localVelocity.normalized;

            Vector3 totalLift = Vector3.zero;

            totalLift = transform.TransformDirection(lift + inducedDrag);



            rb.AddForce(totalLift);









        }


        void ApplyThrust()
        {
            Vector3 thrust = Vector3.forward * currentThrust;
            Vector3 drag = Vector3.zero;

            drag.x = -localVelocity.x * dragLeft;
            drag.y = -localVelocity.y * dragUp;
            drag.z = -localVelocity.z * dragForward;
            Debug.Log(drag);


            rb.AddRelativeForce(thrust);
            rb.AddRelativeForce(((drag * localVelocity.sqrMagnitude)));

        }


        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {

            UpdatePhysics();
            Debug.Log(localVelocity.z * 3.6);


        }










    }
}