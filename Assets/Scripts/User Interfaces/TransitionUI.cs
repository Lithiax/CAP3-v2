using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;



public class TransitionUI : MonoBehaviour
{

    public delegate void FadeTransition(float p_opacity,
                                        bool aboveLayer = false,
                                        bool p_isActiveOnEnd = true);
    public delegate void FadeInAndOutTransition(float p_preOpacity,
                                            float p_preTransitionTime,
                                            float p_delayTime,
                                            float p_postOpacity,
                                            float p_postTransitionTime,
                                            bool aboveLayer = false,
                                            Action p_preAction = null,
                                            Action p_postAction = null);
    public static TransitionUI instance;
    public static FadeInAndOutTransition onFadeInAndOutTransition;
    [SerializeField] public Image transitionUI;
    private Canvas transitionUICanvas;
    [SerializeField] private int defaultLayer;
    [SerializeField] private int targetLayer;
    public IEnumerator runningCoroutine;
    public static FadeTransition onFadeTransition;
    public TMP_Text tester;
    public Color color;

    private void Awake()
    {
        instance = this;
        transitionUICanvas = GetComponent<Canvas>();
        onFadeTransition =TransitionFade;
        onFadeInAndOutTransition=TransitionPreFadeAndPostFade;
    }


    public void TransitionFade(float p_opacity, bool aboveLayer = true, bool p_isActiveOnEnd = true)
    {
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }
        if (aboveLayer)
        {
            transitionUICanvas.sortingOrder = defaultLayer;
        }
        else
        {
            //transitionUICanvas.sortingOrder = targetLayer;

        }
        runningCoroutine = Co_TransitionFade(p_opacity, p_isActiveOnEnd);
        StartCoroutine(runningCoroutine);
    }
    public IEnumerator Co_TransitionFade(float p_opacity, bool p_isActiveOnEnd = true)

    {
        transitionUI.raycastTarget = (true);

        Sequence fadeSequence = DOTween.Sequence()
        .Append(transitionUI.DOFade(p_opacity, 0.5f));
        fadeSequence.Play();

        yield return fadeSequence.WaitForCompletion();
        transitionUI.raycastTarget = (p_isActiveOnEnd);
    }
    public void TransitionPreFadeAndPostFade(float p_preOpacity,
                                            float p_preTransitionTime,
                                            float p_delayTime,
                                            float p_postOpacity,
                                            float p_postTransitionTime,
                                            bool aboveLayer = true,
                                            Action p_preAction = null,
                                            Action p_postAction = null)
    {
        if (aboveLayer)
        {
            transitionUICanvas.sortingOrder = defaultLayer;
        }
        else
        {
            transitionUICanvas.sortingOrder = targetLayer;

        }
        transitionUI.color = new Color(color.r, color.g, color.b, transitionUI.color.a);
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }
        runningCoroutine = Co_TransitionPreFadeAndPostFade(p_preOpacity,
                                                            p_preTransitionTime,
                                                            p_delayTime,
                                                            p_postOpacity,
                                                            p_postTransitionTime,
                                                            p_preAction,
                                                            p_postAction);
        StartCoroutine(runningCoroutine);
 
    }

    public IEnumerator Co_TransitionPreFadeAndPostFade(float p_preOpacity,
                            float p_preTransitionTime,
                            float p_delayTime,
                            float p_postOpacity,
                            float p_postTransitionTime,
                            Action p_preAction = null,
                            Action p_postAction = null)
    {
     //   Debug.Log("transition done");
        Sequence preSequence = DOTween.Sequence()

        .Append(transitionUI.DOFade(p_preOpacity, p_preTransitionTime));

        yield return preSequence.WaitForCompletion();
      //  Debug.Log("FADINGl: " + p_preOpacity);
        p_preAction?.Invoke();
        yield return new WaitForSeconds(p_delayTime);
        Sequence postSequence = DOTween.Sequence()
        .Append(transitionUI.DOFade(p_postOpacity, p_postTransitionTime));
        yield return postSequence.WaitForCompletion();
        p_postAction?.Invoke();
    }
}
