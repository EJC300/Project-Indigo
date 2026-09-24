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

        }

        private void OnDisable()
        {

            instantActionMenu.EnterMainEvent.voidEventToFire -= LoadMainMenu;

        }


        void LoadMainMenu()
        {
            int mainMenu = 0;
            SceneManager.LoadScene(mainMenu, LoadSceneMode.Single);
        }
    }
}