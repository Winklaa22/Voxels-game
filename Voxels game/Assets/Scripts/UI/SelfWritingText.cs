using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class SelfWritingText : MonoBehaviour
{
    private TMP_Text _text;
    private CanvasGroup _canvasGroup;
    [SerializeField] private float m_spaceBetween;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void WriteText(string text)
    {
        StartCoroutine(StartWrite(text));
    }

    private IEnumerator StartWrite(string txt)
    {
        _canvasGroup.DOFade(1, .3f);
        
        _text.text = "";

        for (int i = 0; i < txt.Length; i++)
        {
            _text.text += txt[i];
            yield return new WaitForSeconds(m_spaceBetween);
        }
        
        _canvasGroup.DOFade(0, .3f);
    }
}
