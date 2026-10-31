using UnityEngine;
using GameInfrastructure;
using Aircraft;
public class RegisterOriginShift : MonoBehaviour
{
    private Camera playerCamera;
    [SerializeField] private bool isPhysicalOrMovingObject;
    [SerializeField] private AeroBody body;
    [SerializeField] private Rigidbody rb;
    public OriginShift originShift;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    Vector3 recordedVelocity;
    Vector3 recoredAngularVelocity;


    void Awake()
    {
        originShift = GameObject.FindAnyObjectByType<OriginShift>();
        originShift.AddToShifter(this.transform);
        if (isPhysicalOrMovingObject)
        {
            rb = GetComponent<Rigidbody>();
            playerCamera = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isPhysicalOrMovingObject)
        {
            recordedVelocity = rb.linearVelocity;
            recoredAngularVelocity = rb.angularVelocity;

            Vector3 distance = playerCamera.transform.InverseTransformDirection(transform.position);

            if (distance.magnitude > originShift.Limit)
            {
                rb.isKinematic = false;


            }
            else
            {
                rb.isKinematic = true;
                transform.position += recordedVelocity * Time.deltaTime;
                transform.rotation *= Quaternion.Euler(recoredAngularVelocity);
            }

        }
    }
}
