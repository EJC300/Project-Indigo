using UnityEngine;

public class TestLift : MonoBehaviour
{
    //Lift must lift the body in the air
    //Then lift the body in the air based on AOA

    

    public Rigidbody body;
    public float liftStrength;

    private float AOA;

    private Vector3 localAirSpeed;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    //This is a test testing out aircraft lift. Total Throway Code
    private void FixedUpdate()
    {
        
        localAirSpeed = transform.InverseTransformDirection(body.linearVelocity);
        Vector3 velocity = body.linearVelocity;

        Vector3 direction = body.linearVelocity.normalized;
        AOA = Mathf.Atan2(localAirSpeed.y, localAirSpeed.z) * Mathf.Rad2Deg;
        AOA = Mathf.Clamp(AOA, -15, 15);
       
        Vector3 liftDirection = Vector3.Cross(direction, transform.right).normalized;
         float squaredAirSpeed = (localAirSpeed.magnitude * localAirSpeed.magnitude);
         float lift = -AOA *(0.5f * squaredAirSpeed) * liftStrength;
          float liftTorque =   (Input.GetAxis("Horizontal")) * lift * Time.fixedDeltaTime;
          Vector3 adverseYawDirection = Vector3.Cross((liftDirection * lift).normalized, transform.forward).normalized * 5f;

        
       
        body.AddForce(liftDirection * lift);
        body.AddRelativeTorque(adverseYawDirection.normalized.magnitude * -transform.up);
        body.AddRelativeTorque(liftTorque * ( 2*-transform.forward -transform.up));
    }

}
