using UnityEngine;
using Aircraft;
using System.Collections.Generic;
namespace GameUI
{

    

    public class PlaneHUD : MonoBehaviour
    {

        [SerializeField] private int maxDegrees;

        [SerializeField] private int degreesPerStep;

        [SerializeField] private float pixelsPerDegrees;

        [SerializeField] private float halfHeight;

        [SerializeField] private AeroBody body;
        [SerializeField] private RectTransform flightPathMarker;
        [SerializeField] private RectTransform boreSightMarker;

        [SerializeField] private RectTransform pitchLadderMarker;

        [SerializeField] private RectTransform pitchLadderHorizonMarker;

        [SerializeField] private List<PitchRung> pitchRungs = new List<PitchRung>();

        public struct PitchRung
        {
            public RectTransform rectTransform;

            public float pitch;
            // Pitch Degree Text left

            // Pitch Degree Text Right
        }


   
         
        


        void Start()
        {
            SetupHUD();

        }

        private void Update()
        {
            RunHUD();
        }
        void RunHUD()
        {
            PitchLadderHorizonMarker();
            PitchLadder();
            FlightPathMarker();
            BoreSightMarker();
        }


        void SetupHUD()
        {

            SetupPitchLadder();
        }

        void SetupPitchLadder()
        {
            Camera camera = Camera.main;

            int step = degreesPerStep;

            for (int deg = -maxDegrees; deg < maxDegrees; deg += step)
            {
                RectTransform rect = Instantiate(pitchLadderMarker, pitchLadderHorizonMarker, false);
                
                rect.anchoredPosition = Vector3.zero;
                PitchRung rung = new PitchRung();

                rung.pitch = deg;
              

                rung.rectTransform = rect;

                pitchRungs.Add(rung);
            }
          
           
        }

        void PitchLadder()
        {
            
            Camera camera = Camera.main;

            Vector3 fwd = body.transform.forward;
            float pitch = Mathf.Asin(Mathf.Clamp(fwd.y, -1f, 1f)) * Mathf.Rad2Deg;

            Debug.Log(pitch);
            Vector3 localDirection = body.transform.InverseTransformDirection(body.transform.forward);

            Vector3 relativeDirection = camera.transform.position + Quaternion.Euler(0, body.transform.eulerAngles.y, 0) * localDirection;

            Vector3 hudSpacePosition = Utilities.MathHelpers.TransformToHUDSpace(relativeDirection, camera);
            foreach (PitchRung pitchRung in pitchRungs)
            {
                float y = (pitchRung.pitch - pitch) * pixelsPerDegrees;

                bool visible = Mathf.Abs(y) <= halfHeight;
                if (pitchRung.rectTransform.gameObject.activeSelf != visible)
                    pitchRung.rectTransform.gameObject.SetActive(visible);

                if (visible)
                    pitchRung.rectTransform.anchoredPosition = Quaternion.Euler(body.transform.eulerAngles.x,0,0) * new Vector2(0, hudSpacePosition.y - y);
            }
        }
        void PitchLadderHorizonMarker()
        {
            Camera camera = Camera.main;
            Vector3 localDirection = body.transform.InverseTransformDirection(body.transform.forward);

            Vector3 relativeDirection = camera.transform.position + Quaternion.Euler(0, body.transform.eulerAngles.y, 0) * localDirection;

            Vector3 hudSpacePosition = Utilities.MathHelpers.TransformToHUDSpace(relativeDirection, camera);
    
            if (hudSpacePosition.z > 0)
            {
                pitchLadderHorizonMarker.gameObject.SetActive(true);
                pitchLadderHorizonMarker.anchoredPosition = hudSpacePosition;
            }
            else
            {
                pitchLadderHorizonMarker.gameObject.SetActive(false);
            }
            pitchLadderHorizonMarker.eulerAngles = new Vector3(0, 0, -camera.transform.eulerAngles.z);
        }
        void BoreSightMarker()
        {
            Camera camera = Camera.main;

            Quaternion rotation = body.transform.rotation;

            Vector3 localDirection = body.transform.InverseTransformDirection(body.transform.forward);

            Vector3 relativeDirection = camera.transform.position + rotation * localDirection;

            Vector3 hudSpaceBoreSight = Utilities.MathHelpers.TransformToHUDSpace(relativeDirection,camera);

            if (hudSpaceBoreSight.z > 0)
            {
                boreSightMarker.gameObject.SetActive(true);
                boreSightMarker.anchoredPosition = -hudSpaceBoreSight;
            }
            else
            {
                boreSightMarker.gameObject.SetActive(false);
            }

         
        }
        void FlightPathMarker()
        {
            Camera camera = Camera.main;

            Quaternion rotation = body.transform.rotation;

            Vector3 localVelocity =( body.GetLinearVelocity());

            Vector3 relativeVelocity = camera.transform.position + rotation * localVelocity;

            Vector3 hudSpaceFlightPath = Utilities.MathHelpers.TransformToHUDSpace(relativeVelocity,camera);

            flightPathMarker.anchoredPosition = hudSpaceFlightPath;
            if (hudSpaceFlightPath.z > 0)
            {
                flightPathMarker.gameObject.SetActive(true);
                flightPathMarker.anchoredPosition = hudSpaceFlightPath;
            }
            else
            {
                flightPathMarker.gameObject.SetActive(false);
            }

          
        }

     

     
       

   

         






    }

}