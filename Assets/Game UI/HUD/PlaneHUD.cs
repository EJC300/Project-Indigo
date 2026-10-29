using UnityEngine;
using Aircraft;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Gameplay;
namespace GameUI
{

    

    public class PlaneHUD : MonoBehaviour
    {
        [SerializeField] private int maxDegreesCompassLadder;

        [SerializeField] private int degreesCompassPixelsPerStep;

        [SerializeField] private float pixelsPerDegreesCompass;

        [SerializeField] private float halfHeightCompass;
        
        
        [SerializeField] private PlaneController plane;


        [SerializeField] private TMP_Text speedIndicator;
        [SerializeField] private TMP_Text aoaIndicator;
        [SerializeField] private TMP_Text gForceIndicator;

        [SerializeField] private int maxDegreesPitchLadder;

        [SerializeField] private int degreesPitchPixelsPerStep;

        [SerializeField] private float pixelsPerDegreesPitch;

        [SerializeField] private float halfHeightPitch;

        [SerializeField] private AeroBody body;
        [SerializeField] private RectTransform flightPathMarker;
        [SerializeField] private RectTransform boreSightMarker;

        [SerializeField] private RectTransform pitchLadderMarker;

        [SerializeField] private RectTransform compassMarker;

        [SerializeField] private RectTransform pitchLadderHorizonMarker;

        [SerializeField] private GameObject degreeText;

        [SerializeField] private RectTransform pitchLadderRoot;


        [SerializeField] private RectTransform compassRoot;

        [SerializeField] private List<PitchRung> pitchRungs = new List<PitchRung>();

        [SerializeField] private List<CompassPoint> compassPoints = new List<CompassPoint>();


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

        public struct CompassPoint
        {
            public RectTransform pickleMark;

            public RectTransform belowMark;

            public RectTransform aboveMark;

            public TextMeshProUGUI degreeText;

            public string text;

            public float degree;
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
            if (GameController.gameController.alive)
            {
                PitchLadderHorizonMarker();
                PitchLadder();
                FlightPathMarker();
                BoreSightMarker();
                Indicators();
            }
        }


        void SetupHUD()
        {

            SetupPitchLadder();
            SetupCompass();
        }
        void SetupCompass()
        {
        }

        void Indicators()
        {
            speedIndicator.text = $" SPD   {Mathf.RoundToInt(body.GetLinearVelocity().z * 3.6f).ToString()}/Kmh";
            aoaIndicator.text = $" AOA   {Mathf.Round(body.GetAngleOfAttack())}";
            gForceIndicator.text = $"G   ~{Mathf.Round(plane.GetGforce() * 1000) / 1000}";
            float redWarning = Mathf.InverseLerp(0, plane.GetMaxAOA, body.GetAngleOfAttack() * Mathf.Rad2Deg);
            Color aoaColor = aoaIndicator.color;
            Color green = Color.green;
            if (body.GetAngleOfAttack() > 0.0f)
            {
                aoaColor.r = Mathf.Lerp(aoaColor.r, 1, redWarning);
            }
            else
            {
                aoaColor = green;
            }
        }

        void SetupPitchLadder()
        {
            Camera camera = Camera.main;

            int step = degreesPitchPixelsPerStep;

            for (int deg = -maxDegreesPitchLadder; deg < maxDegreesPitchLadder; deg += step)
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

        void Compass()
        {
         
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
         

                float y = (pitchRung.pitch - pitch) * pixelsPerDegreesPitch;
            
           
                bool visible = Mathf.Abs(y) <= halfHeightPitch && hudSpacePosition.z >= 0;
                if (pitchRung.rectTransform.gameObject.activeSelf != visible)
                {
                    pitchRung.rectTransform.gameObject.SetActive(visible);

                }
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