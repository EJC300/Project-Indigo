using UnityEngine;
using UnityEngine.SceneManagement;
namespace GameInfrastructure
{
    public class PauseMenuController : MonoBehaviour
    {
        //Time Flow Controller
        [SerializeField] PauseModalWindow pauseModalWindow;
        [SerializeField] GameObject pauseWindow;
        [SerializeField] GameFlow gameFlow;
        private void Start()
        {
            
        }
        private void OnEnable()
        {
            pauseModalWindow.PauseModalWindowWindowReturnToMainMenuEvent.voidEventToFire += LoadMainMenu;
            pauseModalWindow.PauseModalWindowWindowCloseEvent.voidEventToFire += ClosePauseWindow;
            pauseModalWindow.PauseModalWindowWindowOpenEvent.voidEventToFire += OpenPauseWindow;

        }

        private void OnDisable()
        {
            pauseModalWindow.PauseModalWindowWindowReturnToMainMenuEvent.voidEventToFire -= LoadMainMenu;
            pauseModalWindow.PauseModalWindowWindowCloseEvent.voidEventToFire -= ClosePauseWindow;
            pauseModalWindow.PauseModalWindowWindowOpenEvent.voidEventToFire -= OpenPauseWindow;
        }

        void OpenPauseWindow()
        {

                gameFlow.SetPaused(true);
                pauseWindow.SetActive(!pauseWindow.activeInHierarchy);
          
        }

        void ClosePauseWindow() 
        {
          
            if (pauseWindow.activeInHierarchy)
            {
                gameFlow.SetPaused(false);
                pauseWindow.SetActive(false);

            }
         }
        void LoadMainMenu()
        {
            int mainMenu = 0;
            SceneManager.LoadScene(mainMenu, LoadSceneMode.Single);
        }

        

    }
}
