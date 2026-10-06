using UnityEngine;
using UnityEngine.UIElements;

public class LiftTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Vector3 localVelocity;
    private Rigidbody rb;
  
    public float liftPower;
    public float maxStallAngle = 15;
    public float maxStallSpeed = 100;
    Vector3 angularVelocity;
    Vector3 previousAngularVelocity;
    Vector3 control;
    public float aoaDeg;
    public float inducedDragFactor;


   
    public void DirectTorque(float pitch,float roll)
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
        float speedSquared = 0.5f * localVelocity.magnitude * localVelocity.magnitude;
        angularVelocity = transform.InverseTransformDirection( rb.angularVelocity);
        float pitchDiff =  (pitch * speedSquared  * 5 ) - ( previousAngularVelocity.x) * rb.mass ;
        float rollDiff = (roll * speedSquared * -15) - (previousAngularVelocity.z) * rb.mass;
        
         control = new Vector3(pitchDiff , -previousAngularVelocity.y * rb.mass, rollDiff);
        
       rb.AddRelativeTorque(control );
       previousAngularVelocity = angularVelocity;
    }

    //Test Stall
    void Stall()
    {
        Vector3 linearVelocity = rb.linearVelocity;
        float speed = localVelocity.magnitude ;
        bool atMaxStall = rb.linearVelocity.magnitude < maxStallSpeed && transform.localEulerAngles.x > maxStallAngle;
       
        if (atMaxStall)
        {
            Debug.Log("Stall");
            rb.AddRelativeTorque(-speed * liftPower * rb.mass * Vector3.right);
        }
    }
    //CalculateLift
    void ApplyLift()
    {
        Vector3 linearVelocity = rb.linearVelocity;
        Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
        Vector3 flightDirection = linearVelocity.normalized;
        Vector3 relativeFlightDirection = localVelocity.normalized;
        Vector3 liftDirection = Vector3.Cross(flightDirection, transform.right).normalized;
        float speedSquared = localVelocity.magnitude * localVelocity.magnitude;
        float aoa = Mathf.Atan2(-relativeFlightDirection.y, relativeFlightDirection.z);
        aoaDeg = aoa * Mathf.Rad2Deg;
       
        aoaDeg = Mathf.Clamp(aoaDeg, -maxStallAngle,maxStallAngle);
        float aoaFinal = Mathf.Abs(aoaDeg / maxStallAngle);
        float liftCoefficient = 0.5f * speedSquared * liftPower * aoaFinal;

        float maxLift = rb.mass * rb.mass * 0.25f;

        float totalLift = Mathf.Clamp(liftCoefficient, -maxLift, maxLift);

        Vector3 liftForce = totalLift * liftDirection;
        float sideSlip = Vector3.Dot((transform.right).normalized, Vector3.up) * -linearVelocity.magnitude ;
        float inducedDragLimit = totalLift;
        inducedDragLimit = Mathf.Lerp(inducedDragFactor, 0, speedSquared * 0.001f * Time.fixedDeltaTime);
       
        Debug.Log(inducedDragLimit);

        float inducedDrag = aoaFinal * totalLift * inducedDragFactor * rb.mass * inducedDragLimit * 0.01f;
        Vector3 inducedDragForce = inducedDrag * Vector3.Cross(liftDirection, transform.right);
        
   
        Vector3 weatherVeinEffect = rb.mass * 0.001f *  sideSlip * Vector3.up;
        Vector3 liftTorque =  Vector3.Cross(liftDirection,transform.up) * liftCoefficient * 0.05f;
        rb.AddRelativeTorque(weatherVeinEffect );
       
        rb.AddForce(liftForce + inducedDragForce);
    }
   
    private void Start()
    {
        
        rb = GetComponent<Rigidbody>();
       
    }
    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {

            rb.AddRelativeForce(Vector3.forward * 0.00f * rb.mass);
        }
        else
        {
            rb.AddRelativeForce(Vector3.forward * 45 * rb.mass);
        }

        //Needs to put the drag in the lift calculation then the test will be complete
        Stall();
        ApplyLift();
     

      


    }
}
