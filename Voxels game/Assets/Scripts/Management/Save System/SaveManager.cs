using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using _3D.Mathf2;
using Chunks;
using JetBrains.Annotations;
using Management.WorldManagement;
using UnityEngine;

namespace Management.Save_System
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance;
        private string _dataDirectory => $"{Application.persistentDataPath}/Saves/";
        private string _dataPath => _dataDirectory + "save.data";

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Debug.Log(Application.persistentDataPath);
            LoadGame();
        }

        [ContextMenu("Save")]
        public void SaveGame()
        {
            SaveChunks();
            var state = LoadFile();
            CaptureState(state);
            SaveFile(state);
            
        }

        [ContextMenu("Load")]
        public void LoadGame()
        {
            var state = LoadFile();
            RestoreChunksState(state, WorldGenerator.Instance.ModifiedChuks);
            RestoreState(state);
        }

        private void SaveFile(object state)
        {
            if (!Directory.Exists(_dataDirectory))
                Directory.CreateDirectory(_dataDirectory);
            
            using (var stream = File.Open(_dataPath, FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }

        public Dictionary<string, object> LoadFile()
        {
            if (!File.Exists(_dataPath))
            {
                return new Dictionary<string, object>();
            }

            using (var stream = File.Open(_dataPath, FileMode.Open))
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

        private void SaveChunks ()
        {
            var world = WorldGenerator.Instance;
            
            var chunks = new List<ChunkData>(world.ModifiedChuks);

            int count = 0;
            foreach (var chunk in chunks) 
            {
                Debug.Log(count);
                SaveChunk(chunk, world.WorldName);
                CaptureChunkState(LoadFile(), chunk);
                count++;
                Debug.Log(count);
            }
        }

        private string GetChunkDirectory(string worldName)
        {
            return Application.persistentDataPath + "/Saves/" + worldName + "/";
        }

        private void SaveChunk (ChunkData chunk, string worldName) {
            
            var chunkName = chunk.Position.x + "-" + chunk.Position.z;
            Debug.Log(chunkName);

            var chunksDirectory = GetChunkDirectory(worldName);

            if (!Directory.Exists(chunksDirectory))
                Directory.CreateDirectory(chunksDirectory);

            using (var stream = File.Open(chunksDirectory + chunkName + ".chunkData", FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, chunk.CaptureState());
            }
        }

        private void CaptureChunkState(Dictionary<string, object> state, ChunkData chunk)
        {
            if (state == null) 
                return;
            
            state[chunk.Name] = chunk.CaptureState(); 
        }

        private void RestoreChunksState([NotNull] IReadOnlyDictionary<string, object> state, List<ChunkData> chunks)
        {
            foreach (var chunk in chunks)
            {
                if (state.TryGetValue(chunk.Name, out var value))
                {
                    chunk.RestoreState(value);
                }
            }
        }
        
        public Dictionary<string, object> LoadChunk (string worldName, IntVector position) {

            var chunkName = position.x + "-" + position.z;

            var loadPath =  Application.persistentDataPath + "/Saves/" + worldName + "/" + chunkName + ".chunk";

            if (!File.Exists(loadPath))
            {
                return new Dictionary<string, object>();
            }

            using (var stream = File.Open(loadPath, FileMode.Open))
            {
                var formatter = new BinaryFormatter();
                return (Dictionary<string, object>) formatter.Deserialize(stream);
            }
        }

        private void RestoreState(IReadOnlyDictionary<string, object> state)
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
