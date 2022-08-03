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
                return _position.x + "-" + _position.z;
            }
        }

        private Dictionary<string, byte> _modifiedVoxels = new Dictionary<string, byte>();

        public Dictionary<string, byte> ModifiedVoxels
        {
            get
            {
                return _modifiedVoxels;
            }
        }

        private IntVector _position;

        private bool _isMapPopulated;

        public bool IsMapPopulated
        {
            get
            {
                return _isMapPopulated;
            }

            set
            {
                _isMapPopulated = value;
            }
        }
        
        public ChunkData(Vector3 pos)
        {
            _position = new IntVector(pos);
        }

        public void LoadData(Dictionary<string, byte> voxels)
        {
            // Debug.Log("Try to load: " + Name + " modified voxels count: " + voxels.Count.ToString());
            _modifiedVoxels = voxels;
        }

        public void ModifyVoxel(IntVector coord, byte id)
        {
            if(_modifiedVoxels.ContainsKey(coord.ToString()))
                _modifiedVoxels.Remove(coord.ToString());
            
            _modifiedVoxels.Add(coord.ToString(), id);
        }
    }
}
