using System;
using Types._Cursor;
using UnityEngine;
using UnityEngine.UI;

namespace Management._Cursor
{
    public class CursorManager : MonoBehaviour
    {
        public static CursorManager Instance;
        private ActionsManager _inputs;
        [SerializeField] private CursorType[] _cursors;
        [SerializeField] private RawImage _cursorIcon;
        public RawImage CursorIcon => _cursorIcon;

        private void Awake()
        {
            Instance = this;
            _cursors = Resources.LoadAll<CursorType>("CursorTypes");

            _inputs = new ActionsManager();
            _inputs.Enable();
        }

        private void Start()
        {
            SetCursor(CursorName.Normal);
        }

        public Vector2 CursorPosition()
        {
            return _inputs.Player.MousePosition.ReadValue<Vector2>();
        }

        public void SetActive(bool active)
        {
            Cursor.visible = active;
            Cursor.lockState = active ? CursorLockMode.None : CursorLockMode.Locked;
        }

        public void SetCursor(CursorName name)
        {
            foreach (var cursor in _cursors)
            {
                if(!cursor.Name.Equals(name))
                    continue;
                
                Cursor.SetCursor(cursor.MainTexture, Vector2.zero, CursorMode.Auto);
            }
        }

        public void GrabIcon(Texture icon)
        {
            if (icon.Equals(null))
            {
                _cursorIcon.enabled = false;
                return;
            }

            _cursorIcon.enabled = true;
            _cursorIcon.texture = icon;
        }
    } 
}
