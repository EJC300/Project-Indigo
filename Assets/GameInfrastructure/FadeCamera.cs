using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace GameInfrastructure
{
    public class FadeCamera : MonoBehaviour
    {
      

        [SerializeField] private float fadeOutTime;
        [SerializeField] private Image fader;
    
        
        private Color fadeColor;

   
        private void Update()
        {
        
        }
        private void Start()
        {
            StartCoroutine(ToggleFadeOut());
         
        }
        private IEnumerator ToggleFadeOut()
        {
            float elapsedTime = 0.0f;

            while (elapsedTime < fadeOutTime)
            {
                elapsedTime += Time.deltaTime;
                fadeColor.a = (1f - (elapsedTime / fadeOutTime));
                fader.color = fadeColor;
                yield return null;
            }



        }

      
    }
}
