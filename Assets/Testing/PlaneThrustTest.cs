using UnityEngine;

public class PlaneThrustTest : MonoBehaviour
{
     Rigidbody rb;
    [SerializeField] private float thrustSpeed;
    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce(transform.forward  * thrustSpeed * rb.mass);
     //   Debug.Log(rb.linearVelocity.magnitude);
    }
}
