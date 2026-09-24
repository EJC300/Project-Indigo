using UnityEngine;
using UnityEngine.SceneManagement;
namespace GameInfrastructure
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] MainMenu mainMenu;
     

        private void OnEnable()
        {
            mainMenu.OnExitGame.voidEventToFire += ExitGame;
            mainMenu.OnEnterInstantAction.voidEventToFire += LoadInstantAction;
     
        }

        private void OnDisable()
        {
            mainMenu.OnExitGame.voidEventToFire -= ExitGame;
            mainMenu.OnEnterInstantAction.voidEventToFire -= LoadInstantAction;
         
        }
    

        void LoadInstantAction()
        {
            int instantActionEnter = 1;
            SceneManager.LoadScene(instantActionEnter, LoadSceneMode.Single);
        }
        
        void ExitGame()
        {
            Application.Quit();
        }
        

    
    }
}
