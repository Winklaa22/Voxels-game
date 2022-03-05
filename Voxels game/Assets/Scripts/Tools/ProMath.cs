using UnityEngine;

namespace Tools
{
    public static class ProMath
    {
        public static int[] FloorVector3ToInt(Vector3 vector)
        {
            var intVector = new int[3]
            {
                Mathf.FloorToInt(vector.x),
                Mathf.FloorToInt(vector.y),
                Mathf.FloorToInt(vector.z),
            };
            


            return intVector;
        }

    }
}
