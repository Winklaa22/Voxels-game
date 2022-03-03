using System;
using System.Collections.Generic;
using Management.VoxelManagement;
using Management.WorldManagement;
using Tools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Management.ChunkManagement
{
    public class Chunk
    {
        private ChunkCoord _coordinates;
        private GameObject _chunkObject;
        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;
        private MeshCollider _collider;
        private int _texture;
        private int _vertexIndex = 0;
        private List<Vector3> _vertices = new List<Vector3>();
        private List<int> _triangles = new List<int>();
        private List<Vector2> _uvs = new List<Vector2>();
        private byte[,,] _voxelMap;
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

        public Vector3 Position
        {
            get
            {
                return _chunkObject.transform.position;
            }
        }

        public bool IsActive
        {
            get
            {
                return _chunkObject.activeSelf;
            }
            
            set
            {
                _chunkObject.SetActive(value);
            }
        }
        
        

        public Chunk(ChunkCoord coord, WorldManager worldManager)
        {

            _voxelMap = new byte[WorldManager.Instance.ChunkWidth, WorldManager.Instance.ChunkHeight, WorldManager.Instance.ChunkWidth];
            _chunkObject = new GameObject()
            {
                tag = "Chunk",
                
                transform =
                {
                    name = "Chunk " + coord.x + ", " + coord.z,
                    parent = worldManager.transform,
                    position = new Vector3(coord.x * WorldManager.Instance.ChunkWidth, 0, coord.z * WorldManager.Instance.ChunkWidth)
                }
            };

            _worldManager = worldManager;
            _collider = _chunkObject.AddComponent<MeshCollider>();
            _meshFilter = _chunkObject.AddComponent<MeshFilter>();
            _meshRenderer = _chunkObject.AddComponent<MeshRenderer>();
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
                        if (!_worldManager.BlockTypes[_voxelMap[x, y, z]].IsSolid) 
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
        
        public byte GetVoxelFromGlobalVector3 (Vector3 pos) {

            int xCheck = Mathf.FloorToInt(pos.x);
            int yCheck = Mathf.FloorToInt(pos.y);
            int zCheck = Mathf.FloorToInt(pos.z);

            xCheck -= Mathf.FloorToInt(_chunkObject.transform.position.x);
            zCheck -= Mathf.FloorToInt(_chunkObject.transform.position.z);

            return _voxelMap[xCheck, yCheck, zCheck];

        }
        
        public void EditVoxel (int x, int y, int z, byte newID) {



            x -= Mathf.FloorToInt(_chunkObject.transform.position.x);
            z -= Mathf.FloorToInt(_chunkObject.transform.position.z);

            _voxelMap[x, y, z] = newID;

            UpdateSurroundingVoxels(x, y, z);

            UpdateChunk();

        }
        
        void UpdateChunk () {

            ClearMeshData();

            for (int y = 0; y < WorldManager.Instance.ChunkHeight; y++) {
                for (int x = 0; x < WorldManager.Instance.ChunkWidth; x++) {
                    for (int z = 0; z < WorldManager.Instance.ChunkWidth; z++) {

                        if (WorldManager.Instance.BlockTypes[_voxelMap[x,y,z]].IsSolid)
                            AddVoxelData(new Vector3(x, y, z));

                    }
                }
            }

            CreateMesh();

        }
        
        void ClearMeshData () {

            _vertexIndex = 0;
            _vertices.Clear();
            _triangles.Clear();
            _uvs.Clear();

        }

        void UpdateSurroundingVoxels (int x, int y, int z) {

            Vector3 thisVoxel = new Vector3(x, y, z);

            for (int p = 0; p < 6; p++) {

                Vector3 currentVoxel = thisVoxel + VoxelData.FaceCheck[p];

                if (!IsVoxelInChunk((int)currentVoxel.x, (int)currentVoxel.y, (int)currentVoxel.z)) {

                    WorldManager.Instance.GetChunkFromVector3(currentVoxel + Position).UpdateChunk();

                }

            }

        }

        private void PopulateVoxelMap()
        {
            for (int y = 0; y < WorldManager.Instance.ChunkHeight; y++)
            {
                for (int x = 0; x < WorldManager.Instance.ChunkWidth; x++)
                {
                    for (int z = 0; z < WorldManager.Instance.ChunkWidth; z++)
                    {
                        byte voxelIndex = _worldManager.GetVoxel(new Vector3(x, y ,z) + Position); 

                        _voxelMap[x, y, z] = voxelIndex;
                    }
                }
            }
        }

        public void CreateVoxel(Vector3 pos, byte type)
        {
            var intPos = ProMath.FloorVector3ToInt(pos);
            AddVoxelData(new Vector3(intPos.x, intPos.y, intPos.z));
            CreateMesh();
        }

        private bool IsInsideChunk(int x, int y, int z)
        {
            return y < 0 || y > WorldManager.Instance.ChunkHeight - 1 || x < 0 || x > WorldManager.Instance.ChunkWidth - 1 || z < 0 || z > WorldManager.Instance.ChunkWidth - 1;
        }

        private bool HasNeighbourVoxel(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x);
            int y = Mathf.FloorToInt(pos.y);
            int z = Mathf.FloorToInt(pos.z);
            
            if (IsInsideChunk(x, y, z))
                return _worldManager.BlockTypes[_worldManager.GetVoxel(pos + Position)].IsSolid;

            return _worldManager.BlockTypes[_voxelMap[x, y, z]].IsSolid;
        }
        
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

                byte voxelID = _voxelMap[(int) pos.x, (int) pos.y, (int) pos.z];
                
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
            float textureSize = WorldManager.Instance.BlockOnAtlasSize;
            float y = textureID / WorldManager.Instance.AtlasSize;
            float x = textureID - (y * WorldManager.Instance.AtlasSize);

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
