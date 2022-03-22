using System;
using Management.Loading;
using Management.UI;
using UI.Text;
using UI.Type;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Controllers.Menu
{
    public class MenuController : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button _newWorldButton;

        private void Init()
        {
            _newWorldButton.onClick.AddListener(SetGenerationWorldButton);
            LoadingManager.Instance.OnProgress += OnLoadingpProgress;
        }

        private void Awake()
        {
            Init();
        }

        private void SetGenerationWorldButton()
        {
            UIManager.Instance.SetScreen(UIType.LOADING);
            LoadingManager.Instance.LoadScene("Test");
        }

        private void OnLoadingpProgress(int percent)
        {
            UIManager.Instance.SetText(TextType.LOADING_COUNTER, percent + "%");
        }
    }
}
