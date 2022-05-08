using UnityEngine;

namespace Types._Cursor
{
    [CreateAssetMenu(fileName = "new cursor type", menuName = "Cursor Type")]
    public class CursorType : ScriptableObject
    {
        public CursorName Name;
        public Texture2D MainTexture;
    }
}
