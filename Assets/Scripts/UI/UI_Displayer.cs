using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A controller for UI display.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class UI_Displayer : MonoBehaviour
{
    public delegate void AfterShow();
    public delegate void AfterHide();
    public AfterShow afterShow;
    public AfterHide afterHide;
    
    CanvasGroup cg;
    public AnimationCurve showCurver, hideCurve;
    float animationSpeed = 1;
    
    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        afterShow = Nothing;
        afterHide = Nothing;

    }

    public void Blink()
    {
        StopAllCoroutines();
        ShowPanel();
    }

    public void ShowPanel()
    {
        StartCoroutine(ShowPanelCor());
    }

    public void HidePanel()
    {
        StartCoroutine(HidePanelCor());
    }

    IEnumerator ShowPanelCor()
    {
        float timer = 0;
        while (cg.alpha<1)
        {
            cg.alpha = showCurver.Evaluate(timer);
            timer += Time.deltaTime * animationSpeed;
            yield return null;
        }
        cg.blocksRaycasts = true;
        cg.interactable = true;
        afterShow();
    }

    IEnumerator HidePanelCor()
    {
        float timer = 0;
        while (cg.alpha > 0)
        {
            cg.alpha = hideCurve.Evaluate(timer);
            timer += Time.deltaTime * animationSpeed;
            yield return null;
        }
        cg.blocksRaycasts = false;
        cg.interactable = false;
        afterHide();
    }

    void Nothing() { }
}
