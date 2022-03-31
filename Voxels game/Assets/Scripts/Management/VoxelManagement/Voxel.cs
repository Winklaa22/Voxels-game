using System.Collections.Generic;
using Management.WorldManagement;
using UnityEngine;

namespace Management.VoxelManagement
{
    public class Voxel : MonoBehaviour
    {
        [Header("Mesh")] 
        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;
        private MeshCollider _collider;
        private List<Vector3> _vertices = new List<Vector3>();
        private List<int> _triangles = new List<int>();
        private List<Vector2> _uvs = new List<Vector2>();
        private int _vertexIndex = 0;

        public void CreateMesh()
        {
            var mesh = new Mesh();
            mesh.vertices = _vertices.ToArray();
            mesh.triangles = _triangles.ToArray();
            mesh.uv = _uvs.ToArray();
            mesh.RecalculateNormals();
            _meshFilter.mesh = mesh;
        }


    }
}
