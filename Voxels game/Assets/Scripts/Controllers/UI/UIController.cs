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
            _inputs.Player.InventoryActive.started += context => SetScreen(ScreenType.INVENTORY, true);
            _inputs.Player.PauseGame.started += context =>
            {
                switch (UIManager.Instance.CurrentScreen)
                {
                    case ScreenType.RENDER:
                        return;
                    case ScreenType.HUD:
                        SetScreen(ScreenType.PAUSE, true);
                        break;
                    default:
                        SetScreen(UIManager.Instance.CurrentScreen, false);
                        break;
                }
            };
        }
        
        
        private void SetScreen(ScreenType type, bool active)
        {
            if(UIManager.Instance.CurrentScreen != ScreenType.HUD && UIManager.Instance.CurrentScreen != type)
                return;

            var screen = active ? type : ScreenType.HUD;
            
            CursorManager.Instance.SetActive(active);
            GameManager.Instance.MainPlayer.SetActive(!active);
            GameManager.Instance.CantBuild = active;
            UIManager.Instance.SetScreen(screen);
            
        }
    }
}
