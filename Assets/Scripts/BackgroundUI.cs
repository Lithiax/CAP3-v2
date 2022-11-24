using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;
public class BackgroundUI : MonoBehaviour
{
    [SerializeField] private Image currentBackgroundImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField]
    private Canvas backgroundCanvas;

 [SerializeField] private Image secondaryBackgroundImage;
    [SerializeField]
    private Canvas secondaryBackgroundCanvas;
    public static Action<Sprite> onSetBackgroundEvent;// = new onLoadAvatarsEvent

    private void Awake()
    {
        onSetBackgroundEvent += SetBackground;
        CharacterDialogueUI.OnIsSkipping += Skip;
    }

    private void OnDestroy()
    {
        onSetBackgroundEvent -= SetBackground;
        CharacterDialogueUI.OnIsSkipping -= Skip;
    }
    void Skip()
    {
        StopAllCoroutines();
        currentBackgroundImage.color = new Color32(255, 255, 255, 255);
    }
    void SetBackground(Sprite p_backgroundSprite)
    {

        if (!StorylineManager.sideDialogue)
        {
            //Debug.Log("BACKGROUND: " + StorylineManager.sideDialogue);

            if (p_backgroundSprite != null)
            {

                if (p_backgroundSprite != currentBackgroundImage.sprite)
                {
                    if (currentBackgroundImage == backgroundImage)
                    {
                        currentBackgroundImage = secondaryBackgroundImage;
                        secondaryBackgroundCanvas.sortingOrder = 0;
                        backgroundCanvas.sortingOrder = -1;
                    }
                    else
                    {
                        currentBackgroundImage = backgroundImage;
                        secondaryBackgroundCanvas.sortingOrder = -1;
                        backgroundCanvas.sortingOrder = 0;
                    }
                    currentBackgroundImage.color = new Color32(255, 255, 255, 0);
                    StartCoroutine(TransitionIm(p_backgroundSprite));
                    //currentBackgroundImage.color = new Color32(255, 255, 255, 255);
                }

            }
            else if (p_backgroundSprite == null)
            {

                //StartCoroutine(TransitionIm(p_backgroundSprite));
                currentBackgroundImage.sprite = p_backgroundSprite;
                currentBackgroundImage.color = new Color32(255, 255, 255, 255);
            }
        }
        else
        {
            Dialogue origDialo = StorylineManager.savedSO_Dialogues.dialogues[StorylineManager.savedDialogueIndex];
            if (origDialo.backgroundSprite != null)
            {

                if (origDialo.backgroundSprite != currentBackgroundImage.sprite)
                {
                    if (currentBackgroundImage == backgroundImage)
                    {
                        currentBackgroundImage = secondaryBackgroundImage;
                        secondaryBackgroundCanvas.sortingOrder = 0;
                        backgroundCanvas.sortingOrder = -1;
                    }
                    else
                    {
                        currentBackgroundImage = backgroundImage;
                        secondaryBackgroundCanvas.sortingOrder = -1;
                        backgroundCanvas.sortingOrder = 0;
                    }
                    currentBackgroundImage.color = new Color32(255, 255, 255, 0);
                    StartCoroutine(TransitionIm(origDialo.backgroundSprite));
                    //currentBackgroundImage.color = new Color32(255, 255, 255, 255);
                }
            }
            else if (origDialo.backgroundSprite == null)
            {

                //StartCoroutine(TransitionIm(p_backgroundSprite));
                currentBackgroundImage.sprite = origDialo.backgroundSprite;
                currentBackgroundImage.color = new Color32(255, 255, 255, 255);
            }
        }

        }

    IEnumerator TransitionIm(Sprite p_backgroundSprite)
    {
        currentBackgroundImage.sprite = p_backgroundSprite;
        var fadeInSequence = DOTween.Sequence()
            .Append(currentBackgroundImage.DOFade(1f, 0.25f));
        fadeInSequence.Play();
        yield return fadeInSequence.WaitForCompletion();
    }

}
