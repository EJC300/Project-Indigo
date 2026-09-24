using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace GameInfrastructure
{
    public class FadeCamera : MonoBehaviour
    {
        //I am bad at UI still
       public LevelLoader levelLoader;
      
        [SerializeField] private float fadeInTime;
        [SerializeField] private float fadeOutTime;
        [SerializeField] private Image fader;
         private bool fadeOut;
         private bool fadeIn;
        
        private Color fadeColor;

        private void OnEnable()
        {

                levelLoader = LevelLoader.instance;
                levelLoader.onFadeOut.voidEventToFire += FadeOut;
                levelLoader.onFadeIn.voidEventToFire += FadeIn;
            
        }
        private void OnDisable()
        {
                levelLoader = LevelLoader.instance;
                levelLoader.onFadeOut.voidEventToFire -= FadeOut;
                levelLoader.onFadeIn.voidEventToFire -= FadeIn;
            
        }
        private void Awake()
        {
            levelLoader = LevelLoader.instance;
        }
        private void Start()
        {
            levelLoader = LevelLoader.instance;
            fadeColor = fader.color;
        }
        private void Update()
        {
          fader.color = fadeColor;
        }
        
        private IEnumerator ToggleFadeOut()
        {
            fadeColor.a = Mathf.MoveTowards(fadeColor.a, 0, fadeOutTime);
            yield return new WaitForSeconds(fadeOutTime);
            fadeOut = true;
        }
        private IEnumerator ToggleFadeIn()
        {
            fadeColor.a = Mathf.MoveTowards(fadeColor.a, 1, fadeOutTime);
            yield return new WaitForSeconds(fadeOutTime);
            fadeIn = true;
        }

        void FadeIn()
        {
            if (!fadeIn)
            {
                StartCoroutine(ToggleFadeIn());
            }
        }
        void FadeOut()
        {
            if (!fadeOut)
            {
                StartCoroutine(ToggleFadeOut());
            }
        }
    }
}
