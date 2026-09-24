using UnityEngine;
namespace GameInfrastructure
{
    public class InstantActionMenu : MonoBehaviour
    {
        public VoidGameEvent EnterMainEvent;

        public void OnEnterMainMenu()
        {
            EnterMainEvent?.FireEvent();
        }
    }
}