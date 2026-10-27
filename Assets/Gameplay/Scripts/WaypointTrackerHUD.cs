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

        public GameObject testWaypoint;
        [SerializeField] private RectTransform waypointHUD;
        private Quaternion originalWaypointRotation;
        private void Start()
        {
            originalWaypointRotation = testWaypoint.transform.localRotation;
        }
        private void Update()
        {
           
            RunWaypointHUD();

        }
        private void LateUpdate()
        {
            float cameraRoll = Camera.main.transform.eulerAngles.z;
          
            Vector3 point = (testWaypoint.transform.position);
            waypointHUD.localEulerAngles = new Vector3(0, 0, -cameraRoll);
       
        }
        void RunWaypointHUD()
        {
            Vector3 point = ( testWaypoint.transform.position);
            float distanceMultiplier = Mathf.InverseLerp(1000,50,(point - transform.position).magnitude);
            waypointHUD.transform.localScale = waypointHUD.transform.localScale.normalized * Mathf.Max(0.5f, distanceMultiplier) * 2;
    
            Vector3 HUDPoint = WorldToScreenSpace(point);
            if (HUDPoint.z > 0)
            {
                HUDPoint.x = Mathf.Clamp(HUDPoint.x, -Screen.width / 4, Screen.width / 3);
                HUDPoint.y = Mathf.Clamp(HUDPoint.y, -Screen.height / 3, Screen.height / 3);
        

                waypointHUD.anchoredPosition = new Vector3(HUDPoint.x, HUDPoint.y);

            }













        }
        void ClampWaypointMarkerToHUD()
        {  
            Vector3 previousPoint = Vector3.zero;
            Vector3 point = waypointHUD.anchoredPosition;
            Vector3 cameraCenter = WorldToScreenSpace(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2));
            Vector3 direction = (waypointHUD.anchoredPosition - new Vector2( cameraCenter.x,cameraCenter.y));
          
        }
        private Vector3 WorldToScreenSpace(Vector3 point)
        {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(point);

            return screenPoint - new Vector3(Camera.main.pixelWidth/2,Camera.main.pixelHeight/2);
        }



     


    }
}