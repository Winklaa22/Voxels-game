using Management._Cursor;
using Management.Game;
using Management.UI;
using UnityEngine;

namespace Controllers.Pause
{
    public class PauseController : MonoBehaviour
    {
        private ActionsManager _inputs;
        private bool _active;

        private void Awake()
        {
            _inputs = new ActionsManager();
            _inputs.Enable();
            
            _inputs.Player.PauseGame.started += ctx => ChangeActive();
        }

        private void ChangeActive()
        {
            _active = !_active;
            CursorManager.Instance.SetActive(_active);
            var type = _active ? ScreenType.PAUSE : ScreenType.HUD;
            UIManager.Instance.SetScreen(type);
            GameManager.Instance.MainPlayer.SetActive(!_active);
        }
    }
}
