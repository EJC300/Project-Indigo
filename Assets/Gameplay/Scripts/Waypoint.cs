using UnityEngine;

namespace Gameplay
{
    public class Waypoint : MonoBehaviour
    {

        [SerializeField] private Transform terrain;
        public WaypointTrackerHUD tracker;
        bool passed;


       
        private void Update()
        {
        
           
        }
        private void Awake()
        {
           
        }
        private void Start()
        {
           transform.SetParent(terrain);
        }
        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                tracker.IncreaseIndex();
                Debug.Log("Passed");
                passed = true;
               
            }
          
        }
    }
}
