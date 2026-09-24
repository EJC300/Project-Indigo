using UnityEngine;
using System.Collections.Generic;
using System;


namespace GameInfrastructure
{
    [CreateAssetMenu(fileName = "VoidGameEvent", menuName = "Scriptable Objects/GameEvents")]
    public class VoidGameEvent : ScriptableObject
    {
        public string EventName; //For Design Purposes

        public Action voidEventToFire { get; set; }

        public void FireEvent()
        {
            voidEventToFire?.Invoke();
        }
        
       
    }
}
