using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Management.WorldManagement;
using UnityEngine;

namespace Management.Save
{
    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem Instance;
        
        [Header("Game save data")] 
        [SerializeField] private string _gameDataFileName;
        [SerializeField] private string _fileType;
        
        private string saveDirectiory => $"{Application.persistentDataPath}/{WorldGenerator.Instance.WorldName} /Saves/";


        private void Awake()
        {
            Instance = this;
        }

        private string GetPath(string fileName)
        {
            return saveDirectiory + fileName + "." + _fileType;
        }

        public void SaveGame()
        {
            var state = LoadFile();
            CaptureState(state);
            SaveFile(state);
        }

        private void LoadGame()
        {
            var state = LoadFile();
            RestoreState(state);
        }

        private void Start()
        {
            Debug.Log(saveDirectiory);
            LoadGame();
        }

        public void SaveFile(object state)
        {
            if (!Directory.Exists(saveDirectiory))
                Directory.CreateDirectory(saveDirectiory);
            
            using(var stream = File.Open(GetPath(_gameDataFileName), FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }

        public Dictionary<string, object> LoadFile()
        {
            if(!Directory.Exists(saveDirectiory) || !File.Exists(GetPath(_gameDataFileName)))
                return new Dictionary<string, object>();
            
            using (var stream = File.Open(GetPath(_gameDataFileName), FileMode.Open))
            {
                var formatter = new BinaryFormatter();
                return (Dictionary<string, object>) formatter.Deserialize(stream);
            }
        }

        private void CaptureState(Dictionary<string, object> state)
        {
            foreach (var saveable in FindObjectsOfType<SaveableEntity>())
            {
                state[saveable.ID] = saveable.CaptureState();
            }
        }

        private void RestoreState(Dictionary<string, object> state)
        {
            foreach (var saveable in FindObjectsOfType<SaveableEntity>())
            {
                if (state.TryGetValue(saveable.ID, out var value))
                {
                    saveable.RestoreState(value);
                }
            }
        }
        
        
    }
}
