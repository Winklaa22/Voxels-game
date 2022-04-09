using UnityEngine;

namespace Management.UI
{
    [System.Serializable]
    public class ScreenUI
    {
        public ScreenType Type;
        public GameObject Object;

        public void SetActive(bool active)
        {
            Object.SetActive(active);
        }
    }
}