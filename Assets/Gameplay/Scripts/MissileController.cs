using Aircraft;
using GameInfrastructure;
using System.Net.NetworkInformation;
using UnityEngine;
public class MissileController : Updatable
{

    public bool testing;
    private void OnEnable()
    {

    }
    private void OnDisable()
    {
        if (!testing)
        {
            GameFlow.instance.RemoveUpdatable(this);
        }
    }
    private AeroBody body;
    private Rigidbody rb;

    [SerializeField] private float maxG;
    [SerializeField] private float minG;

    [SerializeField] private ControlSurfaceAnimatable aileronLeft;
    [SerializeField] private ControlSurfaceAnimatable aileronRight;
    [SerializeField] private ControlSurfaceAnimatable elevatorLeft;
    [SerializeField] private ControlSurfaceAnimatable elevatorRight;
    [SerializeField] private float maxManeuveringSpeed = 500;
    [SerializeField] private float minManeuveringSpeed = 50;
    [SerializeField] private float maxDeflectionAnglePitch;
    [SerializeField] private float minDeflectionAnglePitch;
    [SerializeField] private float maxDeflectionAngleYaw;
    [SerializeField] private float minDeflectionAngelYae;
    [SerializeField] private float maxDeflectionAngleRoll;
    [SerializeField] private float minDeflectionAngleRoll;




    private float pitchInputAngle;
    private float yawInputAngle;
    private float rollInputAngle;
    float pitchTarget = 0;
    private bool speedBrakes;
    private float acceleration;
    private float currentG;
    private float previousG;
    private float yawInput;
    private float rollInput;
    private float currentThrust;
    private float throttle;
    private float deflectionAnglePitch;
    private float deflectionAngleYaw;
    private float deflectionAngleRoll;
    private float newPitchInput;
    private float yawForce;
    private float pitchForce;
    private Vector3 lastVelocity;
    private float gravityMagnitude = 9.81f;

    float currentVelocity;
    private Vector3 CalculateGForce()
    {

        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 acceleration = (currentVelocity - lastVelocity) / Time.fixedDeltaTime;



        acceleration -= Physics.gravity;

        lastVelocity = currentVelocity;
        Vector3 localAcceleration = transform.InverseTransformDirection(acceleration);


        return localAcceleration / gravityMagnitude;
    }

    public float GetGforce()
    {
        return currentG;
    }
    private void GLimit(float input)
    {


        float gForce = CalculateGForce().y;
        currentG = gForce;







        Debug.Log("GLimit");
        float controlMultiplier = Mathf.InverseLerp(maxG, minG, Mathf.Abs(currentG));

        newPitchInput = Mathf.Lerp(newPitchInput, controlMultiplier, controlMultiplier);






    }




    public float GetYawForce()
    {
        return yawForce;
    }

    public float GetPitchForce()
    {
        return pitchForce;
    }




    void AnimateControlSurfaces()
    {



        elevatorLeft.RotateToTargetAngle(Vector3.right, pitchInputAngle);


        elevatorRight.RotateToTargetAngle(Vector3.right, pitchInputAngle);

        aileronLeft.RotateToTargetAngle(Vector3.right, rollInputAngle);

        aileronRight.RotateToTargetAngle(Vector3.right, -rollInputAngle);
    }
  
    private void PitchYawRoll()
    {
        float speed = Mathf.InverseLerp(maxManeuveringSpeed, minManeuveringSpeed, body.GetLinearVelocity().z);

        deflectionAnglePitch = Mathf.Lerp(minDeflectionAnglePitch, maxDeflectionAnglePitch, speed);

        pitchInputAngle = Mathf.Clamp(pitchInputAngle, -deflectionAnglePitch, deflectionAnglePitch);

        pitchForce = pitchInputAngle * body.GetPressure;

 


       // deflectionAngleRoll = Mathf.Lerp(maxDeflectionAngleRoll, minDeflectionAngleRoll, speed);
      //  rollInputAngle = Mathf.Clamp(rollInputAngle, -deflectionAngleRoll, deflectionAngleRoll);

       // float rollForce = rollInputAngle * body.GetPressure;
        deflectionAngleYaw = Mathf.Lerp(maxDeflectionAngleYaw, minDeflectionAngelYae, speed);
        yawInputAngle = Mathf.Clamp(yawInputAngle, -deflectionAngleYaw, deflectionAngleYaw);
        yawForce = yawInputAngle * body.GetPressure;
        
        rb.AddRelativeTorque(Vector3.right * pitchForce);
        rb.AddRelativeTorque(Vector3.up * yawForce);
    }




    public void SetYaw(float input)
    {
        if (Mathf.Abs(input) <= 0)
        {
            yawInputAngle = 0;
        }
        else
        {
            yawInputAngle += input * 45 * Time.fixedDeltaTime;
        }

    }


    public void SetPitch(float input)
    {

        GLimit(input);


        if ((input) == 0)
        {
            pitchTarget = 0;
        }
        else
        {
            pitchTarget += input * 45 * Time.fixedDeltaTime;
        }
        float maxAllowedAngle = pitchTarget * newPitchInput;

        if (pitchTarget > 0) pitchTarget = Mathf.Min(pitchTarget, maxAllowedAngle);
        else pitchTarget = Mathf.Max(pitchTarget, maxAllowedAngle);
        pitchInputAngle = Mathf.MoveTowards(pitchInputAngle, pitchTarget, Time.fixedDeltaTime * 15);
    }

    public void SetRoll(float input)
    {

        if (Mathf.Abs(input) <= 0)
        {
            rollInputAngle = 0;
        }
        else
        {
            rollInputAngle += input * Time.fixedDeltaTime * 45;
        }
    }



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        body = GetComponent<AeroBody>();
        if (!testing)
        {
            GameFlow.instance.AddUpdatable(this);
            lastVelocity = rb.linearVelocity;
        }
    }
    private void FixedUpdate()
    {
        if (testing)
        {
            Debug.Log("PlaneController");
         
            PitchYawRoll();
          
            AnimateControlSurfaces();
        }
    }
    public override void CustomStart()
    {


    }

    public override void CustomUpdate()
    {
      

    }
    public override void CustomFixedUpdate()
    {

        if (!testing)
        {



       
            PitchYawRoll();
       
            AnimateControlSurfaces();


        }
    }










}

