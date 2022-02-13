using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance;

    private void Awake()
    {
        if (Instance is null)
            Instance = FindObjectOfType<T>();
        else
            Destroy(Instance);
    }
}
