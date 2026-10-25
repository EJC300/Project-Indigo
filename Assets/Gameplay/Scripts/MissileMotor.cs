using UnityEngine;
namespace Gameplay {
    public class MissileMotor : MonoBehaviour
    {

        private Rigidbody rb;
        [SerializeField] private float totalBurnTimeConstant;

        [SerializeField] private float specificImpulse;
        [SerializeField] private float fuelMassInKg;
        private float gravity = 9.81f;

    

        private float initialMass;

       public float currentMass;
        float fuelBurnRate;
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
              
            initialMass = rb.mass;
            currentMass = rb.mass;
            fuelBurnRate = currentMass;
        }

        float CalculateDeltaV()
        {
          
                fuelBurnRate = (initialMass - currentMass) / totalBurnTimeConstant;

                currentMass -= fuelBurnRate + 0.1f;
            currentMass = Mathf.Clamp(currentMass, 0.0f, initialMass);
         
            
                float deltaV = (currentMass / initialMass) * gravity * specificImpulse;
            if (currentMass > 0.0)
            {
                float thrust = deltaV / currentMass;
                return thrust;




            }
            return 0;
        }

        private void FixedUpdate()
        {

            float thrust = CalculateDeltaV();
         
            Vector3 deltaVAcceleration = Vector3.forward * thrust;
    
       
            rb.AddRelativeForce(deltaVAcceleration,ForceMode.Impulse);
        }

    }
}
