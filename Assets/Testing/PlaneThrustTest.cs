using UnityEngine;

public class PlaneThrustTest : MonoBehaviour
{
     Rigidbody rb;
    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce(transform.forward  * 25 * rb.mass);
        Debug.Log(rb.linearVelocity.magnitude);
    }
}
