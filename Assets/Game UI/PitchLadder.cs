using System.Collections.Generic;
using UnityEngine;
using Aircraft;
namespace GameUI
{
    public class PitchLadder : MonoBehaviour
    {

        private struct Rung
        {
            public RectTransform rect;
            public float pitch;
        }

        private readonly List<Rung> rungs = new List<Rung>();
        private float lastRoll;

        [Header("References")]
        [SerializeField] private AeroBody body;
        [SerializeField] private RectTransform ladderRoot;        // rotates with roll; child of a masked, non-rotating window
        [SerializeField] private RectTransform horizonMarker;     // child of ladderRoot; acts as the 0 degree line
        [SerializeField] private RectTransform pitchLadderMarker; // rung prefab (if it's a scene object, disable it)

        [Header("Layout")]
        [SerializeField] private int stepDegrees = 5;
        [SerializeField] private int maxPitchDegrees = 90;
        [SerializeField] private float pixelsPerDegree = 8f;
        [SerializeField] private float cullHalfHeight = 400f;     // roughly the mask's half-diagonal; the mask does the exact clipping
        [SerializeField] private bool invertRoll = false;

        private List<RectTransform> pitchLadderMarkers = new List<RectTransform>();

  
        private void Start()
        {
            Vector3 forwardDir = Camera.main.transform.position + body.transform.InverseTransformDirection(body.transform.forward);
            Vector3 screenDirectionForward = WorldToHudCoordinates(forwardDir);

            RunPitchLadder();
            BuildRungs();
            SetPitchLadderElements(screenDirectionForward);
           
        }
        private void Update()
        {
            RunPitchLadder();
        }


        void SetPitchLadderElements(Vector3 screenForwardDirection)
        {
     
        }

      

   
        private void RunPitchLadder()
        {

            
            Vector3 forwardDir = Camera.main.transform.position + body.transform.TransformDirection( body.transform.forward);
            Vector3 screenDirectionForward = WorldToHudCoordinates(forwardDir);
       
            horizonMarker.anchoredPosition = screenDirectionForward;
            horizonMarker.eulerAngles = new Vector3(0,0,-body.transform.eulerAngles.z);
          
        }
        private Vector3 WorldToHudCoordinates(Vector3 point)
        {

            float camX = Camera.main.pixelWidth/2;
            float camY = Camera.main.pixelHeight/2;

            Vector3 center = new Vector3(camX, camY);

            return Camera.main.WorldToScreenPoint(point) - center;
        }

   

  

    

        private void LateUpdate()
        {
            UpdatePitchLadder();
            RunPitchLadder();
        }

        private void BuildRungs()
        {
            int step = Mathf.Max(1, stepDegrees);

            for (int deg = -maxPitchDegrees; deg <= maxPitchDegrees; deg += step)
            {
                if (deg == 0) continue; // the horizon marker is the 0 degree line

                RectTransform rect = Instantiate(pitchLadderMarker, ladderRoot, false);
                rect.anchoredPosition = Vector2.zero;
                rungs.Add(new Rung { rect = rect, pitch = deg });
            }
        }

        private void UpdatePitchLadder()
        {
            Vector3 fwd = body.transform.forward;
            float pitch = Mathf.Asin(Mathf.Clamp(fwd.y, -1f, 1f)) * Mathf.Rad2Deg;

            // Roll is undefined when pointing straight up or down, so hold the last value there.
            if (Mathf.Abs(fwd.y) < 0.999f)
            {
                lastRoll = -body.transform.eulerAngles.z;
            }

            ladderRoot.localRotation = Quaternion.Euler(0f, 0f, lastRoll);

            // Horizon line: where 0 degrees sits relative to the nose.
            horizonMarker.anchoredPosition = new Vector2(0f, -pitch * pixelsPerDegree);
            Vector3 forwardDir = Camera.main.transform.position + body.transform.localRotation * body.transform.InverseTransformDirection(body.transform.forward);
            Vector3 screenDirectionForward = WorldToHudCoordinates(forwardDir);
            foreach (Rung rung in rungs)
            {
                float y = (rung.pitch - pitch) * pixelsPerDegree;

                bool visible = Mathf.Abs(y) <= cullHalfHeight;
                if (rung.rect.gameObject.activeSelf != visible)
                    rung.rect.gameObject.SetActive(visible);

                if (visible)
                    rung.rect.anchoredPosition =  new Vector2(screenDirectionForward.x, screenDirectionForward.y - y);
            }
        }
    }
}

