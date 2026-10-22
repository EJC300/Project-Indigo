using UnityEngine;
using Utilities;
using GameInfrastructure;
using UnityEngine.UIElements;
namespace Aircraft
{
    [RequireComponent(typeof(Rigidbody))]
    public class AeroBody : Updatable
    {





        /*
         * Adds Drag and lift to the a rigidbody 
         * 
         * Returns angle of attack and pressure for control authority
         * 
         * can be made for planes or missiles
         */
        public bool testing;
        public bool activate;
        //Forces 

        private Rigidbody rb;
        public Rigidbody RB;
        private Vector3 localVelocity;
        private Vector3 localAngularVelocity;
        private Vector3 speed;
       public Vector3 prevVelocity;
        //Lift 
        [SerializeField] private float wingArea;
        [SerializeField] private float wingAreaYaw;
        [Range(0.0f, 1.0f)]
        [SerializeField] private float liftCoefficient;
        [Range(0.0f, 1.0f)]
        [SerializeField] private float liftCoefficientYaw;
        [Range(0.0f, 1.0f)]
        [SerializeField] private float rho;
        private float coefficientOfLift;
        private float coefficientOfLiftYaw;
        private float angleOfAttack;
        private float angleOfAttackYaw;
        private float maxStallSpeed;
        private float minStallSpeed;
        private float loadFactor;
        [Range(0.0f, 360.0f)]
        [SerializeField]
        private float maxAngleAttack;
        [Range(0.0f, 360.0f)]
        [SerializeField]
        private float maxAngleAttackYaw;
        private float pressure;
        //Drag
        [Range(0.0f, 1.0f)]
        [SerializeField] private float aspectRatio;

        [Range(0.0f, 1.0f)]
        [SerializeField] private float efficiencyFactor;
        [Range(0.0f, 1.0f)]
        [SerializeField] private float dragUpCoefficient, dragForwardCoefficient, dragLeftCoefficient;

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
        private void UpdateState()
        {
            localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
            localAngularVelocity = transform.InverseTransformVector(rb.angularVelocity) * Mathf.Deg2Rad;
            CalculateLift();
            CalculateDrag();
            CalculateStall();
        }
        private void CalculateDrag()
        {
            Vector3 dragForce = Vector3.zero;

            dragForce.x = localVelocity.x * -dragLeftCoefficient;
            dragForce.y = localVelocity.y * -dragUpCoefficient;
            dragForce.z = localVelocity.z * -dragForwardCoefficient;

            dragForce *= pressure;
            dragForce = transform.TransformDirection(dragForce);
            rb.AddForce(dragForce);
        }
        private void CalculateLift()
        {

            angleOfAttack = Mathf.Atan2(-localVelocity.y, localVelocity.z) * Mathf.Rad2Deg;

            angleOfAttackYaw = Mathf.Atan2(localVelocity.x, localVelocity.z) * Mathf.Rad2Deg;

            coefficientOfLift = MathUtilities.UnclampedInverseLerp(-maxAngleAttack, maxAngleAttack, angleOfAttack) * liftCoefficient;
            coefficientOfLiftYaw = MathUtilities.UnclampedInverseLerp(-maxAngleAttack, maxAngleAttack, angleOfAttackYaw) * liftCoefficientYaw;

            Vector3 relativeWind = localVelocity.z * Vector3.up;
            Vector3 relativeWindYaw = localVelocity.z * Vector3.right;

            pressure = rho * rb.linearVelocity.sqrMagnitude;



            Vector3 liftForce = relativeWind.normalized * pressure * 0.5f * (coefficientOfLift * coefficientOfLift) * wingArea;

            Vector3 liftForceYaw = relativeWindYaw.normalized * pressure * (0.5f * (coefficientOfLift * coefficientOfLift)) * wingArea;

            Vector3 liftTotalForce = (liftForce);

            float inducedDragCoefficient = (coefficientOfLift * coefficientOfLift) / Mathf.PI * aspectRatio * efficiencyFactor;
            Vector3 inducedDragForce = -(localVelocity.z * transform.forward).normalized * inducedDragCoefficient;
            Vector3 inducedDragForceYaw = -(localVelocity.z * transform.right).normalized * inducedDragCoefficient;
            Vector3 totalForce = (liftTotalForce + inducedDragForce + inducedDragForceYaw);
            totalForce = transform.TransformDirection(totalForce);
            rb.AddForce(totalForce, ForceMode.Force);


        }
        void CalculateStall()
        {
            float coef = (rho * wingArea) * coefficientOfLift;
         
            if (Mathf.Abs(coef) > 0.5)
            {
                maxStallSpeed = Mathf.Sqrt((2 * rb.mass) / coef);

                minStallSpeed = (2 * rb.mass) / coef;
            }
        }

        public override void SetRigidbody(bool setActive)
        {

            if (!setActive)
            {



                
                activate = true;
          
            }
     
          

         


        }

        //Return angle of attack and pressure

       
        public override void CustomStart()
        {
            if (!testing)
            {
                rb = GetComponent<Rigidbody>();
                RB = rb;
            }
         
        }
        private void FixedUpdate()
        {
            if (testing)
            {
                UpdateState();
            }
        }
        public override void CustomFixedUpdate()
        {

            if (!GameFlow.instance.paused && !testing)
            {   
                rb.isKinematic = false;
              
            

           
                

                UpdateState();
             
            }
           


        }
        public override void CustomLateUpdate()
        {
            
        }
        public override void CustomUpdate()
        {
           

        }
        public void Start()
        {
            if (!testing)
            {
                GameFlow.instance.AddUpdatable(this);

            }
            rb = GetComponent<Rigidbody>();
            RB = rb;
        }

        public float GetAngleOfAttack()
        {
            return angleOfAttack;
        }
        public float GetAngleOfAttackYaw()
        {
            return angleOfAttackYaw;
        }


        public float GetPressure
        {
            get { return pressure; }
        }
        public float GetWingSurfaceArea()
        {
            return wingArea;
        }
        public Vector3 GetLinearVelocity()
        {
            return localVelocity;
        }

        public Vector3 GetAngularVelocity()
        {
            return localAngularVelocity;
        }
        public float GetMinStallSpeed()
        {
            return Mathf.Max(1, minStallSpeed);
        }
        public float GetMaxStallSpeed()
        {
           
            return Mathf.Max(1, maxStallSpeed);
        }
    }
}
