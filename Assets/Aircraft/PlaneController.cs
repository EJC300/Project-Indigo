using Aircraft;
using UnityEngine;
using Utilities;

namespace AircraftData
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlaneController : MonoBehaviour
    {
        [SerializeField] AircraftSpecifications airSpecifications;

        [Header("Tuning")]
        [Tooltip("Scales aero forces. 0.001 matches the old (mass * 0.001) hack.")]
        [SerializeField] float aeroForceScale = 0.001f;
        [Tooltip("How many G of headroom before the limiter fully cuts pitch input.")]
        [SerializeField] float gLimiterSoftRange = 2f;
        [Tooltip("Nose-drop torque (rad/s^2) when below stall speed.")]
        [SerializeField] float noseDropStrength = 1.5f;
        [Tooltip("Angular damping per local axis (pitch, yaw, roll). Replaces ApplyTorqueDrag.")]
        [SerializeField] Vector3 angularDamping = new Vector3(1.5f, 1.0f, 1.5f);

        [Header("Roll / Yaw")]
        [Tooltip("Roll torque (rad/s^2) at full input and full control authority.")]
        [SerializeField] float rollStrength = 4f;
        [Tooltip("Yaw torque (rad/s^2) at full input and full control authority.")]
        [SerializeField] float yawStrength = 1.5f;
        [Tooltip("Side force opposing sideslip. Stops the plane sliding sideways when yawing.")]
        [SerializeField] float sideslipDrag = 0.5f;
        [Tooltip("Weathervane torque that swings the nose back into the airflow.")]
        [SerializeField] float sideslipStability = 1.0f;

        private AircraftThrust aircraftThrust;
        private Rigidbody rb;
        private AerodynamicParameters aerodynamicParameters;
        private ControlParameters controlParameters;
        private EngineParameters engineParameters;

        // Inputs are stored and applied in FixedUpdate so behaviour is framerate independent.
        private float pitchInput;
        private float rollInput;
        private float yawInput;
        private float throttleInput;

        private Vector3 lastVelocity;
        private float currentG = 1f;

        // Debug / gizmo access
        public Vector3 dragDirection;
        public Vector3 liftDirection;

        // ---------- Helpers ----------

        private Vector3 localVelocity => transform.InverseTransformDirection(rb.linearVelocity);

        // Dynamic pressure (density omitted; fold it into liftPower/dragPower)
        private float q => 0.5f * rb.linearVelocity.sqrMagnitude;

        // Angle of attack in degrees. Positive = nose above the flight path.
        private float CalculateAOADegrees()
        {
            Vector3 lv = localVelocity;
            return Mathf.Atan2(-lv.y, lv.z) * Mathf.Rad2Deg;
        }

        private float EvaluateAOACurve() => aerodynamicParameters.aoaCurve.Evaluate(CalculateAOADegrees());

        // ---------- Public input API ----------

        public void ApplyThrottle(float throttle) => throttleInput = throttle;
        public void ApplyPitch(float input) => pitchInput = Mathf.Clamp(input, -1f, 1f);
        public void ApplyRoll(float input) => rollInput = Mathf.Clamp(input, -1f, 1f);   // +1 = roll right
        public void ApplyYaw(float input) => yawInput = Mathf.Clamp(input, -1f, 1f);     // +1 = yaw right

        // ---------- G force ----------

        // Proper acceleration (what the pilot feels) = actual acceleration - gravity.
        // Level flight reads +1G on local Y. Call once per physics step.
        private void UpdateGForce()
        {
            Vector3 accel = (rb.linearVelocity - lastVelocity) / Time.fixedDeltaTime;
            lastVelocity = rb.linearVelocity;

            Vector3 properAccel = accel - Physics.gravity;
            currentG = transform.InverseTransformDirection(properAccel).y / 9.81f;
        }

        // pitchInput > 0 = pull up. Fade out input as we approach the G limit.
        private float ApplyGLimiter(float input, float g)
        {
            if (input > 0f)
            {
                float maxG = Mathf.Abs(controlParameters.maxGlimit);
                return input * Mathf.Clamp01((maxG - g) / gLimiterSoftRange);
            }
            if (input < 0f)
            {
                float minG = -Mathf.Abs(controlParameters.minGlimit);
                return input * Mathf.Clamp01((g - minG) / gLimiterSoftRange);
            }
            return 0f;
        }

        // Controls fade out below stall speed, full authority above it.
        private float ControlAuthority()
        {
            float stall = Mathf.Max(1f, aerodynamicParameters.stallSpeed);
            return Mathf.Clamp01(q / (0.5f * stall * stall));
        }

        // ---------- Forces ----------

        private void ApplyAerodynamics()
        {
            Vector3 lv = localVelocity;
            if (lv.z <= 0.1f) return; // flying backwards / stationary: no wing forces

            Vector3 velDir = lv.normalized;
            float aoaLift = EvaluateAOACurve();

            // Lift is perpendicular to the airflow, in the plane of the wings' pitch axis (local X).
            Vector3 velYZ = new Vector3(0f, lv.y, lv.z).normalized;
            liftDirection = Vector3.Cross(velYZ, Vector3.right).normalized; // forward flow -> local up
            dragDirection = -velDir;

            float lift = q * aerodynamicParameters.liftPower * aoaLift;
            Debug.Log(lift);
            float parasiticDrag = 0.5f * q * aerodynamicParameters.dragPower;

            // Induced drag: grows with how hard the wing is working (|AoA lift|), bleeds speed in hard turns.
            float speed = rb.linearVelocity.magnitude;
            float inducedDrag = aerodynamicParameters.inducedDragCurve.Evaluate(speed)
                                * aerodynamicParameters.inducedDragPower
                                * Mathf.Abs(aoaLift);

            Vector3 force = -liftDirection * lift + dragDirection * (parasiticDrag + inducedDrag);

            // Everything above is in local space -> AddRelativeForce.
            // Acceleration mode == force * mass, so this matches the old "* mass * 0.001" behaviour.
            rb.AddRelativeForce(force * aeroForceScale, ForceMode.Acceleration);
        }

        private void ApplyThrust()
        {
            // ForceMode.Force because we're in FixedUpdate (Impulse here made thrust scale with step rate).
            rb.AddRelativeForce(aircraftThrust.ApplyThrust(throttleInput, engineParameters), ForceMode.Impulse);
        }

        private void ApplyPitchTorque()
        {
            float limited = ApplyGLimiter(-pitchInput, currentG);
            float strength = limited * controlParameters.pitchStrength * ControlAuthority();

            // Unity is left-handed: +X rotation pitches the nose DOWN, so pull-up is -X.
            // If your input is inverted, flip this sign.
            rb.AddRelativeTorque(Vector3.right * strength, ForceMode.Acceleration);
        }

        private void ApplyRollTorque()
        {
            // Positive Z rotation in Unity rolls LEFT, so roll-right is -Z.
            float strength = rollInput * rollStrength * ControlAuthority();
            rb.AddRelativeTorque(Vector3.back * strength, ForceMode.Acceleration);
        }

        private void ApplyYawTorque()
        {
            // Positive Y rotation yaws RIGHT.
            float strength = yawInput * yawStrength * ControlAuthority();
            rb.AddRelativeTorque(Vector3.up * strength, ForceMode.Acceleration);
        }

        // Without this, yawing just points the nose while the plane keeps sliding the old way.
        private void ApplySideslip()
        {
            Vector3 lv = localVelocity;
            float speed = rb.linearVelocity.magnitude;
            if (speed < 0.5f) return;

            // Side force opposes lateral velocity so the flight path follows the nose.
            rb.AddRelativeForce(Vector3.right * (-lv.x * sideslipDrag), ForceMode.Acceleration);

            // Weathervane: airflow from the right (lv.x > 0) yaws the nose right to meet it.
            float slip = lv.x / speed; // roughly sin(sideslip angle)
            rb.AddRelativeTorque(Vector3.up * (slip * sideslipStability * ControlAuthority() * 10f), ForceMode.Acceleration);
        }

        // Below stall speed the nose falls toward the ground.
        private void ApplyNoseStall()
        {
            float forwardSpeed = localVelocity.z;
            float stall = aerodynamicParameters.stallSpeed;
            if (forwardSpeed >= stall) return;

            float factor = 1f - Mathf.Clamp01(forwardSpeed / Mathf.Max(1f, stall));
            Vector3 axis = Vector3.Cross(transform.forward, Vector3.down); // rotates nose toward down
            rb.AddTorque(axis * factor * noseDropStrength, ForceMode.Acceleration);
        }

        // Stops the plane spinning forever after input is released.
        private void ApplyAngularDamping()
        {
            Vector3 localAngVel = transform.InverseTransformDirection(rb.angularVelocity);
            Vector3 damp = -Vector3.Scale(localAngVel, angularDamping);
            rb.AddRelativeTorque(damp, ForceMode.Acceleration);
        }

        // ---------- Unity ----------

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.mass = airSpecifications.aircraftMass;
            controlParameters = airSpecifications.controlParameters;
            aerodynamicParameters = airSpecifications.aerodynamicParameters;
            engineParameters = airSpecifications.engineParameters;
            aircraftThrust = GetComponent<AircraftThrust>();
            lastVelocity = rb.linearVelocity;
        }

        private void FixedUpdate()
        {
            UpdateGForce();
            ApplyThrust();
            ApplyAerodynamics();
            ApplyPitchTorque();
            ApplyRollTorque();
            ApplyYawTorque();
            ApplySideslip();
            ApplyNoseStall();
            ApplyAngularDamping();
        }

        private void OnDrawGizmosSelected()
        {
            if (rb == null) return;
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.TransformDirection(liftDirection) * 3f);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.TransformDirection(dragDirection) * 3f);
        }
    }
}