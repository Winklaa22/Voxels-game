using System;
using UnityEngine;

namespace Math
{
    public class IntVector
    {
        public int x { get; set;}
        public int y { get; set;}
        public int z { get; set;}

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
    }
}