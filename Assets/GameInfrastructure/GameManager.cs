using UnityEngine;
namespace GameInfrastructure
{
    public class GameManager : MonoBehaviour
    {
        //Combine Managers with Events - Events are registered within managers but fired in other classes that have registered the managers
        //May need to refactor into call back functions however no idea how to do that? That will forgo the singleton abuse
        //Level Loader ---- Loads the level configured scene
       private LevelLoader LevelLoader { get; set; }
       public static GameManager instance;

        public VoidGameEvent exitGame;

        public VoidGameEvent goToMainMenu;

        public VoidGameEvent goToInstantAction;

        public VoidGameEvent LoadMainScene;

        public BoolGameEvent pauseGameToggle;


        public void Start()
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
            LevelLoader = LevelLoader.instance;
        }

     
        void SelectInstantAction()
        {
            Debug.Log("SelectInstantAction");
            int instantAction = 1;
            LevelLoader.LoadLevel(instantAction);
        }
        void SelectMainMenu()
        {
            Debug.Log("SelectMainMenu");
            int mainMenu = 0;
            LevelLoader.LoadLevel(mainMenu);
        }
        void QuiteGame()
        {
           Debug.Log("Quite Game");
          // Application.Quit();
        }


    }
}
