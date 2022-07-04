using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Assets.Scripts.Chunks.Jobs
{
    public struct GeneratingDataJob : IJob
    {
        public NativeList<Vector3> vertices;
        public NativeList<int> triangles;

        public NativeArray<Vector3> verticesData;
        public NativeArray<int> trianglesData;

        public NativeArray<Vector3> position;
        public NativeArray<int> index;
        
        public void Execute()
        {
            for (int i = 0; i < 6; i++)
            {
                var trisIndex = trianglesData[i];
                vertices.Add(verticesData[trisIndex] + position[0]);
                triangles.Add(index[0]);
                index[0]++;
            }
        }
    }
}