using Aircraft;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TextCore.Text;
using TMPro;
namespace GameUI
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] private PlaneController plane;
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
            localVelocity = body.GetLinearVelocity();
            boreSight.anchoredPosition = new Vector2(0,0);
            FlightPathMarker();
            Indicators();
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
           float posY = boresightPosition.y -(body.GetLinearVelocity().y + body.GetAngleOfAttack() ) * PixelsPerDegree();
           float posX = boresightPosition.x - Mathf.Atan2(body.GetLinearVelocity().x, body.GetLinearVelocity().z)* Mathf.Rad2Deg * PixelsPerDegree();

            flightPathMarker.anchoredPosition = new Vector3(posX,posY) ;
            bool enabled = Mathf.Abs(body.transform.InverseTransformDirection(Camera.main.transform.eulerAngles).y) < 25;
            //flightPathMarker.gameObject.SetActive(enabled);
           // boreSight.gameObject.SetActive(enabled);
        }

        float PixelsPerDegree()
        {
            float pixels = Camera.main.scaledPixelHeight + Camera.main.scaledPixelWidth;
            return Mathf.Tan(pixels / 15);
        }

    }
}