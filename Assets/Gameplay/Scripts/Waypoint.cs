using UnityEngine;
using GameInfrastructure;
namespace Gameplay
{
    public class Waypoint : MonoBehaviour
    {

        [SerializeField] private Transform terrain;
        [SerializeField] private bool endMission;
        [SerializeField] PauseModalWindow pauseMenuController;
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
                if (endMission)
                {
                    pauseMenuController.OnOpenPauseWindow();
                }
            }
          
        }
    }
}
