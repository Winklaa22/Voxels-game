using System;
using TMPro;
using UnityEngine;

namespace UI.Text
{
    [Serializable]
    public class UIText
    {
        [SerializeField] private string _name;
        [SerializeField] private TextMeshProUGUI _textMeshPro;
        public TextType Type;

        public string text
        {
            set
            {
                _textMeshPro.text = value;
            }
        }
    }
}
