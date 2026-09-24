using UnityEngine;
using System.Collections.Generic;
using System;
namespace GameInfrastructure
{
    [CreateAssetMenu(fileName = "BoolGameEvent", menuName = "Scriptable Objects/BoolGameEvent")]
    public class BoolGameEvent : ScriptableObject
    {

        public Action<bool>  boolGameEvent { get; set; }

        public void FireEvent(bool flag)
        {
            boolGameEvent.Invoke(flag);
        }
    }
}