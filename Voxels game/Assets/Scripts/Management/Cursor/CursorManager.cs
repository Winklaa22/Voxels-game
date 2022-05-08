using System;
using Types._Cursor;
using UnityEngine;

namespace Management.Cursor
{
    public class CursorManager : MonoBehaviour
    {
        public static CursorManager Instance;
        private ActionsManager _inputs;
        [SerializeField] private CursorType[] _cursors;
        
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

        public void SetCursor(CursorName name)
        {
            foreach (var cursor in _cursors)
            {
                if(!cursor.Name.Equals(name))
                    continue;
                
                UnityEngine.Cursor.SetCursor(cursor.MainTexture, Vector2.zero, CursorMode.Auto);
            }
        }
    } 
}
