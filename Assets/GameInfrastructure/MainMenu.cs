using UnityEngine;
namespace GameInfrastructure
{
    public class MainMenu : MonoBehaviour
    {
        public VoidGameEvent OnExitGame;
        public VoidGameEvent OnEnterInstantAction;

        public void ExitGame()
        {
            OnExitGame?.FireEvent();
        }

        public void EnterInstantActionMode()
        {
            OnEnterInstantAction?.FireEvent();
        }
    }
}