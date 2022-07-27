using System;
using System.Collections;
using UnityEngine;

namespace Time_Events.Destroy
{
    public class DestroyAfterTime : MonoBehaviour
    {
        [SerializeField] private float _time;

        private void Start()
        {
            StartCoroutine(StartCountingDown());
        }

        private IEnumerator StartCountingDown()
        {
            yield return new WaitForSeconds(_time);
            
            Destroy(gameObject);
        }
    }
}
