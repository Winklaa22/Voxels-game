using System;
using System.Collections.Generic;
using UnityEngine;

namespace Management.Save
{
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] private string _id;

        public string ID => _id;

        [ContextMenu("Generate id")]
        private void GenerateID()
        {
            _id = Guid.NewGuid().ToString();
        }

        public object CaptureState()
        {
            var state = new Dictionary<string, object>();

            foreach (var saveable in GetComponents<ISaveable>())
            {
                state[saveable.GetType().ToString()] = saveable.CatureState();
            }

            return state;
        }

        public void RestoreState(object state)
        {
            var stateDictionary = (Dictionary<string, object>) state;

            foreach (var saveable in GetComponents<ISaveable>())
            {
                var typeName = saveable.GetType().ToString();

                if (stateDictionary.TryGetValue(typeName, out var value))
                {
                    saveable.RestoreState(value);
                }
            }
            
        }
        
    }
}
