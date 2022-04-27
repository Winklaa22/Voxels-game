using System;
using System.Collections;
using System.Collections.Generic;
using _3D.Mathf2;
using Blocks.Type;
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

        [Header("Mesh")] 
        private Mesh _mesh;
        private List<Vector3> _vertices = new List<Vector3>();
        private List<int> _triangles = new List<int>();
        private List<Vector2> _uvs = new List<Vector2>();

        [Header("Map")] 
        private byte[,,] _voxelMap;

        private WorldGenerator _worldGenerator;
        private bool _isVoxelMapPopulated = false;
        [SerializeField] private bool _isGenerated;

        public byte[,,] VoxelMap
        {
            get
            {
                return _voxelMap;
            }
        }
        
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
                return WorldGenerator.Instance.ChunkSize;
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
            _voxelMap = new byte[WorldGenerator.Instance.ChunkSize.x, WorldGenerator.Instance.ChunkSize.y,
                WorldGenerator.Instance.ChunkSize.z];
            _collider = gameObject.AddComponent<MeshCollider>();
            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            _worldGenerator = WorldGenerator.Instance;
            _meshRenderer.material = _worldGenerator.WorldMaterial;
            
            StartCoroutine(TryToCreate());
        }

        private IEnumerator TryToCreate()
        {
            if (!_isGenerated) 
                yield break;
            
            Initialize();
            yield return null;
        }

        private void Initialize()
        {
            PopulateVoxelMap();
            CreateChunk();
            SetCollisionActive(true);
        }

        private void CreateChunk()
        {
            for (int y = 0; y < WorldGenerator.Instance.ChunkSize.y; y++)
            {
                for (int x = 0; x < WorldGenerator.Instance.ChunkSize.x; x++)
                {
                    for (int z = 0; z < WorldGenerator.Instance.ChunkSize.z; z++)
                    {
                        if (!_worldGenerator.BlockTypes[_voxelMap[x, y, z]].IsSolid)
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

        public byte GetVoxelID(Vector3 pos)
        {
            var coords = GetVoxel(pos);
            return _voxelMap[coords.x, coords.y, coords.z];
        }

        public IntVector GetVoxel(Vector3 pos)
        {
            var checkVector = new IntVector(pos);
            var chunkPos = new IntVector(position);

            checkVector.x -= chunkPos.x;
            checkVector.z -= chunkPos.z;
            checkVector.y = checkVector.y < 0 ? 0 : checkVector.y;
            return new IntVector(checkVector.x, checkVector.y, checkVector.z);
        }


        public void SetVoxel(IntVector voxelCoord, byte id)
        {
            if (!Math3D.IsInsideTheObject(voxelCoord, WorldGenerator.Instance.ChunkSize))
            {
                _voxelMap[voxelCoord.x, voxelCoord.y, voxelCoord.z] = id;
                UpdateChunk();
                SetCollisionActive(true);
                // UpdateNearestVoxel(voxelCoord);
            }
            else
            {
                var chunk = WorldGenerator.Instance.GetChunkFromVector3(voxelCoord.ToVector3() + position);
                var voxelPosition = chunk.GetVoxel(voxelCoord.ToVector3() + position);
                chunk.SetVoxel(voxelPosition , id);
            }
        }

        // private void UpdateNearestVoxel(IntVector coord)
        // {
        //     for (int i = 0; i < 6; i++)
        //     {
        //         var nearestVoxelPosition = new IntVector(coord.ToVector3() + _worldGenerator.BlockTypes[i]);
        //         if (!Math3D.IsInsideTheObject(nearestVoxelPosition, WorldGenerator.Instance.ChunkSize))
        //             continue;
        //         
        //         WorldGenerator.Instance.GetChunkFromVector3(nearestVoxelPosition.ToVector3() + position).UpdateChunk();
        //     }
        // }

        private void UpdateChunk()
        {
            ClearMesh();

            for (int y = 0; y < WorldGenerator.Instance.ChunkSize.y; y++)
            {
                for (int x = 0; x < WorldGenerator.Instance.ChunkSize.x; x++)
                {
                    for (int z = 0; z < WorldGenerator.Instance.ChunkSize.z; z++)
                    {
                        if (!WorldGenerator.Instance.BlockTypes[_voxelMap[x, y, z]].IsSolid)
                            continue;

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
            for (int y = 0; y < WorldGenerator.Instance.ChunkSize.y; y++)
            {
                for (int x = 0; x < WorldGenerator.Instance.ChunkSize.x; x++)
                {
                    for (int z = 0; z < WorldGenerator.Instance.ChunkSize.z; z++)
                    {
                        var voxelPosition = new Vector3(x, y, z) + position;
                        var voxelIndex = _worldGenerator.GetVoxelByPosition(voxelPosition);

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
            return y < 0 || y > WorldGenerator.Instance.ChunkSize.y - 1 || x < 0 ||
                   x > WorldGenerator.Instance.ChunkSize.x - 1 || z < 0 || z > WorldGenerator.Instance.ChunkSize.z - 1;
        }

        private bool CanRenderSide(Vector3 pos)
        {
            var intPos = new IntVector(pos);
            var blocks = _worldGenerator.BlockTypes;

            if (blocks[_worldGenerator.GetVoxelByPosition(pos + position)].IsTransparent)
                return true;

            if (IsInsideChunk(intPos.x, intPos.y, intPos.z))
            {
                return !_worldGenerator.IsVoxelExist(pos + position);
            }

            return blocks[_voxelMap[intPos.x, intPos.y, intPos.z]].IsSolid;
        }

        private void AddVoxelData(Vector3 pos)
        {
            var voxelPos = new IntVector(pos);
            var id = _voxelMap[voxelPos.x, voxelPos.y, voxelPos.z];
            var type = _worldGenerator.BlockTypes[id];

            for (int i = 0; i < type.MeshData.Faces.Length; i++)
            {
                if (CanRenderSide(pos + type.MeshData.Faces[i].Value) )
                    continue;
                
                AddTexture(type, type.GetTextureIDFromSide(VoxelData.Sides[i]), ref _uvs);

                for (int j = 0; j < type.MeshData.Faces.Length; j++)
                {
                    var trisIndex = type.MeshData.Faces[i].Triangles[j];
                    _vertices.Add(type.MeshData.Vertices[trisIndex] + pos);
                    _triangles.Add(_vertexIndex);
                    _vertexIndex++;
                }
            }
        }

        private void CreateMesh()
        {
            _mesh = new Mesh
            {
                vertices = _vertices.ToArray(),
                triangles = _triangles.ToArray(),
                uv = _uvs.ToArray()
            };
            
            _mesh.RecalculateNormals();
            _meshFilter.mesh = _mesh;
        }

        private static void AddTexture(Block type, int textureID, ref List<Vector2> uvs)
        {
            var textureSize = WorldGenerator.Instance.BlockOnAtlasSize;
            float y = textureID / WorldGenerator.Instance.AtlasSize;
            var x = textureID - (y * WorldGenerator.Instance.AtlasSize);

            x *= textureSize;
            y *= textureSize;

            y = 1f - y - textureSize;

            for (int i = 0; i < type.MeshData.Faces.Length; i++)
            {
                float xOffset = type.MeshData.Faces[i].UV.x.Equals(1) ? textureSize : 0;
                float yOffset = type.MeshData.Faces[i].UV.y.Equals(1) ? textureSize : 0;

                uvs.Add(new Vector2(x + xOffset, y + yOffset));
            }
        }
    }
}
