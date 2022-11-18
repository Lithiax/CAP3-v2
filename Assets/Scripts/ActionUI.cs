using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
[System.Serializable]
public enum CueType
{
    Voice,
    Body_Posture,
    Proxemic,
    Eye_Contact,
    Gesture,
    None
}

public class ActionUI : MonoBehaviour
{
    [SerializeField]
    public CueType cueType;
    public Image image; 
    bool can = true;
    private Action justExited;
    [SerializeField] private List<TMP_Text> tmp = new List<TMP_Text>();
    IEnumerator runningCoroutine;
    IEnumerator runningCDCoroutine;
    Sequence fadeInSeq;
    public bool canDoWork = true;
    private void Awake()
    {
        image = GetComponent<Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
        for (int i = 0; i < tmp.Count; i++)
        {
            tmp[i].color = new Color(tmp[i].color.r, tmp[i].color.g, tmp[i].color.b, 0f);
        }
        justExited += JustExited;
        ChoicesUI.OnChoosingChoiceEvent += startchoose;
        CharacterDialogueUI.OnEndChooseChoiceEvent += canDoReset;
    }

    private void OnDestroy()
    {
        justExited -= JustExited;
        ChoicesUI.OnChoosingChoiceEvent -= startchoose;
        CharacterDialogueUI.OnEndChooseChoiceEvent -= canDoReset;
    }

    void canDoReset()
    {
        canDoWork = true;
        can = true;
    }

    void startchoose(List<ChoiceData> p_choiceDatas)
    {
        canDoWork = false;
        can = false;
        ActionUIs.onExitEvent.Invoke();
        JustExited();
    }
    void JustExited()
    {
        Debug.Log("EVENT CALL");
        if (runningCoroutine != null)
        {
            Debug.Log("EVENT stopped");
            StopCoroutine(runningCoroutine);

            image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
            for (int i = 0; i < tmp.Count; i++)
            {
                tmp[i].color = new Color(tmp[i].color.r, tmp[i].color.g, tmp[i].color.b, 0f);
            }
        }
        if (fadeInSeq != null)
        {
            fadeInSeq.Kill();
        }

    }

  
    public void EnterFunction()
    {
        Debug.Log("IS SHOWING: " + ActionUIs.isShowing + " - " + canDoWork);
        if (!ActionUIs.isShowing)
        {
            if (canDoWork)
            {
                ActionUIs.onEnterEvent.Invoke(this);
                if (runningCoroutine != null)
                {
                    StopCoroutine(runningCoroutine);
                }
                runningCoroutine = OpenTransition();
                StartCoroutine(runningCoroutine);
            }
        }
      
     
    }

    public void ExitFunction()
    {
        ActionUIs.onExitEvent.Invoke();
        //if (runningCoroutine != null)
        //{
        //    StopCoroutine(runningCoroutine);
        //}
        //runningCoroutine = CloseTransition();
        StartCoroutine(CloseTransition());
    }

    public void PointClickFunction()
    {
        Debug.Log("ACTIVE: " + canDoWork + " - " + can);
        if (canDoWork)
        {
            if (can)
            {
                can = false;

                if (runningCDCoroutine != null)
                {
                    StopCoroutine(runningCDCoroutine);
                }
                runningCDCoroutine = cd();
                StartCoroutine(runningCDCoroutine);
                ActionUIs.onPointClickEvent.Invoke(this);
            }
        }
            
     
    }
    IEnumerator OpenTransition()
    {
        Debug.Log("ENTER");
  
        var fadeOutSequence = DOTween.Sequence()
            .Append(image.DOFade(0.3f, 1f));
        for (int i =0; i< tmp.Count; i++)
        {
            fadeOutSequence.Join(tmp[i].DOFade(1,1f));
        }
        fadeInSeq = fadeOutSequence;
        fadeOutSequence.Play();
        yield return fadeOutSequence.WaitForCompletion();
        Debug.Log("ENTER ended");

    }
    IEnumerator CloseTransition()
    {
        Debug.Log("EXIT");
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
        }
        if (fadeInSeq != null)
        {
            fadeInSeq.Kill();
        }
        var fadeOutSequence = DOTween.Sequence()
            .Append(image.DOFade(0, 0.25f));
        for (int i = 0; i < tmp.Count; i++)
        {
            fadeOutSequence.Join(tmp[i].DOFade(0, 0.25f));
        }
        fadeOutSequence.Play();
   
        yield return fadeOutSequence.WaitForCompletion();
        Debug.Log("EXIT ended");
        justExited.Invoke();
        //yield return new WaitForSeconds(0.65f);
     

    }

    IEnumerator cd()
    {

        yield return new WaitForSeconds(1f);
        can = true;
    }
}
