using System;
using TMPro;
using UI.Screen;
using UI.Text;
using UI.Type;
using UnityEngine;

namespace Management.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        
        [Header("UI Screens")] 
        [SerializeField] private UIScreen[] _screens;

        [Header("UI Texts")] 
        [SerializeField] private UIText[] _texts;


        private void Awake()
        {
            Instance = this;
        }

        public void SetScreen(UIType type)
        {
            foreach (var screen in _screens)
            {
                var active = screen.Type.Equals(type);
                screen.ScreenObject.SetActive(active);
            }
        }

        public void SetText(TextType type, string content)
        {
            foreach (var uiText in _texts)
            {
                if(!uiText.Type.Equals(type))
                    continue;

                uiText.text = content;
            }
        }
        
        
        
        
    }
}
