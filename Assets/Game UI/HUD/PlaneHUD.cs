using UnityEngine;
using Aircraft;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
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

        [SerializeField] private GameObject degreeText;

        [SerializeField] private RectTransform pitchLadderRoot;

        [SerializeField] private List<PitchRung> pitchRungs = new List<PitchRung>();
        private float pitch;

        private float lastRoll;

        public struct PitchRung
        {
            public RectTransform rectTransform;

            public float pitch;
            // Pitch Degree Text left
            public TextMeshProUGUI textLeft;
            // Pitch Degree Text Right
            public TextMeshProUGUI textRight;
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
                RectTransform rect = Instantiate(pitchLadderMarker, transform, false);
                
                rect.anchoredPosition = Vector3.zero;
                PitchRung rung = new PitchRung();
                GameObject degreeTextLeft = Instantiate(degreeText,rect,false);
                GameObject degreeTextRight = Instantiate(degreeText, rect, false);
        
                degreeTextRight.GetComponent<RectTransform>().anchoredPosition = rect.anchoredPosition + Vector2.right * 275;
                degreeTextLeft.GetComponent<RectTransform>().anchoredPosition = rect.anchoredPosition + Vector2.left * 129;

                TextMeshProUGUI degreeLeft = degreeTextLeft.GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI degreeRight = degreeTextRight.GetComponent <TextMeshProUGUI>();
                degreeLeft.text = deg.ToString();
                degreeRight.text = deg.ToString();

                rung.pitch = deg;
                rung.textLeft = degreeLeft;
                rung.textRight = degreeRight;
                rung.rectTransform = rect;

                rung.rectTransform.SetParent(pitchLadderRoot);
                pitchRungs.Add(rung);
            }
          
           
        }

        void PitchLadder()
        {
            
            Camera camera = Camera.main;

            Vector3 fwd = body.transform.forward;

            if (Mathf.Abs(fwd.y) < 0.999f)
            {
                lastRoll = -body.transform.eulerAngles.z;
            }

            pitchLadderRoot.localRotation = Quaternion.Euler(0f, 0f, lastRoll);


            pitch = Mathf.Asin(Mathf.Clamp(fwd.y, -1f, 1f)) * Mathf.Rad2Deg;
          
           
            Vector3 localDirection = body.transform.InverseTransformDirection(body.transform.forward);

            Vector3 relativeDirection = camera.transform.position + body.transform.rotation * localDirection;

            Vector3 hudSpacePosition =  Utilities.MathHelpers.TransformToHUDSpace( relativeDirection, camera);
            pitchLadderRoot.anchoredPosition =  hudSpacePosition;
            pitchLadderRoot.localRotation = Quaternion.Euler(0,0, -camera.transform.eulerAngles.z);
            foreach (PitchRung pitchRung in pitchRungs)
            {
         

                float y = (pitchRung.pitch - pitch) * pixelsPerDegrees;
            
           
                bool visible = Mathf.Abs(y) <= halfHeight && hudSpacePosition.z >= 0;
                if (pitchRung.rectTransform.gameObject.activeSelf != visible)
                    pitchRung.rectTransform.gameObject.SetActive(visible);


                if (visible)

                {
                

                    pitchRung.rectTransform.anchoredPosition = new Vector2(0, y);
                }
             
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