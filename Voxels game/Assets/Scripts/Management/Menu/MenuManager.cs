using Management.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Management.Menu
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private Button 
    
        public void OnCreateWorldButton()
        {
            SceneManager.LoadScene("Test");
        }

        public void OnSettingsWorldButton()
        {
            UIManager.Instance.SetScreen(ScreenType.SETTINGS);
        }
    }
}
