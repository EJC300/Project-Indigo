using UnityEngine;
namespace GameInfrastructure
{
    public class InstantActionMenu : MonoBehaviour
    {
        public VoidGameEvent EnterMainEvent;
        public VoidGameEvent EnterBattleEvent;
        public void OnEnterMainMenu()
        {
            EnterMainEvent?.FireEvent();
        }

        public void OnEnterBattle()
        {
            EnterBattleEvent?.FireEvent();
        }
    }
}