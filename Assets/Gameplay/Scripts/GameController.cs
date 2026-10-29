using GameInfrastructure;
using UnityEngine;
namespace Gameplay
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private Transform player;

        [SerializeField] private PauseModalWindow pauseMenuController;

        public static GameController gameController;


        bool lose;
        bool win;
        public bool alive;

        private void Start()
        {
            gameController = this;
        }


        private void Update()
        {
            alive = player != null;


            if(lose || win || !alive)
            {
                pauseMenuController.OnOpenPauseWindow();
            }
           
        }




    }
}