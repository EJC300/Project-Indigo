using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Color = UnityEngine.Color;

namespace Gameplay
{
    public class TargetTrackerDemo : MonoBehaviour
    {
        [SerializeField] private Transform player;

        [SerializeField] private Transform demoTarget;
        //Keep the same as the bomb
        [SerializeField] private float navConstant = 5.0f;

        [SerializeField, Range(0f, 0.5f)] private float horizontalLimit = 0.33f;
        [SerializeField, Range(0f, 0.5f)] private float verticalLimit = 0.33f;



        [SerializeField] private RectTransform targetBox;

        [SerializeField] private RectTransform targetCross;

        [SerializeField] private RectTransform targetHUD;
        [SerializeField] private TextMeshProUGUI shootText;

        [SerializeField] private Image targetImage;

        private Vector3 targetPosition;
        private Vector3 previousTargetPosition;
        private Vector3 targetVelocity;
        private Vector3 previousTargetVelocity;
        private Vector3 targetAcceleration;

        public Vector3 targetPoint;

        public static WaypointTrackerHUD instance;

        private Rigidbody rb;


        private void Start()
        {

            rb = player.GetComponent<Rigidbody>();
        }
        private void Awake()
        {


        }
        private void FixedUpdate()
        {
            targetPosition = demoTarget.position;
            TargetAcceleration();
        }
        private void Update()
        {

            RunTarget();

        }
        private void TargetAcceleration()
        {
            float dt = Time.fixedDeltaTime;
            if (dt <= 0f) return;


            targetVelocity = (targetPosition - previousTargetPosition) / dt;
            targetAcceleration = (targetVelocity - previousTargetVelocity) / dt;

            previousTargetVelocity = targetVelocity;
            previousTargetPosition = targetPosition;
        }



        private Vector3 AccelerationCommand()
        {
            Vector3 relativePosition = targetPosition - transform.position;
            Vector3 relVel = targetVelocity - rb.linearVelocity;


            float closingSpeed = -Vector3.Dot(relVel, relativePosition.normalized);
            float timeUntilImpact = closingSpeed > 0.01f ? relativePosition.magnitude / closingSpeed : 1f;
            timeUntilImpact = Mathf.Clamp(timeUntilImpact, 0.05f, 5.0f); // Safety clamps

            Vector3 predictedTargetPosition = targetPosition
                + (targetVelocity * timeUntilImpact)
                + (0.5f * targetAcceleration * (timeUntilImpact * timeUntilImpact));


            Vector3 predictedRelativePos = predictedTargetPosition - transform.position;
            Vector3 zeroEffortMiss = predictedRelativePos + relVel * timeUntilImpact;

            Vector3 unitLOS = predictedRelativePos.normalized;
            Vector3 zeroEffortMissLOS = Vector3.Project(zeroEffortMiss, unitLOS);
            Vector3 zeroEffortNormal = zeroEffortMiss - zeroEffortMissLOS;


            float bias = AccelerationBias();
            float denominator = (timeUntilImpact * timeUntilImpact + bias);
            if (Mathf.Abs(denominator) < 0.0001f) denominator = 0.0001f;

            Vector3 acceleration = navConstant * zeroEffortNormal / denominator;
            return acceleration;
        }

        private float AccelerationBias()
        {
            float n = navConstant / 2f;
            float acceleration = targetAcceleration.magnitude;
            float speed = targetVelocity.magnitude;

            if (speed > 0.001f)
            {
                float delta = acceleration / speed;
                return n * delta;
            }
            else
            {
                return acceleration * n;
            }
        }


        private void LateUpdate()
        {
            float cameraRoll = Camera.main.transform.eulerAngles.z;

            Vector3 point = (demoTarget.transform.position);
            targetBox.localEulerAngles = new Vector3(0, 0, -cameraRoll);
            targetCross.localEulerAngles = new Vector3(0, 0, -cameraRoll);
        }

        void TargetCross()
        {
            if (GameController.gameController.alive)
            {

                Vector3 point = targetPosition + (rb.linearVelocity + AccelerationCommand());
                Debug.Log(Vector3.Distance(targetPosition, rb.linearVelocity + AccelerationCommand()));
                if (Vector3.Distance(targetPosition, rb.linearVelocity + AccelerationCommand()) < 4000f)
                {
                    targetImage.color = Color.red;
                    shootText.enabled = true;
                }
                else
                {
                    shootText.enabled = false;
                    targetImage.color = Color.green;
                }
              
                float distanceMultiplier = Mathf.InverseLerp(500, 50, (point - transform.position).magnitude);
                targetCross.transform.localScale = Vector3.one * Mathf.Max(0.5f, distanceMultiplier) * 2;

                Vector3 HUDPoint = MathHelpers.TransformToHUDSpace(point, Camera.main);




                float halfW = Screen.width * horizontalLimit;
                float halfH = Screen.height * verticalLimit;

                if (HUDPoint.z > 0f)
                {

                    HUDPoint.x = Mathf.Clamp(HUDPoint.x, -halfW, halfW);
                    HUDPoint.y = Mathf.Clamp(HUDPoint.y, -halfH, halfH);
                }
                else
                {

                    HUDPoint = -HUDPoint;
                    if (HUDPoint.sqrMagnitude < 1f) HUDPoint = Vector2.down;

                    float kx = halfW / Mathf.Max(Mathf.Abs(HUDPoint.x), 0.0001f);
                    float ky = halfH / Mathf.Max(Mathf.Abs(HUDPoint.y), 0.0001f);
                    HUDPoint *= Mathf.Min(kx, ky);


                }
                targetPoint = point;
                targetCross.anchoredPosition = HUDPoint;

            }

        }
    
        void TargetBox()
        {
            if (GameController.gameController.alive)
            {
                Vector3 point = (demoTarget.transform.position);
                float distanceMultiplier = Mathf.InverseLerp(500, 50, (point - transform.position).magnitude);
                targetHUD.transform.localScale = Vector3.one * Mathf.Max(0.5f, distanceMultiplier) * 2;

                Vector3 HUDPoint = MathHelpers.TransformToHUDSpace(point, Camera.main);




                float halfW = Screen.width * horizontalLimit;
                float halfH = Screen.height * verticalLimit;

                if (HUDPoint.z > 0f)
                {

                    HUDPoint.x = Mathf.Clamp(HUDPoint.x, -halfW, halfW);
                    HUDPoint.y = Mathf.Clamp(HUDPoint.y, -halfH, halfH);
                }
                else
                {

                    HUDPoint = -HUDPoint;
                    if (HUDPoint.sqrMagnitude < 1f) HUDPoint = Vector2.down;

                    float kx = halfW / Mathf.Max(Mathf.Abs(HUDPoint.x), 0.0001f);
                    float ky = halfH / Mathf.Max(Mathf.Abs(HUDPoint.y), 0.0001f);
                    HUDPoint *= Mathf.Min(kx, ky);


                }
                targetBox.anchoredPosition = HUDPoint;

            }


        }
        
        void RunTarget()
        {


            TargetBox();
            TargetCross();

        }
    }
}

