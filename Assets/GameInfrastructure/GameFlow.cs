using UnityEngine;
using System.Collections.Generic;
namespace GameInfrastructure
{
 
    public class GameFlow : MonoBehaviour
    {

        public static GameFlow instance;
      public List<Updatable> updatables = new List<Updatable>();

        public bool paused;
        
        private void OnEnable()
        {
            instance = this;
        }
        private void Start()
        {
            if (updatables.Count > 0)
            {
                for (int i = 0; i < updatables.Count; i++)
                {
                    updatables[i].CustomStart();
                }
            }
        }
           
        private void FixedUpdate()
        {

        
                for (int i = 0; i < updatables.Count; i++)
                {
                    if (!paused)
                    {
                        Debug.Log(i);
                        updatables[i].CustomFixedUpdate();
                    }
                }
            
        }
        private void LateUpdate()
        {
         
                if (updatables.Count > 0)
                {
                    for (int i = 0; i < updatables.Count; i++)
                {
                    if (!paused)
                    {
                        updatables[i].CustomLateUpdate();
                    }

                }
            }
        }
        private void Update()
        {

            for (int i = 0; i < updatables.Count; i++)
            {
                if (!paused)
                {

                    updatables[i].CustomUpdate();
                }
            }
            
        }

        public void AddUpdatable(Updatable updatable)
        {
            if (!updatables.Contains(updatable))
            {
                updatables?.Add(updatable);
            }
        }

        public void RemoveUpdatable(Updatable updatable)
        {
            if (updatables.Contains(updatable))
            {
                updatables?.Remove(updatable);
            }
        }
        public bool GetPaused()
        {
            return paused;
        }


        public void SetPaused(bool paused)
        {

            this.paused = !this.paused;
            
        }

    }
}