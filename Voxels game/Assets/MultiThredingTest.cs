using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MultiThredingTest : MonoBehaviour
{
    private void Start()
    {
        AsyncTest();
        Debug.Log("Start");
    }

    private async void AsyncTest()
    {
        StartCoroutine(AsyncCoroutine());
        int value = await TestTask();
        Debug.Log("Generation number: " + value);
        StopAllCoroutines();
        Debug.Log("Finished process");
    }

    private IEnumerator AsyncCoroutine()
    {
        yield return new WaitForSeconds(.5f);
        Debug.Log("Playing game " + Time.time);
        StartCoroutine(AsyncCoroutine());
    }

    private async Task<int> TestTask()
    {
        await Task.Delay(2000);
        return await Task.Run(() => 5);
    }
}
