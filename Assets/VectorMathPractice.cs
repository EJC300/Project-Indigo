using UnityEngine;

public class VectorMathPractice : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Rigidbody rb;
    public float speed = 0.5f;
    public Vector3 euler;
    public Vector3 velocity;
    public bool localVelocityToggle;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (localVelocityToggle)
        {
            rb.linearVelocity = transform.InverseTransformDirection(velocity);
        }
        else
        {
            rb.linearVelocity =  transform.TransformDirection( velocity);
        }

        Debug.Log(transform.InverseTransformDirection(velocity));
        rb.MoveRotation(Quaternion.Euler(euler));
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position,transform.up);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right);

    }
}
