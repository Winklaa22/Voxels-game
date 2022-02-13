using System;
using System.Collections.Generic;
using Management.VoxelManagement;
using Management.WorldManagement;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Management.ChunkManagement
{
    public class Chunk : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private int _texture;

        private int width = VoxelData.ChunkSize[0];
        private int height = VoxelData.ChunkSize[1];
        private int _vertexIndex = 0;
        private List<Vector3> _vertices = new List<Vector3>();
        private List<int> _triangles = new List<int>();
        private List<Vector2> _uvs = new List<Vector2>();
        private byte[,,] _voxelMap = new byte[VoxelData.ChunkSize[0], VoxelData.ChunkSize[1], VoxelData.ChunkSize[0]];
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
                        AddVoxelData(new Vector3(x, y, z), _texture);
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
                        _voxelMap[x, y, z] = 0;
                    }
                }
            }
        }

        private bool HasNeighbourVoxel(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x);
            int y = Mathf.FloorToInt(pos.y);
            int z = Mathf.FloorToInt(pos.z);
            
            if (y < 0 || y > height - 1 || x < 0 || x > width - 1 || z < 0 || z > width - 1)
                return false;

            return World.Instance.BlockTypes[_voxelMap[x, y, z]].IsSolid;
        }

        private void AddVoxelData(Vector3 pos, int textureID)
        {
            for (int i = 0; i < 6; i++)
            {
                if (HasNeighbourVoxel(pos + VoxelData.FaceCheck[i])) 
                    continue;

                byte voxelID = _voxelMap[(int) pos.x, (int) pos.y, (int) pos.z];
                
                AddTexture(World.Instance.BlockTypes[voxelID].GetTextureIDFromSide(VoxelData.Sides[i]));
                
                for (int j = 0; j < 6; j++)
                {
                    int trisIndex = VoxelData.Triangles[i, j];
                    _vertices.Add(VoxelData.Vertices[trisIndex] + pos);
                    _triangles.Add(_vertexIndex);
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

        private void AddTexture(int textureID)
        {
            float y = textureID / VoxelData.TextureAtlasSize;
            float x = textureID - (y * VoxelData.TextureAtlasSize);

            x *= VoxelData.BlockTextureSize;
            y *= VoxelData.BlockTextureSize;

            y = 1f - y - VoxelData.BlockTextureSize;

            for (int i = 0; i < VoxelData.UVs.Length; i++)
            {
                float xOffset = VoxelData.UVs[i].x.Equals(1) ? VoxelData.BlockTextureSize : 0;
                float yOffset = VoxelData.UVs[i].y.Equals(1) ? VoxelData.BlockTextureSize : 0;
                
                _uvs.Add(new Vector2(x + xOffset, y + yOffset));
            }
        }

    }

}
