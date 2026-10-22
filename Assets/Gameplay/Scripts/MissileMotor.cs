using UnityEngine;
namespace Gameplay {
    public class MissileMotor : MonoBehaviour
    {
        [SerializeField] private float fuel;

        [SerializeField] private float fuelPower;
        private float deltaV;
        private Rigidbody rb;
        private float seconds;
       

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            CalculateDeltaV();
            if (seconds != deltaV)
            {
                seconds = deltaV;
            }
        }

        void CalculateDeltaV()
        {
            //Simple very simple deltaV timer
            float mass = rb.mass;

            float fuelByMass = fuel / mass;
            deltaV = fuelByMass / Time.fixedDeltaTime;
        }
        
        private void FixedUpdate()
        {
       
            if (seconds > 0)
            {
                seconds -= fuelPower * Time.fixedDeltaTime;
               
           
               
            }
            rb.AddRelativeForce(transform.up * fuelPower, ForceMode.Impulse);

        }

    }
}
