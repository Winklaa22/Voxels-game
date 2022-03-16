using System;
using System.Collections.Generic;
using Management.VoxelManagement;
using Management.WorldManagement;
using Math;
using Tools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Management.ChunkManagement
{
    public class Chunk : MonoBehaviour
    {
        private ChunkCoord _coordinates;
        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;
        private MeshCollider _collider;
        private int _texture;
        private int _vertexIndex = 0;

        [Header("Mesh")]
        private List<Vector3> _vertices = new List<Vector3>();
        private List<int> _triangles = new List<int>();
        private List<Vector2> _uvs = new List<Vector2>();
        
        [Header("Map")]
        private Voxel[,,] _voxelMap;

        private WorldManager _worldManager;
        private bool _isVoxelMapPopulated = false;

        public bool IsVoxelMapPopulated
        {
            get
            {
                return _isVoxelMapPopulated;
            }
            set
            {
                _isVoxelMapPopulated = value;
            }
        }

        public Vector3 position
        {
            get
            {
                return gameObject.transform.position;
            }
        }

        public bool IsActive
        {
            get
            {
                return gameObject.activeSelf;
            }
            
            set
            {
                gameObject.SetActive(value);
            }
        }

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            _voxelMap = new Voxel[WorldManager.Instance.ChunkWidth, WorldManager.Instance.ChunkHeight, WorldManager.Instance.ChunkWidth];
            _collider = gameObject.AddComponent<MeshCollider>();
            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            _worldManager = WorldManager.Instance;
            _meshRenderer.material = _worldManager.WorldMaterial;

            
            PopulateVoxelMap();
            CreateChunk();
            SetCollisionActive(true);
        }
        
        private void CreateChunk()
        {
            for (int y = 0; y < WorldManager.Instance.ChunkHeight; y++)
            {
                for (int x = 0; x < WorldManager.Instance.ChunkWidth; x++)
                {
                    for (int z = 0; z < WorldManager.Instance.ChunkWidth; z++)
                    {
                        if (!_worldManager.BlockTypes[_voxelMap[x, y, z].ID].IsSolid) 
                            continue;

                        AddVoxelData(new Vector3(x, y, z));
                        CreateMesh();

                    }
                }
            }

        }

        public void SetCollisionActive(bool active)
        {
            _collider.sharedMesh = active ? _meshFilter.mesh : null;
        }
        
        public Voxel GetVoxel (Vector3 pos)
        {
            var checkVector = new IntVector(pos);
            var chunkPos = new IntVector(position);
            
            checkVector.x -= chunkPos.x;
            checkVector.z -= chunkPos.z;

            return _voxelMap[checkVector.x, checkVector.y, checkVector.z];

        }
        
        
        public void SetVoxel (Voxel voxel, byte id)
        {
            voxel.ID = id;
            UpdateChunk();
            SetCollisionActive(true);
        }

        void UpdateChunk () {

            ClearMesh();

            for (int y = 0; y < WorldManager.Instance.ChunkHeight; y++) {
                for (int x = 0; x < WorldManager.Instance.ChunkWidth; x++) {
                    for (int z = 0; z < WorldManager.Instance.ChunkWidth; z++) {

                        if (WorldManager.Instance.BlockTypes[_voxelMap[x,y,z].ID].IsSolid)
                            AddVoxelData(new Vector3(x, y, z));

                    }
                }
            }

            CreateMesh();

        }

        private void UpdateAmbientVoxels()
        {
            for (int i = 0; i < 6; i++)
            {
                
            }
        }
        
        private void ClearMesh () 
        {

            _vertexIndex = 0;
            _vertices.Clear();
            _triangles.Clear();
            _uvs.Clear();

        }

        private void PopulateVoxelMap()
        {
            for (int y = 0; y < WorldManager.Instance.ChunkHeight; y++)
            {
                for (int x = 0; x < WorldManager.Instance.ChunkWidth; x++)
                {
                    for (int z = 0; z < WorldManager.Instance.ChunkWidth; z++)
                    {
                        var voxelPosition = new Vector3(x, y, z) + position;
                        byte voxelIndex = _worldManager.GetVoxelByPosition(voxelPosition);

                        var intPos = new IntVector(voxelPosition);
                        _voxelMap[x, y, z] = new Voxel(voxelIndex, new Vector3(intPos.x, intPos.y, intPos.z));
                    }
                }
            }
        }

        private bool IsInsideChunk(int x, int y, int z)
        {
            return y < 0 || y > WorldManager.Instance.ChunkHeight - 1 || x < 0 || x > WorldManager.Instance.ChunkWidth - 1 || z < 0 || z > WorldManager.Instance.ChunkWidth - 1;
        }

        private bool HasNeighbourVoxel(Vector3 pos)
        {
            var intPos = new IntVector(pos);
            
            if (IsInsideChunk(intPos.x, intPos.y, intPos.z))
                return _worldManager.BlockTypes[_worldManager.GetVoxelByPosition(pos + position)].IsSolid;

            return _worldManager.BlockTypes[_voxelMap[intPos.x, intPos.y, intPos.z].ID].IsSolid;
        }

        // public byte SetVoxel(Vector3 pos, byte id)
        // {
        //     var voxelPos = new IntVector(pos);
        //     var chunkPos = new IntVector(_chunkObject.transform.position);
        //
        //     voxelPos.x -= voxelPos.x / chunkPos.x;
        //     voxelPos.z -= voxelPos.z / chunkPos.z;
        //
        //     return ;
        // }
        
        bool IsVoxelInChunk (int x, int y, int z)
        {
            return !(x < 0 || x > WorldManager.Instance.ChunkWidth - 1 || y < 0 ||
                     y > WorldManager.Instance.ChunkHeight - 1 || z < 0 || z > WorldManager.Instance.ChunkWidth - 1);
        }

        public void AddVoxelData(Vector3 pos)
        {
            for (int i = 0; i < 6; i++)
            {
                if (HasNeighbourVoxel(pos + VoxelData.FaceCheck[i])) 
                    continue;

                var voxelPos = new IntVector(pos);

                var voxelID = _voxelMap[voxelPos.x, voxelPos.y, voxelPos.z].ID;
                
                AddTexture(_worldManager.BlockTypes[voxelID].GetTextureIDFromSide(VoxelData.Sides[i]));
                
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
            var textureSize = WorldManager.Instance.BlockOnAtlasSize;
            float y = textureID / WorldManager.Instance.AtlasSize;
            var x = textureID - (y * WorldManager.Instance.AtlasSize);
            
            x *= textureSize;
            y *= textureSize;

            y = 1f - y - textureSize;

            for (int i = 0; i < VoxelData.UVs.Length; i++)
            {
                float xOffset = VoxelData.UVs[i].x.Equals(1) ? textureSize : 0;
                float yOffset = VoxelData.UVs[i].y.Equals(1) ? textureSize : 0;
                
                _uvs.Add(new Vector2(x + xOffset, y + yOffset));
            }
        }

    }

}
