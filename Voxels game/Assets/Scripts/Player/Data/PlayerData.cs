using System;
using _3D.Mathf2;
using Controllers.Player;
using Management.Save;
using Management.WorldManagement;
using UnityEngine;

namespace Player.Data
{
    public class PlayerData : MonoBehaviour, ISaveable
    {
        private PlayerController _player;
        
        private Vector3 _savedPosition;
        
        private void Awake()
        {
            _player = GetComponent<PlayerController>();
        }

        private void Start()
        {
            Spawn();
        }

        public object CaptureState()
        {
            return new SaveData
            {
                Position = new IntVector(transform.position)
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData) state;
            _savedPosition = saveData.Position.ToVector3();
        }
        
        private void Spawn()
        {
            var spawnPos = _savedPosition.Equals(Vector3.zero) ? WorldGenerator.Instance.GetSpawnPosition() : _savedPosition + Vector3.one;
            WorldGenerator.Instance.SetPlayersCoord(spawnPos);
            _player.Spawn();
        }

        [System.Serializable]
        public struct SaveData
        {
            public IntVector Position;
        }
    }
}
