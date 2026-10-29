using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
namespace Gameplay
{
    public class WaypointTrackerHUD : MonoBehaviour
    {
        /*
         * Waypoints are placed on the map
         * only a single active waypoint is drawn
         * when the jet passes the waypoint it becomes inactive
         * so the next one is drawn and shown to the player on the HUD
         * 
         * 
         */
        [SerializeField] private Transform player;


        [SerializeField] List<Transform> waypoints;

        private int waypointIndex;



        public GameObject waypoint;
        [SerializeField] private RectTransform waypointHUD;

        public static WaypointTrackerHUD instance;



        private void Start()
        {
            instance = this;
            waypointIndex = 0;
        }
        private void Awake()
        {
          
            IncreaseIndex();
        }
        private void Update()
        {
             waypoint = waypoints[waypointIndex].gameObject;
            RunWaypointHUD();
          
        }



        private void LateUpdate()
        {
            float cameraRoll = Camera.main.transform.eulerAngles.z;
          
            Vector3 point = (waypoint.transform.position);
            waypointHUD.localEulerAngles = new Vector3(0, 0, -cameraRoll);
       
        }
    
        public void IncreaseIndex()
        {
            waypointIndex = (waypointIndex + 1) % waypoints.Count;

          
        }
       
        void RunWaypointHUD()
        {


            if (GameController.gameController.alive)
            {
                Vector3 point = (waypoint.transform.position);
                float distanceMultiplier = Mathf.InverseLerp(1000, 50, (point - transform.position).magnitude);
                waypointHUD.transform.localScale = waypointHUD.transform.localScale.normalized * Mathf.Max(0.5f, distanceMultiplier) * 2;

                Vector3 HUDPoint = WorldToScreenSpace(point);
                if (HUDPoint.z > 0)
                {
                    HUDPoint.x = Mathf.Clamp(HUDPoint.x, -Screen.width / 4, Screen.width / 3);
                    HUDPoint.y = Mathf.Clamp(HUDPoint.y, -Screen.height / 3, Screen.height / 3);


                    waypointHUD.anchoredPosition = new Vector3(HUDPoint.x, HUDPoint.y);

                }

            }

        }
    
        private Vector3 WorldToScreenSpace(Vector3 point)
        {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(point);

            return screenPoint - new Vector3(Camera.main.pixelWidth/2,Camera.main.pixelHeight/2);
        }
    }
}