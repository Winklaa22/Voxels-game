using System.Collections.Generic;
using System.Linq;
using _3D.Mathf2;
using Management.Save;
using Management.WorldManagement;
using UnityEngine;

namespace Chunks
{
    [System.Serializable]
    public class ChunkData
    {
        public string Name
        {
            get
            {
                return Position.x + "-" + Position.z;
            }
        }

        private Dictionary<Vector3, byte> _modifiedVoxels = new Dictionary<Vector3, byte>();


        private byte[,,] _map;
        public byte[,,] Map
        {
            get
            {
                return _map;
            }
        }

        public Vector3 Position
        {
            get
            {
                return _parent.Position;
            }
        }
        
        
        private bool isMapPopulated;

        public bool IsMapPopulated
        {
            get { return isMapPopulated; }
        }

        
        private IntVector _size;
        private Chunk _parent;


        public ChunkData(Chunk parent)
        {
            _parent = parent;
            Init();
        }
        
        private void Init()
        {
            _size = WorldGenerator.Instance.ChunkSize;
            _map = new byte[_size.x, _size.y, _size.z];
        }

        public void LoadData()
        {
            Debug.Log("Try to load: " + Name);
            // var save = SaveManager.Instance.LoadChunk(WorldGenerator.Instance.WorldName, new IntVector(Position));
            // Debug.Log(save.Last());
        }

        public void ModifyVoxel(Vector3 coord, byte id)
        {
            if(_modifiedVoxels.ContainsKey(coord))
                _modifiedVoxels.Remove(coord);
            
            _modifiedVoxels.Add(coord, id);
        }

        public void PopulateMap()
        {
            for (int y = 0; y < _size.y; y++)
            {
                for (int x = 0; x < _size.x; x++)
                {
                    for (int z = 0; z < _size.z; z++)
                    {
                        if (_modifiedVoxels.ContainsKey(new Vector3(x, y, z)))
                        {
                            _map[x, y, z] = _modifiedVoxels[new Vector3(x, y, z)];
                            return;
                        }
                        
                        var voxelPosition = new Vector3(x, y, z) + _parent.Position;
                        var voxelIndex = WorldGenerator.Instance.GetVoxelByPosition(voxelPosition);

                        _map[x, y, z] = voxelIndex;
                    }
                }
            }

            isMapPopulated = true;
        }

        public object CaptureState()
        {
            return new SaveData
            {
                ModifiedVoxels = _modifiedVoxels
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData) state;
            _modifiedVoxels = saveData.ModifiedVoxels;
        }
        
        [System.Serializable]
        private struct SaveData
        {
            public Dictionary<Vector3, byte> ModifiedVoxels;
        }
    }
}
