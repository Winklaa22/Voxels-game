using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Management.Loading
{
    public class LoadingManager : MonoBehaviour
    {
        public static LoadingManager Instance;
        
        public delegate void Progess(int percent);
        public event Progess OnProgress;

        private void Awake()
        {
            Instance = this;
        }


        public void LoadScene(string name)
        {
            StartCoroutine(LoadingScene(name));
        }

        private IEnumerator LoadingScene(string name)
        {
            var operation = SceneManager.LoadSceneAsync(name);

            while (!operation.isDone)
            {
                OnProgress?.Invoke((int)(operation.progress*100));
                yield return null;
            }
        }
        
        
    }
}