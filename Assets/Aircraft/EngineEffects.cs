using UnityEngine;
using GameInfrastructure;
namespace Aircraft
{
    public class EngineEffects : MonoBehaviour
    {

        private AeroBody body;
        private PlaneController planeController;
        [SerializeField] Transform afterBurnerMeshScaler;


        void AfterBurnerEffect()
        {
            float milThrustAmount = planeController.MilThrustAmount();
            float afterburnerThrust = planeController.FullAfterBurnerAmount();
            float throttleAfterBurner = Mathf.InverseLerp(milThrustAmount, afterburnerThrust, planeController.GetThrust());
            Debug.Log(throttleAfterBurner);
            float afterBurnerScale = throttleAfterBurner;
            afterBurnerMeshScaler.transform.localScale = new Vector3(afterBurnerMeshScaler.transform.localScale.x, afterBurnerMeshScaler.transform.localScale.y, afterBurnerScale);
        }
        private void OnEnable()
        {
        }
        private void Update()
        {
            AfterBurnerEffect();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            body = GetComponentInParent<AeroBody>();
            planeController = GetComponentInParent<PlaneController>();

          
        }

    
    }
}