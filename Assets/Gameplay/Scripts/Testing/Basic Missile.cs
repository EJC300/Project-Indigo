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
    [SerializeField] private float navConstant = 3.0f;

    [SerializeField] private float missileSpeed;

    [SerializeField] private float missileTurnRate;

    [SerializeField] private Rigidbody rb;

    [SerializeField] private Vector3 targetPosition;

    [SerializeField] private Vector3 previousTargetPosition;

    [SerializeField] private Vector3 targetVelocity;

    [SerializeField] private Vector3 previousTargetVelocity;

    [SerializeField] private Vector3 targetAcceleration;

    private Vector3 previousTargetAcceleration;
    private Vector3 relativeVelocity;
    private void Update()
    {
       targetPosition = target.transform.position;
    }
    private void FixedUpdate()
    {
        TargetAcceleration();
        RelativeVelocity();
        Debug.Log((AccelerationCommand() - transform.position).normalized);
       // rb.AddRelativeForce(Vector3.forward * missileSpeed, ForceMode.Acceleration);
        Vector3 toTarget = target.transform.position - rb.position;
        Vector3 velocity = (AccelerationCommand());
        Vector3 direction = toTarget.normalized + velocity;
        Debug.Log(direction.normalized * missileTurnRate);
        transform.rotation = Quaternion.LookRotation(direction.normalized  * missileTurnRate);
        previousTargetPosition = targetPosition;
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
        Vector3 currentPosition = transform.position + targetVelocity;
        targetPosition = target.transform.position;

        Vector3 lineOfSight = targetPosition - currentPosition;

        Vector3 relativeVelocity = targetVelocity - rb.linearVelocity;

        Vector3 losRate = Vector3.Cross(relativeVelocity, lineOfSight.normalized);

        float closingVelocity = Vector3.Dot(losRate, relativeVelocity) / lineOfSight.magnitude;


        return navConstant * (closingVelocity) * losRate.normalized;



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
}
