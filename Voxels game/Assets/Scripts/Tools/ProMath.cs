using UnityEngine;

namespace Tools
{
    public static class ProMath
    {
        public static Vector3 FloorVector3ToInt(Vector3 vector)
        {
            var x = Mathf.FloorToInt(vector.x);
            var y = Mathf.FloorToInt(vector.y);
            var z = Mathf.FloorToInt(vector.z);

            return new Vector3(x, y, z);
        }

    }
}
