using UnityEngine;

public class BasicSmartBomb : MonoBehaviour
{
    [Header("Target & Seeker")]
    public Vector3 targetPosition;
    [SerializeField] private float seekerActivationTime;

    [Header("Flight & Guidance")]
   // [SerializeField] private float burnTime;
    [SerializeField] private float navConstant = 3.0f;
   // [SerializeField] private float missileSpeed;
    [SerializeField] private float missileTurnRate;

    private Vector3 platformVelocity;

    [Header("References")]
    [SerializeField] private Rigidbody rb;

    // Tracking variables

    private Vector3 previousTargetPosition;
    private Vector3 targetVelocity;
    private Vector3 previousTargetVelocity;
    private Vector3 targetAcceleration;

    private float seekerSeconds;




    public void SetPlatformVelocity(Vector3 velocity)
    {
        platformVelocity = velocity;
    }




    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();


        seekerSeconds = seekerActivationTime;
       

      
           
            previousTargetPosition = targetPosition;
        
    }

    private void Update()
    {
       
      
        
        Debug.Log(rb.linearVelocity.magnitude);
    }

    private void FixedUpdate()
    {
       
        TargetAcceleration();
        Seek();
    }

    public void BeginFlight(bool launch)
    {
        if (launch)
        {
            rb.linearVelocity = platformVelocity;
            launch = false;
        }
    }
    private void Seek()
    {
        if (seekerSeconds <= 0)
        {
            Vector3 accelCmd = AccelerationCommand();

            // Combine current velocity/forward with the guidance acceleration command
            Vector3 desiredDirection = (rb.linearVelocity + accelCmd).normalized;
            if (desiredDirection == Vector3.zero)
            {
                desiredDirection = transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(desiredDirection);
            float turnStep = missileTurnRate * Time.fixedDeltaTime;

            // Smoothly rotate towards the target guidance vector
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRotation, turnStep));
        }
        else
        {
            seekerSeconds -= Time.fixedDeltaTime;
        }
    }



    private void TargetAcceleration()
    {
        float dt = Time.fixedDeltaTime;
        if (dt <= 0f) return;

        // Correct velocity and acceleration finite-difference math (divided by dt, not multiplied)
        targetVelocity = (targetPosition - previousTargetPosition) / dt;
        targetAcceleration = (targetVelocity - previousTargetVelocity) / dt;

        // Update previous states for next frame
        previousTargetVelocity = targetVelocity;
        previousTargetPosition = targetPosition;
    }

  

    private Vector3 AccelerationCommand()
    {
        Vector3 relativePosition = targetPosition - transform.position;
        Vector3 relVel = targetVelocity - rb.linearVelocity;

        // 1. Estimate time until impact (time-to-go)
        float closingSpeed = -Vector3.Dot(relVel, relativePosition.normalized);
        float timeUntilImpact = closingSpeed > 0.01f ? relativePosition.magnitude / closingSpeed : 1f;
        timeUntilImpact = Mathf.Clamp(timeUntilImpact, 0.05f, 5.0f); // Safety clamps

        // 2. PREDICTIVE LEAD PURSUIT: Extrapolate future target position
        // Includes velocity and acceleration over the time-to-go window
        Vector3 predictedTargetPosition = targetPosition
            + (targetVelocity * timeUntilImpact)
            + (0.5f * targetAcceleration * (timeUntilImpact * timeUntilImpact));

        // 3. Recompute relative position and zero-effort miss using the predicted lead point
        Vector3 predictedRelativePos = predictedTargetPosition - transform.position;
        Vector3 zeroEffortMiss = predictedRelativePos + relVel * timeUntilImpact;

        Vector3 unitLOS = predictedRelativePos.normalized;
        Vector3 zeroEffortMissLOS = Vector3.Project(zeroEffortMiss, unitLOS);
        Vector3 zeroEffortNormal = zeroEffortMiss - zeroEffortMissLOS;

        // 4. Compute final guidance command with acceleration bias
        float bias = AccelerationBias();
        float denominator = (timeUntilImpact * timeUntilImpact + bias);
        if (Mathf.Abs(denominator) < 0.0001f) denominator = 0.0001f;

        Vector3 acceleration = navConstant * zeroEffortNormal / denominator;
        return acceleration;
    }

    private float AccelerationBias()
    {
        float n = navConstant / 2f;
        float acceleration = targetAcceleration.magnitude;
        float speed = targetVelocity.magnitude;

        if (speed > 0.001f)
        {
            float delta = acceleration / speed;
            return n * delta;
        }
        else
        {
            return acceleration * n;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
