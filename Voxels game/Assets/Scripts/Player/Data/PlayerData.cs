using Management.Save;
using UnityEngine;

namespace Player.Data
{
    public class PlayerData : MonoBehaviour, ISaveable
    {
        [SerializeField] private string _position;

        public object CatureState()
        {
            return new SaveData
            {
                Position = _position
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData) state;
            _position = saveData.Position;
        }
        
        
        [System.Serializable]
        public struct SaveData
        {
            public string Position;
        }
    }
}
