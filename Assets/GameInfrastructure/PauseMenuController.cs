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
        private int currentLevelIndex;
        private void Start()
        {
            currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        }
        private void OnEnable()
        {
            pauseModalWindow.PauseModalWindowWindowReturnToMainMenuEvent.voidEventToFire += LoadMainMenu;
            pauseModalWindow.PauseModalWindowWindowCloseEvent.voidEventToFire += ClosePauseWindow;
            pauseModalWindow.PauseModalWindowWindowOpenEvent.voidEventToFire += OpenPauseWindow;
            pauseModalWindow.PauseModalWindowRestartLevelEvent.voidEventToFire += RestartLevel;

        }

        private void OnDisable()
        {
            pauseModalWindow.PauseModalWindowWindowReturnToMainMenuEvent.voidEventToFire -= LoadMainMenu;
            pauseModalWindow.PauseModalWindowWindowCloseEvent.voidEventToFire -= ClosePauseWindow;
            pauseModalWindow.PauseModalWindowWindowOpenEvent.voidEventToFire -= OpenPauseWindow;
            pauseModalWindow.PauseModalWindowRestartLevelEvent.voidEventToFire -= RestartLevel;
        }

        void OpenPauseWindow()
        {

                gameFlow.SetPaused(true);
                pauseWindow.SetActive(!pauseWindow.activeInHierarchy);
                 return; 
        }

        void ClosePauseWindow() 
        {
          
            if (pauseWindow.activeInHierarchy)
            {
                gameFlow.SetPaused(false);
                pauseWindow.SetActive(false);

            }
         }

        void RestartLevel()
        {
            SceneManager.LoadScene(currentLevelIndex, LoadSceneMode.Single);
        }
        void LoadMainMenu()
        {
            int mainMenu = 0;
            SceneManager.LoadScene(mainMenu, LoadSceneMode.Single);
        }

        

    }
}
