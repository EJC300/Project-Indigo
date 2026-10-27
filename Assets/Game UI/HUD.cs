using Aircraft;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
namespace GameUI
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] private PlaneController plane ;
        [SerializeField] private AeroBody body;
        [SerializeField] private RectTransform flightPathMarker;
        [SerializeField] private RectTransform boreSight;

        [SerializeField] private TMP_Text speedIndicator;
        [SerializeField] private TMP_Text aoaIndicator;
        [SerializeField] private TMP_Text gForceIndicator;

        private Vector3 localVelocity;
        private Vector3 boresightPosition;



        private void FixedUpdate()
        {
            if (plane != null && body != null)
            {
                localVelocity = body.GetLinearVelocity();

                BoreSightMarker();
                FlightPathMarker();
                Indicators();
            }
        }

        void Indicators()
        {
            speedIndicator.text = $" SPD   {Mathf.RoundToInt(body.GetLinearVelocity().z * 3.6f ).ToString()  }/Kmh";
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
        void FlightPathMarker()
        {

            Vector3 point = ObjectToCameraSpace(Camera.main.transform.position +  body.transform.rotation *body.GetLinearVelocity());
            flightPathMarker.anchoredPosition = point;
            if (point.z > 0)
            {
                flightPathMarker. gameObject.SetActive(true);
                flightPathMarker.anchoredPosition = new Vector3(point.x, point.y, 0);

            }
            else
            {
                flightPathMarker.gameObject.SetActive(false);
            }

        }

        void BoreSightMarker()
        {
            Vector3 point = ObjectToCameraSpace(Camera.main.transform.position + body.transform.rotation * body.transform.InverseTransformDirection( body.transform.forward));
            boreSight.anchoredPosition = point;
            if (point.z > 0)
            {
                boreSight.gameObject.SetActive(true);
                boreSight.anchoredPosition = new Vector3(point.x, point.y, 0);

            }
            else
            {
                boreSight.gameObject.SetActive(false);
            }
        }

       Vector3 ObjectToCameraSpace(Vector3 pos)
        {
            Vector3 cameraSpace = Camera.main.WorldToScreenPoint(pos);

            return cameraSpace - new Vector3(Camera.main.scaledPixelWidth / 2, Camera.main.scaledPixelHeight / 2);
        }

    }
}