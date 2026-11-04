using UnityEngine;
namespace Gameplay
{
    public class DemoBombLauncher : MonoBehaviour
    {
        [SerializeField] private TargetTrackerDemo targetTracker;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Transform pylonLeft;
        [SerializeField] private Transform pylonRight;
        [SerializeField] private int ammo;
        [SerializeField] private GameObject bombPrefab;
        private bool launch;

        private void Start()
        {
            targetTracker = GameObject.FindAnyObjectByType<TargetTrackerDemo>();
        }
        public void DropBomb(bool fire)
        {
            if (fire && ammo > 0)
            {
                ammo = 0;
                launch = true;
                BasicSmartBomb basicSmartBomb0 = Instantiate(bombPrefab, pylonLeft.position, Quaternion.identity).GetComponent<BasicSmartBomb>();
                BasicSmartBomb basicSmartBomb1 = Instantiate(bombPrefab, pylonLeft.position, Quaternion.identity).GetComponent<BasicSmartBomb>();
                basicSmartBomb0.SetPlatformVelocity(rb.linearVelocity);
                basicSmartBomb1.SetPlatformVelocity(rb.linearVelocity);
                basicSmartBomb0.targetPosition = targetTracker.targetPoint;
                basicSmartBomb1.targetPosition = targetTracker.targetPoint;
                basicSmartBomb0.BeginFlight(launch);
                basicSmartBomb1.BeginFlight(launch);
            }
        }



    }

}