using UnityEngine;
namespace Gameplay
{
    public class MissileLauncher : MonoBehaviour
    {

        public bool fire;
        [SerializeField] private GameObject particleSmoke;
        [SerializeField] private float launchSpeed;
        [SerializeField] private Transform missilePoint;
        [SerializeField] private GameObject missilePrefab;
        public Rigidbody target;
         
      

        private Vector3 CalculateLaunchVelocity()
        {
            Vector3 launchPoint = transform.InverseTransformPoint(missilePoint.position);

            Vector3 launchDirection =- missilePoint.up;

            Vector3 launchForce =  launchDirection.normalized * launchSpeed;

            return launchForce;
        }


        void FireMissile()
        {
            Vector3 launchVelocity = CalculateLaunchVelocity();
            GameObject missileInstance = GameObject.Instantiate(missilePrefab,missilePoint.position,missilePoint.localRotation);
            Rigidbody missileRB = missileInstance.GetComponent<Rigidbody>();
            BasicMissile missileGuidance = missileInstance.GetComponent<BasicMissile>();
            missileGuidance.target = target.gameObject;
            missileRB.AddRelativeForce(launchVelocity,ForceMode.Impulse);
            Instantiate(particleSmoke, missilePoint, false);
            
            
        }

        private void Update()
        {
          
        }

        private void FixedUpdate()
        {
            //Testing 
            if (fire)
            {
                fire = false;
                FireMissile();
            }
        }

    }
}
