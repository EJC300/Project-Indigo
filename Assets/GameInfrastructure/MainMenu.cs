using UnityEngine;
namespace GameInfrastructure
{
    public class MainMenu : MonoBehaviour
    {
        public GameManager gameManager;

        private void Awake()
        {
            gameManager = GameManager.instance;
        }

        public void ExitGame()
        {
            gameManager.exitGame?.FireEvent();
        }

        public void EnterInstantActionMode()
        {
            gameManager.goToInstantAction?.FireEvent();
        }
    }
}