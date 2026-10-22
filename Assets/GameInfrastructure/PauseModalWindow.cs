using UnityEngine;
namespace GameInfrastructure
{
    public class PauseModalWindow : MonoBehaviour
    {
        public VoidGameEvent PauseModalWindowWindowOpenEvent;
        public VoidGameEvent PauseModalWindowWindowCloseEvent;
        public VoidGameEvent PauseModalWindowWindowReturnToMainMenuEvent;
        public VoidGameEvent PauseModalWindowRestartLevelEvent;
        
        public void OnClosePauseWindow()
        {
            PauseModalWindowWindowCloseEvent?.FireEvent();
        }
        public void OnOpenPauseWindow()
        {
            
       
            PauseModalWindowWindowOpenEvent?.FireEvent();
            
        }
        public void OnReturnToMainMenu()
        {
            PauseModalWindowWindowReturnToMainMenuEvent?.FireEvent();
        }

        public void OnRestartLevelPauseWindow()
        {
            PauseModalWindowRestartLevelEvent?.FireEvent();
        }

    }
}