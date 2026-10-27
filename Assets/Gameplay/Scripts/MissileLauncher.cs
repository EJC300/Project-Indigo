using UnityEngine;
namespace Gameplay
{
    public class MissileLauncher : MonoBehaviour
    {
        [SerializeField] private float launchSpeed;
        [SerializeField] private Transform missilePoint;
        [SerializeField] private GameObject missilePrefab;
        public Rigidbody target;
        public bool Fire;
        private Vector3 CalculateLaunchVelocity()
        {
            Vector3 launchPoint = transform.InverseTransformPoint(missilePoint.position);

            Vector3 launchDirection = launchPoint + missilePoint.up;

            Vector3 launchForce =  launchDirection.normalized * launchSpeed;

            return launchForce;
        }


        void FireMissile()
        {
            Vector3 launchVelocity = CalculateLaunchVelocity();
            GameObject missileInstance = GameObject.Instantiate(missilePrefab,missilePoint.position,missilePoint.localRotation);
            Rigidbody missileRB = missileInstance.GetComponent<Rigidbody>();
            MissileGuidance missileGuidance = missileInstance.GetComponent<MissileGuidance>();
            missileGuidance.target = target.gameObject;
            missileRB.AddRelativeForce(launchVelocity,ForceMode.Impulse);

        }

        private void FixedUpdate()
        {
            //Testing 
            if (Fire)
            {
                Fire = false;
                FireMissile();
            }
        }

    }
}
