using System;
using DG.Tweening;
using Management.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Management.Menu
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private CanvasGroup m_buttons;

        [Header("Buttons")] 
        [SerializeField] private Button m_playButton;
        [SerializeField] private Button m_quitButton;

        private void Awake()
        {
            m_playButton.onClick.AddListener(PlayButton);
            m_quitButton.onClick.AddListener(QuitButton);
        }

        private void Start()
        {
            m_buttons.DOFade(1, 0.85f).SetEase(Ease.InExpo);
        }

        public void PlayButton()
        {
            SceneManager.LoadScene("Test");
        }

        public void QuitButton()
        {
            Application.Quit();
        }
    }
}
