using UnityEngine;


public class WingTest : MonoBehaviour
{

    [SerializeField] private Rigidbody rb;

    public float liftEfficiency = 1.0f;
    
    private void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        if (rb.GetPointVelocity(rb.linearVelocity).magnitude < Mathf.Epsilon) return;
        float q = rb.GetRelativePointVelocity(rb.linearVelocity).sqrMagnitude;
        float absoluteAOA = Vector3.Dot(rb.GetPointVelocity(rb.linearVelocity), -transform.up) * Mathf.Rad2Deg;
        float liftCoeff = 0.5f * q * liftEfficiency;
       
        liftCoeff = Mathf.Clamp(liftCoeff, 0.0f, 160000);
        Debug.Log(liftCoeff);
        float dragCoeff = liftCoeff * liftEfficiency;

        Debug.Log(dragCoeff);
        
     
        Vector3 liftDirection = Vector3.Cross(Vector3.Cross( -rb.GetPointVelocity(transform.position).normalized,transform.up).normalized, -rb.GetPointVelocity(transform.position).normalized).normalized;
        Vector3 dragForce = -rb.GetPointVelocity(transform.position).normalized * dragCoeff;
        Debug.DrawRay(transform.position, liftDirection*liftCoeff,Color.red);
        rb.AddForceAtPosition(liftDirection * ( liftCoeff) +dragForce, transform.position);
        
    }



}
