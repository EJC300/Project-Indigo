using UnityEngine;
namespace Aircraft
{
    public class PlaneHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth;
        [SerializeField] private GameObject crashSmoke;
        [SerializeField] private AeroBody body;

        private void Update()
        {
            
        }
        public void ApplyDamage()
        {
            //Damage for weapons
        }
        private void OnCollisionEnter(Collision collision)
        {
            CrashDamage();
            Debug.Log("Crash");
        }
        public void CrashDamage()
        {
            float directionOfImpact = Vector3.Dot(body.GetLinearVelocity(), transform.forward) * 3.6f;
            if (directionOfImpact > 400 )
            {

             
               Instantiate(crashSmoke,transform.position,Quaternion.identity);
               Destroy(this.gameObject);
            }
        }
    }

}