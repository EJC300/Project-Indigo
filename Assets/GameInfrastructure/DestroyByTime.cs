using UnityEngine;
namespace GameInfrastructure
{
    public class DestroyByTime : MonoBehaviour
    {
        [SerializeField] private float timer;
        private void Update()
        {
            Destroy(gameObject, timer);
        }


    }
}
