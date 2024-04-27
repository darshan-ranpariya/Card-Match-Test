﻿using UnityEngine;
 using UnityEngine.UI;
 using UnityEngine.EventSystems;
 using UnityEngine.Events;
 
 using System.Collections;
 
 // on select event class
 [System.Serializable]
public class OnSelectEvent : UnityEvent<BaseEventData> { }

// --
public class UIInputFieldCaretFix : MonoBehaviour, ISelectHandler
{
    private bool alreadyFixed;

    public float upFix = 7.0f;
    public float rightFix;

    public OnSelectEvent onSelectEvent;

    public void Start()
    {
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (onSelectEvent != null)
            onSelectEvent.Invoke(eventData);

        StartCoroutine(FixCaret());
    }

    IEnumerator FixCaret()
    {
        yield break;

        if (alreadyFixed)
        {
            //Debug.Log("fixed");
            yield break;
        }

        string caretName = gameObject.name + " Input Caret";
        RectTransform caretTransorm = null;
        do
        {
            caretTransorm = (RectTransform)transform.Find(caretName);
            //Debug.Log("getting transform");
            if (!caretTransorm)
                yield return null;
        } while (!caretTransorm);

        //Debug.Log("Fixing input field caret: " + caretName);

        Vector2 caretPosition = caretTransorm.anchoredPosition;
        caretPosition.x += rightFix;
        caretPosition.y += upFix;
        caretTransorm.anchoredPosition = caretPosition;
        caretTransorm.sizeDelta = new Vector2(1, 10);

        alreadyFixed = true;
    }
}