using System;
using Blocks.Type;
using UnityEngine;

namespace Management.WorldManagement
{
    public class World : MonoBehaviour
    {
        public static World Instance;
        
        [SerializeField] private Material _material;
        [SerializeField] private BlockType[] _blockTypes;

        public BlockType[] BlockTypes
        {
            get
            {
                return _blockTypes;
            }
        }

        private void Awake()
        {
            Instance = this;
        }
    }
}
