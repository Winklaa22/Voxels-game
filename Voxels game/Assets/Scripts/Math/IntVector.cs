using System;
using UnityEngine;

namespace NewMathf
{
    [Serializable]
    public class IntVector
    {
        [SerializeField] private int _x, _y, _z;

        public int x
        {
            get => _x;
            set => _x = value;
        }

        public int y
        {
            get => _y;
            set => _y = value;
        }

        public int z
        {
            get => _z;
            set => _z = value;
        }


        public IntVector(int x, int y, int z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public IntVector(Vector3 vector3)
        {
            _x = Mathf.FloorToInt(vector3.x);
            _y = Mathf.FloorToInt(vector3.y);
            _z = Mathf.FloorToInt(vector3.z);
        }
        
        public IntVector(Vector2 vector2)
        {
            _x = Mathf.FloorToInt(vector2.x);
            _y = Mathf.FloorToInt(vector2.y);
        }

        public Vector3 ToVector3()
        {
            return new Vector3(_x, _y, _z);
        }
    }
}
