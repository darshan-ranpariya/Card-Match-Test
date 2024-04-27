﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFillTween : AnimationBase
{  
    public Image target;
    public Image targetImage
    {
        get
        {
            if (target == null)
            {
                target = GetComponent<Image>();
            }
            return target;
        }
    } 

    public AnimationCurveAsset curve; 
    public float startFill=0, endFill=1; 
    public float initialDleay = 0;
    public float duration = 1;
    float timeElapsed;

    void OnEnable()
    {
        if (startOnEnable) StartTween();
    }

    void OnDisable()
    {
        EndTween();
    }

    public void StartTween()
    {
        StartCoroutine("Tween");
    }

    public void EndTween()
    {
        StopCoroutine("Tween");
    }

    public void ResetTween()
    {
        EndTween();
        timeElapsed = 0;
        ApplyVal();
    }

    IEnumerator Tween()
    {
        onStart.Invoke();

        timeElapsed = 0; 
        ApplyVal();
        if (initialDleay > 0) yield return new WaitForSeconds(initialDleay);
        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            ApplyVal();
            yield return null;
        }

        timeElapsed = duration; 
        ApplyVal();
        onEnd.Invoke();
        if (loop) StartCoroutine("Tween");
    }

    void ApplyVal()
    { 
        target.fillAmount = Mathf.LerpUnclamped(startFill, endFill, curve.Evaluate(timeElapsed / duration));
    }

    #region AnimationBase implementation 
    public override float Duration
    {
        get
        {
            return duration;
        }

        set
        {
            duration = value;
        }
    }

    public override void StartAnim()
    {
        StartTween();
    }
    public override void StopAnim()
    {
        EndTween();
    }
    public override void ResetAnim()
    {
        ResetTween();
    }
    #endregion
}
