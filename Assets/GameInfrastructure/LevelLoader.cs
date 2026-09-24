using UnityEngine;
using UnityEngine.SceneManagement;
namespace GameInfrastructure
{
    public class LevelLoader : MonoBehaviour
    {
        public static LevelLoader instance;
        //Register with menu camera
        public VoidGameEvent onFadeIn;
        public VoidGameEvent onFadeOut;
        private Scene currentScene;
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(instance.gameObject);
            }
            currentScene = SceneManager.GetActiveScene();
        }
   


           
        

        public void LoadLevel(int level)
        {
            Fade();
            SceneManager.UnloadSceneAsync(currentScene);
            
            SceneManager.LoadSceneAsync(level);
            FadeOut();
                     
            
         
        }

        void FadeOut()
        {
            onFadeOut?.FireEvent();
        }
        void Fade()
        {
            onFadeIn?.FireEvent();
        }
    }
}
