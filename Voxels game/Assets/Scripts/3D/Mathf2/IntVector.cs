using UnityEngine;

namespace _3D.Mathf2
{
    [System.Serializable]
    public class IntVector
    {
        public int x;
        public int y;
        public int z;

        public IntVector(Vector3 vector3)
        {
            x = Mathf.FloorToInt(vector3.x);
            y = Mathf.FloorToInt(vector3.y);
            z = Mathf.FloorToInt(vector3.z);
        }
        
        
        
        public IntVector(Vector2 vector2)
        {
            x = Mathf.FloorToInt(vector2.x);
            y = Mathf.FloorToInt(vector2.y);
        }

        public IntVector(int _x, int _y, int _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }
}
