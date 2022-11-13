using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using DG.Tweening;
public class PopUpUI : MonoBehaviour
{
    [SerializeField] GameObject frame;

    RectTransform detailedFrameRectTransform;
    Image detailedFrameImage;
    [SerializeField] TMP_Text detailedTitleText;
    [SerializeField] TMP_Text detailedContentText;

    [SerializeField]
    private Vector2 defaultPosition;

    [SerializeField]
    private Vector2 targetPosition;
    [SerializeField]
    private Vector2 defaultSize;

    [SerializeField]
    private Vector2 targetSize;

    [SerializeField]
    private float avatarMoveTime;
    [SerializeField]
    private float avatarFadeTime;
    [SerializeField]
    private float avatarDelayTime;
    [SerializeField]
    private float wordDelayTime;
    [SerializeField]
    private float wordFadeTime;
    [SerializeField]
    private float avatarSizeTime;
    public static Action<string,string> OnPopUpEvent;

    private void Awake()
    {
        detailedFrameRectTransform = frame.GetComponent<RectTransform>();
        detailedFrameImage = frame.GetComponent<Image>();
        OnPopUpEvent += Initialize;
        CharacterDialogueUI.OnResettingCharacterUIEvent += CloseUI;
    }

    private void OnDestroy()
    {
        OnPopUpEvent -= Initialize;
        CharacterDialogueUI.OnResettingCharacterUIEvent -= CloseUI;
    }
    public void Initialize(string p_title,string p_content)
    {
        Debug.Log("POPPING UP " + p_content);

        detailedTitleText.text = p_title;
        detailedContentText.text = p_content;
        frame.SetActive(true);
        StartCoroutine(In());
    }

    public void CloseUI()
    {
        
        StartCoroutine(Out());
    }

    IEnumerator In()
    {
       // CharacterDialogueUI.OnAddNewTransitionEvent.Invoke();
        var fadeOutSequence = DOTween.Sequence()
        .Append(detailedFrameRectTransform.DOAnchorPos(targetPosition, avatarMoveTime));
        fadeOutSequence.Join(detailedFrameImage.DOFade(1, avatarFadeTime));
        fadeOutSequence.Play();
        yield return new WaitForSeconds(wordDelayTime);
        var wordSequence = DOTween.Sequence()
        .Append(detailedTitleText.DOFade(1, wordFadeTime));
        wordSequence.Play();
        yield return fadeOutSequence.WaitForCompletion();
        yield return wordSequence.WaitForCompletion();
        yield return new WaitForSeconds(avatarDelayTime);

        //Expand
        var fadeInSequence = DOTween.Sequence()
        .Append(detailedFrameRectTransform.DOSizeDelta(targetSize, avatarSizeTime));
        fadeInSequence.Join(detailedContentText.DOFade(1, 1.5f));
        fadeInSequence.Play();
        //CharacterDialogueUI.OnFinishTransitionEvent.Invoke();
    }

    IEnumerator Out()
    {
        // CharacterDialogueUI.OnAddNewTransitionEvent.Invoke();
        var fadeOutSequence = DOTween.Sequence()
        .Append(detailedFrameImage.DOFade(0, avatarFadeTime));
        fadeOutSequence.Join(detailedTitleText.DOFade(0, 0.2f));
        fadeOutSequence.Join(detailedContentText.DOFade(0, 0.2f));
        fadeOutSequence.Join(detailedFrameRectTransform.DOSizeDelta(new Vector2(0,0), avatarSizeTime));
        fadeOutSequence.Play();
        yield return fadeOutSequence.WaitForCompletion();
        detailedTitleText.text = "";
        detailedContentText.text = "";
        detailedFrameRectTransform.sizeDelta = defaultSize;
        detailedFrameRectTransform.anchoredPosition = defaultPosition;
        //var fadeeeSequence = DOTween.Sequence()
        //.Append(detailedFrameRectTransform.DOAnchorPos(defaultPosition, avatarMoveTime));
        frame.SetActive(false);
        //CharacterDialogueUI.OnFinishTransitionEvent.Invoke();
    }
}
