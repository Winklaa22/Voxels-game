using System;
using System.Collections;
using System.Collections.Generic;
using Block;
using Texture.Block;
using UnityEngine;
using VoxelManagement;

namespace ChunkManagement
{
    public class Chunk : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private MeshFilter _meshFilter;

        private int width = VoxelData.ChunkSize[0];
        private int height = VoxelData.ChunkSize[1];
        private int _vertexIndex = 0;
        private List<Vector3> _vertices = new List<Vector3>();
        private List<int> _triangles = new List<int>();
        private List<Vector2> _uvs = new List<Vector2>();
        private bool[,,] _voxelMap = new bool[VoxelData.ChunkSize[0] + 1, VoxelData.ChunkSize[1] + 1, VoxelData.ChunkSize[0] + 1];

        private void Start()
        {
            PopulateVoxelMap();
            CreateChunk();
            
            
        }

        private void CreateChunk()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < width; z++)
                    {
                        AddVoxelData(new Vector3(x, y, z));
                        CreateMesh();
                    }
                }
            }

        }

        private void PopulateVoxelMap()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < width; z++)
                    {
                        _voxelMap[x, y, z] = true;
                    }
                }
            }
        }

        private bool HasNeighbourVoxel(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x);
            int y = Mathf.FloorToInt(pos.y);
            int z = Mathf.FloorToInt(pos.z);
            
            if (y < 0 || y > height || x < 0 || x > width - 1 || z < 0 || z > width)
                return false;

            Debug.Log(x + ", " + y + ", " + z);
            
            return _voxelMap[x, y, z];
        }

        private void AddVoxelData(Vector3 pos)
        {
            for (int i = 0; i < 6; i++)
            {
                if (HasNeighbourVoxel(pos + VoxelData.FaceCheck[i])) 
                    continue;
                
                for (int j = 0; j < 6; j++)
                {
                    int trisIndex = VoxelData.Triangles[i, j];
                    _vertices.Add(VoxelData.Vertices[trisIndex] + pos);
                    _triangles.Add(_vertexIndex);
                    _uvs.Add(VoxelData.UVs[j]);
                    _vertexIndex++;
                }
            }
        }

        private void CreateMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = _vertices.ToArray();
            mesh.triangles = _triangles.ToArray();
            mesh.uv = _uvs.ToArray();
            mesh.RecalculateNormals();
            _meshFilter.mesh = mesh;
        }
    }

}
