using System;
using Assets.Scripts.Controllers.UI;
using Management._Cursor;
using Management.Game;
using Management.Save;
using Management.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Controllers.Pause
{
    public class PauseController : MonoBehaviour
    {
        
        
        [SerializeField] private UIController m_uiController;
        [Header("Buttons")] 
        [SerializeField] private Button m_resumeButton;
        [SerializeField] private Button m_saveButton; 
        [SerializeField] private Button m_backToMenuButton; 
        [SerializeField] private Button m_quitButton;

        private void Awake()
        {
            m_resumeButton.onClick.AddListener(ResumeButton);
            m_saveButton.onClick.AddListener(SaveGameButton);
            m_backToMenuButton.onClick.AddListener(BackToMenuButton);
            m_quitButton.onClick.AddListener(QuitButton);
        }

        public void ResumeButton()
        {
            m_uiController.SetScreen(ScreenType.PAUSE, false);
        }

        public void BackToMenuButton()
        {
            SceneManager.LoadScene("Menu");
        }

        public void QuitButton()
        {
            Application.Quit();
        }
        
        public void SaveGameButton()
        {
            SaveSystem.Instance.SaveGame();
            m_uiController.SetScreen(ScreenType.PAUSE, false);
            UIManager.Instance.TextAfterSaving.WriteText("Game has been saved!");
        }
    }
}
