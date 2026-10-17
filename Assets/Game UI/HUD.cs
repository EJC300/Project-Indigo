using Aircraft;
using UnityEngine;
namespace GameUI
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] private AeroBody body;
        [SerializeField] private Transform flightPathMarker;
        [SerializeField] private Transform boreSight;
        
        public Vector3 CameraToScreenPoint(Vector3 pos)
        {
            Vector3 centerPos = new Vector3(Camera.main.scaledPixelWidth / 2, Camera.main.scaledPixelHeight / 2, 0);
            return  Camera.main.WorldToScreenPoint(pos - centerPos );
        }
        void CheckVisibility()
        {
            bool visibility = Mathf.Abs( Camera.main.transform.eulerAngles.y) < 5 || Mathf.Abs(Camera.main.transform.eulerAngles.x) < 5;
            flightPathMarker.gameObject.SetActive(visibility);
        }
        Quaternion KeepRotation(Vector3 screenForwardPos,Vector3 rectPos)
        {
            Vector2 screenDirection = (Vector2)(screenForwardPos - rectPos);

            // Calculate the angle in degrees
            float angle = Vector3.Angle(screenForwardPos, rectPos);
           
            return (Quaternion.Euler(angle,angle,0));
        }
        void Boresight()
        { 
        
          Vector3 targetForward   = Camera.main.WorldToScreenPoint( body.transform.position + body.transform.forward);
            boreSight.rotation = KeepRotation(targetForward,boreSight.position);
        
        }
        void FlightPathMarker()
        {
            Vector3 velocityPoint = (  body.transform.position + (body.GetLinearVelocity().normalized * 5));
            
            Vector3 fpmPos = Camera.main.WorldToScreenPoint(velocityPoint);
            
            flightPathMarker.rotation = KeepRotation(fpmPos,flightPathMarker.position);
            flightPathMarker.position = fpmPos;
        }
        private void FixedUpdate()
        {
            //CheckVisibility();
            FlightPathMarker();
            Boresight();
         
        }
    }
}