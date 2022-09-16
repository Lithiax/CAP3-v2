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
    //public delegate Task FadeTransition(float p_opacity, 
    //                                    bool p_isActiveOnEnd = true);
    public delegate void FadeTransition(float p_opacity,
                                        bool p_isActiveOnEnd = true);
    public delegate void FadeInAndOutTransition(float p_preOpacity,
                                            float p_preTransitionTime,
                                            float p_delayTime,
                                            float p_postOpacity,
                                            float p_postTransitionTime,
                                            Action p_preAction = null,
                                            Action p_postAction = null);
    public static FadeInAndOutTransition onFadeInAndOutTransition;
    [SerializeField] public Image transitionUI;
    public IEnumerator runningCoroutine;
    public static FadeTransition onFadeTransition;
    //public static FadeInAndOutTransition onFadeInAndOutTransition = new FadeInAndOutTransition();
    private void Awake()
    {
        onFadeTransition=TransitionFade;
        onFadeInAndOutTransition=TransitionPreFadeAndPostFade;
    }
    //public async Task TransitionFade(float p_opacity, bool p_isActiveOnEnd = true)
    //{
    //    //if (runningCoroutine != null)
    //    //{
    //    //    StopCoroutine(runningCoroutine);
    //    //    runningCoroutine = null;
    //    //}
    //    //await Task.Yield();
    //    transitionUI.raycastTarget = (true);

    //    Sequence fadeSequence = DOTween.Sequence();
    //    fadeSequence.Join(transitionUI.DOFade(p_opacity, 0.5f));
    //    fadeSequence.Play();
    //    await fadeSequence.AsyncWaitForCompletion();
    //    //yield return fadeSequence.WaitForCompletion();
    //    transitionUI.raycastTarget = (p_isActiveOnEnd);
   
    //    //runningCoroutine = Co_TransitionFade(p_opacity, p_isActiveOnEnd);
    //    //StartCoroutine(runningCoroutine);
    //}

    public void TransitionFade(float p_opacity, bool p_isActiveOnEnd = true)
    {
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }
        

        runningCoroutine = Co_TransitionFade(p_opacity, p_isActiveOnEnd);
        StartCoroutine(runningCoroutine);
    }
    public IEnumerator Co_TransitionFade(float p_opacity, bool p_isActiveOnEnd = true)

    {
        //await Task.Yield();
        transitionUI.raycastTarget = (true);

        Sequence fadeSequence = DOTween.Sequence();
        fadeSequence.Join(transitionUI.DOFade(p_opacity, 0.5f));
        fadeSequence.Play();
        //await fadeSequence.AsyncWaitForCompletion();
        yield return fadeSequence.WaitForCompletion();
        transitionUI.raycastTarget = (p_isActiveOnEnd);
    }
    public void TransitionPreFadeAndPostFade(float p_preOpacity,
                                            float p_preTransitionTime,
                                            float p_delayTime,
                                            float p_postOpacity,
                                            float p_postTransitionTime,
                                            Action p_preAction = null,
                                            Action p_postAction = null)
    {
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
        Sequence preSequence = DOTween.Sequence();
        //transitionUI.gameObject.SetActive(true);
        preSequence.Join(transitionUI.DOFade(p_preOpacity, p_preTransitionTime));

        yield return preSequence.WaitForCompletion();

        p_preAction?.Invoke();
        yield return new WaitForSeconds(p_delayTime);
        Sequence postSequence = DOTween.Sequence();
        postSequence.Join(transitionUI.DOFade(p_postOpacity, p_postTransitionTime));
        yield return postSequence.WaitForCompletion();

        //transitionUI.gameObject.SetActive(false);
        p_postAction?.Invoke();
    }
}
