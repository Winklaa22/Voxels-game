using System;
using System.Collections;
using System.Collections.Generic;
using _3D.Mathf2;
using Management.VoxelManagement;
using Management.WorldManagement;
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

        [Header("Mesh")] private Mesh _mesh;
        private List<Vector3> _vertices = new List<Vector3>();
        private List<int> _triangles = new List<int>();
        private List<Vector2> _uvs = new List<Vector2>();

        [Header("Map")] private byte[,,] _voxelMap;

        private WorldManager _worldManager;
        private bool _isVoxelMapPopulated = false;
        [SerializeField] private bool _isGenerated;

        public bool IsGenerated
        {
            set
            {
                _isGenerated = value;
            }
        }

        private IntVector _size
        {
            get
            {
                return WorldManager.Instance.ChunkSize;
            }
        }

        public bool IsVoxelMapPopulated
        {
            get { return _isVoxelMapPopulated; }
            set { _isVoxelMapPopulated = value; }
        }

        public Vector3 position
        {
            get { return gameObject.transform.position; }
        }

        public bool IsActive
        {
            get { return gameObject.activeSelf; }

            set { gameObject.SetActive(value); }
        }

        private void Start()
        {
            StartCoroutine(TryToCreate());
        }

        private IEnumerator TryToCreate()
        {
            if(_isGenerated)
            {
                yield return Initialize();
            }
        }

        private IEnumerator Initialize()
        {
            _voxelMap = new byte[WorldManager.Instance.ChunkSize.x, WorldManager.Instance.ChunkSize.y,
                WorldManager.Instance.ChunkSize.z];
            _collider = gameObject.AddComponent<MeshCollider>();
            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            _worldManager = WorldManager.Instance;
            _meshRenderer.material = _worldManager.WorldMaterial;


            PopulateVoxelMap();
            CreateChunk();
            SetCollisionActive(true);

            yield return null;
        }

        private void CreateChunk()
        {
            for (int y = 0; y < WorldManager.Instance.ChunkSize.y; y++)
            {
                for (int x = 0; x < WorldManager.Instance.ChunkSize.x; x++)
                {
                    for (int z = 0; z < WorldManager.Instance.ChunkSize.z; z++)
                    {
                        if (!_worldManager.BlockTypes[_voxelMap[x, y, z]].IsSolid)
                            continue;

                        AddVoxelData(new Vector3(x, y, z));
                        CreateMesh();

                    }
                }
            }

        }

        private void SetCollisionActive(bool active)
        {
            _collider.sharedMesh = active ? _meshFilter.mesh : null;
        }

        public IntVector GetVoxel(Vector3 pos)
        {
            var checkVector = new IntVector(pos);
            var chunkPos = new IntVector(position);

            checkVector.x -= chunkPos.x;
            checkVector.z -= chunkPos.z;

            return new IntVector(checkVector.x, checkVector.y, checkVector.z);
        }


        public void SetVoxel(IntVector voxelCoord, byte id)
        {
            if (!Math3D.IsInsideTheObject(voxelCoord, WorldManager.Instance.ChunkSize))
            {
                _voxelMap[voxelCoord.x, voxelCoord.y, voxelCoord.z] = id;
                UpdateChunk();
                SetCollisionActive(true);
                UpdateNearestVoxel(voxelCoord);
            }
            else
            {
                var chunk = WorldManager.Instance.GetChunkFromVector3(voxelCoord.ToVector3() + position);
                var voxelPosition = chunk.GetVoxel(voxelCoord.ToVector3() + position);
                chunk.SetVoxel(voxelPosition , id);
                
            }
        }

        private void UpdateNearestVoxel(IntVector coord)
        {
            for (int i = 0; i < 6; i++)
            {
                var nearestVoxelPosition = new IntVector(coord.ToVector3() + VoxelData.FaceCheck[i]);
                if (!Math3D.IsInsideTheObject(nearestVoxelPosition, WorldManager.Instance.ChunkSize))
                    continue;


                var chunk = WorldManager.Instance.GetChunkFromVector3(nearestVoxelPosition.ToVector3() + position);
                chunk.UpdateChunk();
            }
        }

        private void UpdateChunk()
        {
            ClearMesh();

            for (int y = 0; y < WorldManager.Instance.ChunkSize.y; y++)
            {
                for (int x = 0; x < WorldManager.Instance.ChunkSize.x; x++)
                {
                    for (int z = 0; z < WorldManager.Instance.ChunkSize.z; z++)
                    {
                        if (WorldManager.Instance.BlockTypes[_voxelMap[x, y, z]].IsSolid)
                            AddVoxelData(new Vector3(x, y, z));
                    }
                }
            }

            CreateMesh();
        }

        private void ClearMesh()
        {
            _vertexIndex = 0;
            _vertices.Clear();
            _triangles.Clear();
            _uvs.Clear();
            _mesh.Clear();
        }

        private void PopulateVoxelMap()
        {
            for (int y = 0; y < WorldManager.Instance.ChunkSize.y; y++)
            {
                for (int x = 0; x < WorldManager.Instance.ChunkSize.x; x++)
                {
                    for (int z = 0; z < WorldManager.Instance.ChunkSize.z; z++)
                    {
                        var voxelPosition = new Vector3(x, y, z) + position;
                        var voxelIndex = _worldManager.GetVoxelByPosition(voxelPosition);

                        _voxelMap[x, y, z] = voxelIndex;
                    }
                }
            }
        }

        public byte GetVoxelType(IntVector coords)
        {
            return _voxelMap[coords.x, coords.y, coords.z];
        }

        private bool IsInsideChunk(int x, int y, int z)
        {
            return y < 0 || y > WorldManager.Instance.ChunkSize.y - 1 || x < 0 ||
                   x > WorldManager.Instance.ChunkSize.x - 1 || z < 0 || z > WorldManager.Instance.ChunkSize.z - 1;
        }

        private bool CanRenderSide(Vector3 pos)
        {
            var intPos = new IntVector(pos);

            return !IsInsideChunk(intPos.x, intPos.y, intPos.z)
                ? _worldManager.BlockTypes[_voxelMap[intPos.x, intPos.y, intPos.z]].IsSolid
                : _worldManager.BlockTypes[_worldManager.GetVoxelByPosition(pos + position)].IsSolid;
        }

        public void AddVoxelData(Vector3 pos)
        {
            for (int i = 0; i < 6; i++)
            {
                if (CanRenderSide(pos + VoxelData.FaceCheck[i]))
                    continue;
    
                var voxelPos = new IntVector(pos);

                AddTexture(_worldManager.BlockTypes[_voxelMap[voxelPos.x, voxelPos.y, voxelPos.z]].GetTextureIDFromSide(VoxelData.Sides[i]), ref _uvs);

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
            _mesh = new Mesh();
            _mesh.vertices = _vertices.ToArray();
            _mesh.triangles = _triangles.ToArray();
            _mesh.uv = _uvs.ToArray();
            _mesh.RecalculateNormals();
            _meshFilter.mesh = _mesh;

        }
        
        public static void AddTexture(int textureID, ref List<Vector2> uvs)
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

                uvs.Add(new Vector2(x + xOffset, y + yOffset));
            }
        }
    }
}
