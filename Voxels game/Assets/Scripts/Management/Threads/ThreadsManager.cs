using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ThreadsManager : MonoBehaviour
{
    [SerializeField] private Transform Cube;
    
    private Thread[] _threads = new Thread[3];
    
    // Start is called before the first frame update
    void Start()
    {
        // MainThreadMethod();
        // SecondThreadMethod();
        // ThirdThreadMethod();
        
        _threads[0] = new Thread(() => MainThreadMethod());
        _threads[1] = new Thread(() => SecondThreadMethod());
        _threads[2] = new Thread(() => ThirdThreadMethod());

        
        
        

        for (var i = 0; i < _threads.Length; i++)
        {
            _threads[i].Name = i > 0 ? "Thread" + i : "Main thread"; 
            Debug.Log(_threads[i].Name);
            _threads[i].Start();
        }
        
        _threads[1].IsBackground = true;
    }

    // Update is called once per frame
    void Update()
    {
        Cube.Rotate(new Vector3(15, 0, 0), 1f);
    }

    private void MainThreadMethod()
    {
        Debug.Log("Main thread");
    }
    
    private void SecondThreadMethod()
    {
        for (var i = 0; i < 99999; i++)
        {
            
        }
    }
    
    private void ThirdThreadMethod()
    {
        Debug.Log("Third thread");
    }
}
