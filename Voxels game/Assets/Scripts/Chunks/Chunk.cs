using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using _3D.Mathf2;
using Blocks;
using Blocks.Textures;
using Blocks.Type;
using Management.ChunkManagement;
using Management.VoxelManagement;
using Management.WorldManagement;
using UnityEngine;

namespace Chunks
{
    public class Chunk : MonoBehaviour
    {
        public string ID { get; private set; }

        private ChunkCoord _coordinates;
        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;
        private MeshCollider _collider;
        private int _texture;
        private int _vertexIndex = 0;

        [Header("Transform")] 
        private Vector3 _position;

        [Header("Mesh")]
        [SerializeField] private List<Vector3> _vertices = new List<Vector3>();
        [SerializeField] private List<int> _triangles = new List<int>();
        private Mesh _mesh;
        private List<Vector2> _uvs = new List<Vector2>();

        [Header("Threading")] 
        [SerializeField] private bool _threadingIsLocked;
        private ChunkData _data;

        public ChunkData Data
        {
            get
            {
                return _data;
            }
        }
        private WorldGenerator _worldGenerator;
        private bool _canBeGenerate;

        public Vector3 Position
        {
            get
            {
                return _position;
            }
        }
        
        public bool CanBeGenerate
        {
            set
            {
                _canBeGenerate = value;
            }
        }

        private IntVector _size
        {
            get
            {
                return WorldGenerator.Instance.ChunkSize;
            }
        }

        public bool CanModify
        {
            get
            {
                return !_threadingIsLocked && _data.IsMapPopulated;
            }
        }

        public bool IsActive
        {
            get { return gameObject.activeSelf; }

            set { gameObject.SetActive(value); }
        }

        private void Awake()
        {
            ID = new IntVector(transform.position).ToString();
            _data = new ChunkData(this);
            _collider = gameObject.AddComponent<MeshCollider>();
            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            _worldGenerator = WorldGenerator.Instance;
            _meshRenderer.material = _worldGenerator.WorldMaterial;
            _position = transform.position;
        }

        private void Start()
        {
            StartCoroutine(TryToCreate());
        }

        public void SetActive(bool active)
        {
            _collider.enabled = active;
        }

        private IEnumerator TryToCreate()
        {
            if (!_canBeGenerate) 
                yield break;
            
            Initialize();
            yield return null;
        }

        private void Initialize()
        {
            _data.PopulateMap();
            CreateChunk();
        }

        private void CreateChunk()
        {
            var thread = new Thread(() => ThreadingCreateChunk());
            thread.Start();
        }
        
        private void ThreadingCreateChunk()
        {
            _threadingIsLocked = true;
            
            for (int y = 0; y < WorldGenerator.Instance.ChunkSize.y; y++)
            {
                for (int x = 0; x < WorldGenerator.Instance.ChunkSize.x; x++)
                {
                    for (int z = 0; z < WorldGenerator.Instance.ChunkSize.z; z++)
                    {
                        if (!_worldGenerator.BlockTypes[_data.Map[x, y, z]].IsSolid)
                            continue;
                        
                        UpdateMeshData(new Vector3(x, y, z), true);
                    }
                }
            }

            lock (_worldGenerator.ChunksToRender)
            {
                _worldGenerator.ChunksToRender.Enqueue(this);
            }

            _threadingIsLocked = false;
        }
        

        public byte GetVoxelID(Vector3 pos)
        {
            var coords = GetVoxel(pos);
            return _data.Map[coords.x, coords.y, coords.z];
        }

        public IntVector GetVoxel(Vector3 pos)
        {
            var checkVector = new IntVector(pos);
            var chunkPos = new IntVector(_position);

            checkVector.x -= chunkPos.x;
            checkVector.z -= chunkPos.z;
            checkVector.y = checkVector.y < 0 ? 0 : checkVector.y;
            return new IntVector(checkVector.x, checkVector.y, checkVector.z);
        }


        public void SetVoxel(IntVector voxelCoord, byte id)
        {
            if (!Math3D.IsInsideTheObject(voxelCoord, WorldGenerator.Instance.ChunkSize))
            {
                _data.Map[voxelCoord.x, voxelCoord.y, voxelCoord.z] = id;
                UpdateChunk();
                UpdateNearestVoxel(voxelCoord);
                
                _data.ModifyVoxel(voxelCoord.ToVector3(), id);
                
                if(!_worldGenerator.ModifiedChuks.Contains(_data))
                    _worldGenerator.ModifiedChuks.Add(_data);
            }
            else
            {
                var chunk = WorldGenerator.Instance.GetChunkFromVector3(voxelCoord.ToVector3() + _position);
                var voxelPosition = chunk.GetVoxel(voxelCoord.ToVector3() + _position);
                chunk.SetVoxel(voxelPosition , id);
            }
        }

        private void UpdateNearestVoxel(IntVector coord)
        {
            for (int i = 0; i < 6; i++)
            {
                var nearestVoxelPosition = new IntVector(coord.ToVector3() + VoxelProperties.FaceCheck[i]);
                if (!Math3D.IsInsideTheObject(nearestVoxelPosition, WorldGenerator.Instance.ChunkSize))
                    continue;
                
                var chunk = WorldGenerator.Instance.GetChunkFromVector3(nearestVoxelPosition.ToVector3() + _position);
                chunk.UpdateChunk();
            }
        }

        private void UpdateChunk()
        {
            ClearMesh();

            for (int y = 0; y < WorldGenerator.Instance.ChunkSize.y; y++)
            {
                for (int x = 0; x < WorldGenerator.Instance.ChunkSize.x; x++)
                {
                    for (int z = 0; z < WorldGenerator.Instance.ChunkSize.z; z++)
                    {
                        if (!WorldGenerator.Instance.BlockTypes[_data.Map[x, y, z]].IsSolid)
                            continue;

                        UpdateMeshData(new Vector3(x, y, z), false);
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
        }

        public byte GetVoxelType(IntVector coords)
        {
            return _data.Map[coords.x, coords.y, coords.z];
        }

        bool IsVoxelInChunk(int x, int y, int z)
        {
            return !(x < 0 || x > _worldGenerator.ChunkSize.x - 1 || y < 0 || y > _worldGenerator.ChunkSize.y - 1 ||
                     z < 0 || z > _worldGenerator.ChunkSize.x - 1);
        }

        private bool CanRenderSide(Vector3 pos, bool firstGenerate)
        {
            var intPos = new IntVector(pos);
            var blocks = _worldGenerator.BlockTypes;

            if (!IsVoxelInChunk(intPos.x, intPos.y, intPos.z))
            {
                return !firstGenerate
                    ? _worldGenerator.IsVoxelExist(pos + _position)
                    : _worldGenerator.CheckForVoxel(pos + _position);
            }

            return blocks[_data.Map[intPos.x, intPos.y, intPos.z]].IsSolid && !blocks[_data.Map[intPos.x, intPos.y, intPos.z]].IsTransparent;
        }

        private void UpdateMeshData(Vector3 pos, bool firstGenerate)
        {
            var voxelPos = new IntVector(pos);
            var id = _data.Map[voxelPos.x, voxelPos.y, voxelPos.z];
            var type = _worldGenerator.BlockTypes[id];

            for (int i = 0; i < type.MeshData.Faces.Length; i++)
            {
                if (CanRenderSide(pos + type.MeshData.Faces[i].Value, firstGenerate) )
                    continue;
                
                AddTexture(type, type.GetTextureIDFromSide(VoxelProperties.Sides[i]), ref _uvs);
                
                CreateFaceData(type, pos, i);
            }
        }
        
        

        private void CreateFaceData(Block type, Vector3 pos, int face)
        {
            for (int j = 0; j < type.MeshData.Faces.Length; j++)
            {
                var trisIndex = type.MeshData.Faces[face].Triangles[j];
                _vertices.Add(type.MeshData.Vertices[trisIndex] + pos);
                _triangles.Add(_vertexIndex);
                _vertexIndex++;
            }
        }

        public void CreateMesh()
        {
            _mesh = new Mesh
            {
                vertices = _vertices.ToArray(),
                triangles = _triangles.ToArray(),
                uv = _uvs.ToArray()
            };
            
            _mesh.RecalculateNormals();
            
            _meshFilter.mesh = _mesh;
            _collider.sharedMesh = _meshFilter.mesh;
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
                var xOffset = type.MeshData.Faces[i].UV.x.Equals(1) ? textureSize : 0;
                var yOffset = type.MeshData.Faces[i].UV.y.Equals(1) ? textureSize : 0;

                uvs.Add(new Vector2(x + xOffset, y + yOffset));
            }
        }
    }
}
