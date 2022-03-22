using System;
using System.Collections.Generic;
using Management.VoxelManagement;
using Management.WorldManagement;
using NewMathf;
using Tools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Management.ChunkManagement
{
    public class Chunk : MonoBehaviour
    {
        private Coordinates _coordinates;
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
        private byte[,,] _voxelMap;

        private WorldGeneration _worldGeneration;
        private bool _isVoxelMapSetted = false;

        public bool IsVoxelMapSetted
        {
            get
            {
                return _isVoxelMapSetted;
            }
            set
            {
                _isVoxelMapSetted = value;
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
            // _voxelMap = new byte[WorldGeneration.Instance.ChunkSize.x, WorldGeneration.Instance.ChunkSize.y, WorldGeneration.Instance.ChunkSize.x];
            // _collider = gameObject.AddComponent<MeshCollider>();
            // _meshFilter = gameObject.AddComponent<MeshFilter>();
            // _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            // _worldGeneration = WorldGeneration.Instance;
            // _meshRenderer.material = _worldGeneration.WorldMaterial;

            
            SetVoxelMap();
            CreateChunk();
            SetCollisionActive(true);
        }

        // private IntVector size
        // {
        //     get
        //     {
        //         // return WorldGeneration.Instance.ChunkSize;
        //     }
        // }
        
        private void CreateChunk()
        {
            // for (int y = 0; y < WorldGeneration.Instance.ChunkSize.y; y++)
            // {
            //     for (int x = 0; x < WorldGeneration.Instance.ChunkSize.x; x++)
            //     {
            //         for (int z = 0; z < WorldGeneration.Instance.ChunkSize.x; z++)
            //         {
            //             if (!_worldGeneration.BlockTypes[_voxelMap[x, y, z]].IsSolid) 
            //                 continue;
            //
            //             AddVoxelData(new Vector3(x, y, z));
            //             CreateMesh();
            //
            //         }
            //     }
            // }

        }

        protected virtual void SetCollisionActive(bool active)
        {
            _collider.sharedMesh = active ? _meshFilter.mesh : null;
        }

        protected virtual void UpdateVoxelsInNearby(IntVector pos)
        {
            var voxelPos = pos.ToVector3();

            for (int i = 0; i < 6; i++)
            {
                var currentVoxel = new IntVector(voxelPos + VoxelData.FaceCheck[i]);
                    
                // if (!Math3D.IsInsideObject(currentVoxel, size)) {
                //
                //     // WorldGeneration.Instance.GetChunkFromVector3(currentVoxel.ToVector3() + position).UpdateChunk();
                //
                // }
            }
        }
        
        public VoxelCoord GetVoxel (Vector3 pos)
        {
            var checkVector = new IntVector(pos);
            var chunkPos = new IntVector(position);
            
            checkVector.x -= chunkPos.x;
            checkVector.z -= chunkPos.z;

            return new VoxelCoord(checkVector.x, checkVector.y, checkVector.z);
        }
        
        
        public void SetVoxel (VoxelCoord voxelCoord, byte id)
        {
            _voxelMap[voxelCoord.x, voxelCoord.y, voxelCoord.z] = id;
            UpdateChunk();
            UpdateVoxelsInNearby(voxelCoord.ToIntVector());
            SetCollisionActive(true);
        }

        void UpdateChunk () {

            ClearMesh();

            for (int y = 0; y < 2; y++) {
                for (int x = 0; x < 2; x++) {
                    for (int z = 0; z < 2; z++) {

                        if (true)
                            AddVoxelData(new Vector3(x, y, z));

                    }
                }
            }

            CreateMesh();

        }

        private void ClearMesh () 
        {
            _vertexIndex = 0;
            _vertices.Clear();
            _triangles.Clear();
            _uvs.Clear();

        }
        
        protected virtual void SetVoxelMap()
        {
            // for (int y = 0; y < size.y; y++)
            // {
            //     for (int x = 0; x < size.x; x++)
            //     {
            //         for (int z = 0; z < size.z; z++)
            //         {
            //             var voxelPosition = new Vector3(x, y, z) + position;
            //             var voxelIndex = _worldGeneration.GetVoxelByPosition(voxelPosition);
            //             _voxelMap[x, y, z] = voxelIndex;
            //         }
            //     }
            // }
        }

        public byte GetVoxelType(VoxelCoord coords)
        {
            return _voxelMap[coords.x, coords.y, coords.z];
        }

        protected virtual bool HasNeighbourVoxel(Vector3 pos)
        {
            var intPos = new IntVector(pos);
            
            // return Math3D.IsInsideObject(intPos, size) 
            //     ? _worldGeneration.BlockTypes[_worldGeneration.GetVoxelByPosition(pos + position)].IsSolid 
            //     : _worldGeneration.BlockTypes[_voxelMap[intPos.x, intPos.y, intPos.z]].IsSolid;

            return true;
        }

        protected virtual void AddVoxelData(Vector3 pos)
        {
            for (int i = 0; i < 6; i++)
            {
                if (HasNeighbourVoxel(pos + VoxelData.FaceCheck[i])) 
                    continue;

                var voxelPos = new IntVector(pos);

                var voxelID = _voxelMap[voxelPos.x, voxelPos.y, voxelPos.z];
                //
                // AddTexture(_worldGeneration.BlockTypes[voxelID].GetTextureIDFromSide(VoxelData.Sides[i]));
                
                for (int j = 0; j < 6; j++)
                {
                    var trisIndex = VoxelData.Triangles[i, j];
                    _vertices.Add(VoxelData.Vertices[trisIndex] + pos);
                    _triangles.Add(_vertexIndex);
                    _vertexIndex++;
                }
            }
        }

        private void CreateMesh()
        {
            var mesh = new Mesh
            {
                vertices = _vertices.ToArray(),
                triangles = _triangles.ToArray(),
                uv = _uvs.ToArray()
            };
            
            mesh.RecalculateNormals();
            _meshFilter.mesh = mesh;

        }



        private void AddTexture(int textureID)
        {
            // var textureSize = WorldGeneration.Instance.BlockOnAtlasSize;
            // float y = textureID / WorldGeneration.Instance.AtlasSize;
            // var x = textureID - (y * WorldGeneration.Instance.AtlasSize);
            //
            // x *= textureSize;
            // y *= textureSize;
            //
            // y = 1f - y - textureSize;
            //
            // for (int i = 0; i < VoxelData.UVs.Length; i++)
            // {
            //     var xOffset = VoxelData.UVs[i].x.Equals(1) ? textureSize : 0;
            //     var yOffset = VoxelData.UVs[i].y.Equals(1) ? textureSize : 0;
            //     
            //     _uvs.Add(new Vector2(x + xOffset, y + yOffset));
            // }
        }



    }

}
