using GameInfrastructure;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Player
{
    public class PlayerInputController : MonoBehaviour
    {

        public InputActionAsset inputAsset;

        private InputActionMap inputActions;

        [SerializeField] private string openClosePauseWindowName;
        [SerializeField] private PauseModalWindow pauseWindow;
        private InputAction openClosePauseWindowAction;
        public bool openClosePauseWindow;
    
        private void Awake()
        {
            InputActionMap inputActions = inputAsset.FindActionMap("UI");
            openClosePauseWindowAction = inputActions.FindAction(openClosePauseWindowName);
            inputActions.Enable();
        }

        public void OnEnable()
        {

        }
        public void OnDisable()
        {


        }
        void OpenClosePauseWindow()
        {

            if (openClosePauseWindowAction.WasPressedThisFrame())
            {
                openClosePauseWindow = !openClosePauseWindow;
                pauseWindow.OnOpenPauseWindow();
                
            }
            if (openClosePauseWindow)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }


        }
        private void Update()
        {
            OpenClosePauseWindow();
        }
       
    }
}
