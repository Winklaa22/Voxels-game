using System;
using UI.Type;
using UnityEngine;

namespace UI.Screen
{
    [Serializable]
    public class UIScreen
    {
        [SerializeField] private string _name;
        public UIType Type;
        public GameObject ScreenObject;

    }
}
