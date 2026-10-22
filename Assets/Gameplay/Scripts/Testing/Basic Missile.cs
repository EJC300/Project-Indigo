using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

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

    private Vector3 targetPosition;

    private Vector3 previousTargetPosition;

    private Vector3 targetVelocity;

    private Vector3 previousTargetVelocity;

    private Vector3 targetAcceleration;

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
        rb.AddRelativeForce(Vector3.forward * missileSpeed, ForceMode.Acceleration);
        Vector3 toTarget = target.transform.position - rb.position;
        Vector3 velocity = (AccelerationCommand());
        Vector3 direction = toTarget.normalized + velocity;
        transform.rotation = Quaternion.LookRotation(direction  * Time.fixedDeltaTime * missileTurnRate);
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
        Vector3 lead = target.transform.position + targetVelocity;
        Vector3 toTarget = lead;
       float distance = toTarget.sqrMagnitude;
        Vector3 los = toTarget.normalized;
        Vector3 relativeVelocity =  targetVelocity - rb.linearVelocity;
        float terminalVelocity = Vector3.Dot(relativeVelocity, los);
        Vector3 losRate = Vector3.Cross( relativeVelocity,los) / distance;
        Vector3 accelCommand = navConstant * terminalVelocity * losRate * AccelerationBias();
        return accelCommand;
    }
    float AccelerationBias()
    {
        float n = navConstant / 2;
        float acceleration = targetAcceleration.magnitude;
        float speed = rb.linearVelocity.magnitude;
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
