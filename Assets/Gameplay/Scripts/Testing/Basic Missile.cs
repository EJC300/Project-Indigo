using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class BasicMissile : MonoBehaviour
{
    /*
     * Basic Missile PN 
     * With no aircraft physics for testing
     * 
     */
    public GameObject target;

    [SerializeField] private float seekerActivationTime;

    [SerializeField] private float burnTime;
    [SerializeField] private float navConstant = 3.0f;

    [SerializeField] private float missileSpeed;

    [SerializeField] private float missileTurnRate;

    [SerializeField] private Rigidbody rb;

    [SerializeField] private Vector3 targetPosition;

    [SerializeField] private Vector3 previousTargetPosition;

    [SerializeField] private Vector3 targetVelocity;

    [SerializeField] private Vector3 previousTargetVelocity;

    [SerializeField] private Vector3 targetAcceleration;

    private float burnSeconds;
    private float seekerSeconds;

    private Vector3 relativeVelocity;
    private Vector3 velocity;
    private void Update()
    {
       targetPosition = target.transform.position;
    }
    private void Start()
    {
        seekerSeconds = seekerActivationTime;
        burnSeconds = burnTime;
    }
    private void FixedUpdate()
    {
        

        TargetAcceleration();
        RelativeVelocity();
        Debug.Log((AccelerationCommand() - transform.position).normalized);
        ThrustBurn();
        Seek();
    }


    void Seek()
    {
        if(seekerSeconds < 0 && velocity.magnitude > 0)
        {
            Vector3 toTarget = target.transform.position - rb.position;
            Vector3 velocity = (AccelerationCommand());
            Vector3 direction = toTarget.normalized + velocity;
            Debug.Log(direction.normalized * missileTurnRate);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(direction.normalized * missileTurnRate * Time.fixedDeltaTime),Time.fixedDeltaTime * 2/velocity.normalized.magnitude));
            previousTargetPosition = targetPosition;
            Debug.Log(rb.linearVelocity.magnitude);
        }
        else if (seekerSeconds > 0)
        {
            seekerSeconds -= Time.fixedDeltaTime;
        }
    }

    void ThrustBurn()
    {
        velocity = rb.linearVelocity;
        if (burnSeconds > 0)
        {
            burnSeconds -= Time.fixedDeltaTime;
            rb.AddRelativeForce(Vector3.forward * missileSpeed, ForceMode.Force);
        }
    }
    void TargetAcceleration()
    {
        targetVelocity = (previousTargetPosition - targetPosition) * Time.fixedDeltaTime;

        targetAcceleration = (previousTargetVelocity - targetVelocity) * Time.fixedDeltaTime;

        previousTargetVelocity = targetVelocity;
    }

    void RelativeVelocity()
    {
        relativeVelocity = targetVelocity - rb.linearVelocity;

    }

    Vector3 AccelerationCommand()
    {
        Vector3 currentPosition = transform.position;
        targetPosition = target.transform.position + targetVelocity;

        Vector3 relativePosition = targetPosition - transform.position;

        Vector3 relativeVelocity = targetVelocity - (rb.linearVelocity);

        float timeUntilImpact = (relativeVelocity.magnitude / targetVelocity.magnitude);

        Vector3 zeroEffortMiss = relativePosition + relativeVelocity * timeUntilImpact;

        Vector3 unitLOS = relativePosition.normalized;

        Vector3 zeroEffortMissLOS = Vector3.Project(zeroEffortMiss, unitLOS);

        Vector3 zeroEffortNormal = zeroEffortMiss - zeroEffortMissLOS;

        Vector3 acceleration = navConstant * zeroEffortNormal / (timeUntilImpact * timeUntilImpact + AccelerationBias());


        return acceleration;



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
            return acceleration * n;
        }
       
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
