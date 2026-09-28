using UnityEngine;

public class AtmosphericDragTest : MonoBehaviour
{
    public Rigidbody rb;
    //like all tests these will put in a parameter script

    public float topDrag;
    public float bottomDrag;
    public float horizontalDrag;
    public float forwardDrag;
    public float backwardDrag;

   



    private void FixedUpdate()
    {
        Vector3 velocity = (rb.linearVelocity);
        Vector3 dragForward = -0.5f * (velocity.z * velocity.z) * (Vector3.Dot(velocity.normalized, transform.forward) * -transform.forward);
        Vector3 dragBackward = -0.5f * (velocity.z * velocity.z) * (Vector3.Dot(velocity.normalized, -transform.forward) * -transform.forward);
        Vector3 horizontal = -0.5f * (velocity.x * velocity.x) * (Vector3.Dot(velocity.normalized, transform.right) * transform.right);
      
        Vector3 totalDrag = dragForward + dragBackward + horizontal;
        Debug.Log(totalDrag);
        Debug.DrawRay(transform.position, totalDrag);
        rb.AddForce(totalDrag * rb.mass,ForceMode.Force);
    }

}
