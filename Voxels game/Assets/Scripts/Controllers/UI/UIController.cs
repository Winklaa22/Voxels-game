using Controllers;
using Management._Cursor;
using Management.Game;
using Management.UI;
using UnityEngine;

namespace Assets.Scripts.Controllers.UI
{
    public class UIController : MonoBehaviour
    {
        private ActionsManager _inputs;

        private void Awake()
        {
            _inputs = new ActionsManager();
            _inputs.Enable();
            _inputs.Player.InventoryActive.started += context => SetScreen(ScreenType.INVENTORY);
            _inputs.Player.PauseGame.started += context =>  SetScreen(ScreenType.PAUSE);
        }
        
        
        private void SetScreen(ScreenType type)
        {
            if(UIManager.Instance.CurrentScreen != ScreenType.HUD && UIManager.Instance.CurrentScreen != type)
                return;

            var active = UIManager.Instance.CurrentScreen.Equals(type);
            var screen = !active ? type : ScreenType.HUD;
            
            CursorManager.Instance.SetActive(!active);
            GameManager.Instance.MainPlayer.SetActive(active);
            UIManager.Instance.SetScreen(screen);
            
        }
    }
}
