using UnityEngine;
using Utilities;
using GameInfrastructure;
using UnityEngine.UIElements;
namespace Aircraft
{
    [RequireComponent(typeof(Rigidbody))]
    public class AeroBody : Updatable
    {


        [SerializeField] private bool startAirborne;


        /*
         * Adds Drag and lift to the a rigidbody 
         * 
         * Returns angle of attack and pressure for control authority
         * 
         * can be made for planes or missiles
         */
        public bool testing;
        public bool activate;
        //Forces 

        private Rigidbody rb;
        public Rigidbody RB;
        private Vector3 localVelocity;
        private Vector3 localAngularVelocity;
        private Vector3 speed;
        public Vector3 prevVelocity;
        //Lift 
        [SerializeField] private float wingArea;
        [SerializeField] private float wingAreaYaw;
        [Range(0.0f, 1.0f)]
        [SerializeField] private float liftCoefficient;
        [Range(0.0f, 1.0f)]
        [SerializeField] private float liftCoefficientYaw;
        [Range(0.0f, 1.0f)]
        [SerializeField] private float rho;
        private float coefficientOfLift;
        private float coefficientOfLiftYaw;
        private float angleOfAttack;
        private float angleOfAttackYaw;
        private float maxStallSpeed;
        private float minStallSpeed;
        private float loadFactor;
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
        [SerializeField] private float dragUpCoefficient, dragForwardCoefficient, dragLeftCoefficient;

        private void OnEnable()
        {

        }
        private void OnDisable()
        {
            // FIX: GameFlow.instance can already be destroyed on scene unload / quit -> NullReferenceException
            if (!testing && GameFlow.instance != null)
            {
                GameFlow.instance.RemoveUpdatable(this);
            }
        }
        private void UpdateState()
        {
            localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
            localAngularVelocity = transform.InverseTransformVector(rb.angularVelocity) * Mathf.Deg2Rad;
            CalculateLift();
            CalculateDrag();
            CalculateStall();
        }
        private void CalculateDrag()
        {
            Vector3 dragForce = Vector3.zero;

            // FIX: pressure already contains v^2. Multiplying it by the raw velocity components made
            // drag scale with v^3. Using the velocity *direction* gives the intended v^2 drag.
            Vector3 velocityDirection = localVelocity.normalized;

            dragForce.x = velocityDirection.x * -dragLeftCoefficient;
            dragForce.y = velocityDirection.y * -dragUpCoefficient;
            dragForce.z = velocityDirection.z * -dragForwardCoefficient;

            dragForce *= pressure;
            dragForce = transform.TransformDirection(dragForce);
            rb.AddForce(dragForce);
        }
        private void CalculateLift()
        {

            angleOfAttack = Mathf.Atan2(-localVelocity.y, localVelocity.z) * Mathf.Rad2Deg;

            angleOfAttackYaw = Mathf.Atan2(localVelocity.x, localVelocity.z) * Mathf.Rad2Deg;

            // FIX: InverseLerp returns 0..1, so zero AoA produced half the lift coefficient.
            // Remapping to -1..1 makes zero AoA = zero lift and keeps the sign for negative AoA.
            // FIX: yaw used maxAngleAttack instead of maxAngleAttackYaw.
            // Guard against a max angle of 0 (inspector default) which would divide by zero.
            float maxAoA = Mathf.Max(0.01f, maxAngleAttack);
            float maxAoAYaw = Mathf.Max(0.01f, maxAngleAttackYaw);

            coefficientOfLift = (MathUtilities.UnclampedInverseLerp(-maxAoA, maxAoA, angleOfAttack) * 2f - 1f) * liftCoefficient;
            coefficientOfLiftYaw = (MathUtilities.UnclampedInverseLerp(-maxAoAYaw, maxAoAYaw, angleOfAttackYaw) * 2f - 1f) * liftCoefficientYaw;

            Vector3 relativeWind = localVelocity.z * Vector3.up;
            Vector3 relativeWindYaw = localVelocity.z * Vector3.right;

            pressure = rho * rb.linearVelocity.sqrMagnitude;

            // FIX: lift is linear in CL (L = 0.5 * rho * v^2 * S * CL). It was squared, which threw away
            // the sign so negative AoA produced positive lift. The square belongs in induced drag only.
            Vector3 liftForce = relativeWind.normalized * pressure * 0.5f * coefficientOfLift * wingArea;

            // FIX: yaw force was using the pitch CL and pitch wing area.
            // FIX: negated so the side force opposes sideslip (positive local X velocity -> push toward -X).
            //      Without this the yaw surface was destabilising.
            Vector3 liftForceYaw = -relativeWindYaw.normalized * pressure * 0.5f * coefficientOfLiftYaw * wingAreaYaw;

            // FIX: yaw lift was computed but never added to the total.
            Vector3 liftTotalForce = liftForce + liftForceYaw;

            // FIX: operator precedence. CL^2 / PI * AR * e  ->  CL^2 / (PI * AR * e).
            // Guarded against AR or e being 0 which would give Infinity/NaN and poison the rigidbody.
            float inducedDragDenominator = Mathf.Max(Mathf.PI * aspectRatio * efficiencyFactor, 0.0001f);
            float inducedDragCoefficient = (coefficientOfLift * coefficientOfLift) / inducedDragDenominator;
            float inducedDragCoefficientYaw = (coefficientOfLiftYaw * coefficientOfLiftYaw) / inducedDragDenominator;

            // FIX: everything here is in LOCAL space (it gets TransformDirection'd below), but the old code
            // used transform.forward / transform.right (world space), so the vector got rotated twice.
            // FIX: induced drag always opposes forward motion. The yaw version pointed along the right axis
            // and reused the pitch coefficient.
            Vector3 inducedDragDirection = -(localVelocity.z * Vector3.forward).normalized;

            // FIX: a coefficient is not a force. Scale by dynamic pressure and wing area like the lift terms.
            Vector3 inducedDragForce = inducedDragDirection * pressure * 0.5f * wingArea * inducedDragCoefficient;
            Vector3 inducedDragForceYaw = inducedDragDirection * pressure * 0.5f * wingAreaYaw * inducedDragCoefficientYaw;

            Vector3 totalForce = (liftTotalForce + inducedDragForce + inducedDragForceYaw);
            totalForce = transform.TransformDirection(totalForce);
            rb.AddForce(totalForce, ForceMode.Force);


        }
        void CalculateStall()
        {
            // FIX: negative CL made coef negative -> Mathf.Sqrt(negative) = NaN, which then leaked out of the
            // getters (Mathf.Max(1, NaN) returns NaN).
            float coef = Mathf.Abs((rho * wingArea) * coefficientOfLift);

            if (coef > 0.5f)
            {
                maxStallSpeed = Mathf.Sqrt((2 * rb.mass) / coef);

                minStallSpeed = (2 * rb.mass) / coef;
            }
        }

        public override void SetRigidbody(bool setActive)
        {

            if (!setActive)
            {

                activate = true;

            }

        }

        //Return angle of attack and pressure


        public override void CustomStart()
        {
            if (!testing)
            {
                rb = GetComponent<Rigidbody>();
                RB = rb;
            }
            if (startAirborne)
            {
                rb.linearVelocity = transform.forward * 220;
                // FIX: Translate defaults to Space.Self, so the 100m offset followed the object's rotation
                // instead of going straight up.
                rb.transform.Translate(Vector3.up * 100, Space.World);
            }
        }
        private void FixedUpdate()
        {

            if (testing)
            {
                UpdateState();
            }
        }
        public override void CustomFixedUpdate()
        {

            if (!GameFlow.instance.paused && !testing)
            {
                rb.isKinematic = false;

                UpdateState();

            }

        }
        public override void CustomLateUpdate()
        {

        }
        public override void CustomUpdate()
        {

        }
        public void Start()
        {
            // FIX: grab the rigidbody BEFORE registering with GameFlow, otherwise CustomStart /
            // CustomFixedUpdate can run with rb == null.
            rb = GetComponent<Rigidbody>();
            RB = rb;

            if (!testing)
            {
                GameFlow.instance.AddUpdatable(this);

            }
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
        public float GetWingSurfaceArea()
        {
            return wingArea;
        }
        public Vector3 GetLinearVelocity()
        {
            return localVelocity;
        }

        public Vector3 GetAngularVelocity()
        {
            return localAngularVelocity;
        }
        public float GetMinStallSpeed()
        {
            return Mathf.Max(1, minStallSpeed);
        }
        public float GetMaxStallSpeed()
        {

            return Mathf.Max(1, maxStallSpeed);
        }
    }
}
