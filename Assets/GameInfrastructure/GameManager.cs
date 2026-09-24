using UnityEngine;
namespace GameInfrastructure
{
    public class GameManager : MonoBehaviour
    {

        //Level Loader ---- Loads the level configured scene

        public static GameManager instance;

        public VoidGameEvent exitGame;

        public VoidGameEvent goToMainMenu;

        public VoidGameEvent goToInstantAction;

        public VoidGameEvent LoadMainScene;

        public BoolGameEvent pauseGameToggle;


        public void Awake()
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
        }

        public void OnEnable()
        {
            exitGame.voidEventToFire += QuiteGame;
            goToInstantAction.voidEventToFire += SelectInstantAction;
        }

        public void OnDisable()
        {
            exitGame.voidEventToFire -= QuiteGame;
            goToInstantAction.voidEventToFire -= SelectInstantAction;
        }
        void SelectInstantAction()
        {
            Debug.Log("SelectInstantAction");
        }
        void SelectMainMenu()
        {
            Debug.Log("SelectMainMenu");
        }
        void QuiteGame()
        {
           Debug.Log("Quite Game");
          // Application.Quit();
        }


    }
}
