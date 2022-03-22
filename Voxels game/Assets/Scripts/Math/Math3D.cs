namespace NewMathf
{
    public static class Math3D
    {
        public static bool IsInsideObject(IntVector coord, IntVector objectSize)
        {
            return coord.x < 0 || coord.x > objectSize.x - 1 || coord.y < 0 || coord.y > objectSize.y -1 || coord.z < 0 || coord.z > objectSize.z - 1;
        }
    }
}