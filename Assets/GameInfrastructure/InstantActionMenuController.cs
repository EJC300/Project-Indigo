using UnityEngine;
using UnityEngine.SceneManagement;
namespace GameInfrastructure
{
    public class InstantActionMenuController : MonoBehaviour
    {
        [SerializeField] InstantActionMenu instantActionMenu;


        private void OnEnable()
        {

            instantActionMenu.EnterMainEvent.voidEventToFire += LoadMainMenu;
            instantActionMenu.EnterBattleEvent.voidEventToFire += LoadGame;
        }

        private void OnDisable()
        {

            instantActionMenu.EnterMainEvent.voidEventToFire -= LoadMainMenu;
            instantActionMenu.EnterBattleEvent.voidEventToFire -= LoadGame;
        }
        void LoadGame()
        {
            int instantAction = 2;
            SceneManager.LoadScene(instantAction, LoadSceneMode.Single);
        }
        void LoadMainMenu()
        {
            int mainMenu = 0;
            SceneManager.LoadScene(mainMenu, LoadSceneMode.Single);
        }
    }
}