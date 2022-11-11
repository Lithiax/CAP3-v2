using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;
public class SpeakerDialogueUI : MonoBehaviour
{
    [SerializeField] private float typewriterSpeed = 0.1f;
    [SerializeField] CharacterDialogueUI characterDialogueUI;
    public GameObject frame;
  
    [SerializeField]
    private GameObject currentDialogueBox;
    private Image currentDialogueBoxImage;
    [SerializeField]
    private GameObject currentSpeakerBox;

    [SerializeField] private TMP_Text currentDialogueText;

    [SerializeField] private TMP_Text currentSpeakerText;


    [SerializeField]
    private TMP_Text smallDialogueText;
    [SerializeField]
    private TMP_Text bigDialogueText;

    [SerializeField]
    private GameObject smallSpeakerBox;
    [SerializeField]
    private GameObject bigSpeakerBox;

    [SerializeField]
    private GameObject smallDialogueBox;
    private Image smallDialogueBoxImage;
    [SerializeField]
    private GameObject bigDialogueBox;
    private Image bigDialogueBoxImage;

    [SerializeField]
    private TMP_Text smallSpeakerText;
    [SerializeField]
    private TMP_Text bigSpeakerText;

    [SerializeField]
    private GameObject extraButtonsContainer;

    [SerializeField]
    float avatarFadeTime;
    string currentWords;
    bool canOpen = true;
    private void Awake()
    {
        smallDialogueBoxImage = smallDialogueBox.GetComponent<Image>();
        bigDialogueBoxImage = bigDialogueBox.GetComponent<Image>();
        smallDialogueBoxImage.color = new Color32(255, 255, 255, 0);
        bigDialogueBoxImage.color = new Color32(255, 255, 255, 0);
        currentDialogueBoxImage = currentDialogueBox.GetComponent<Image>();
        currentDialogueBoxImage.color = new Color32(255, 255, 255, 255);
        CharacterDialogueUI.OnIsSkipping += Skip;
        CharacterDialogueUI.OnInspectingEvent += open;
        CharacterDialogueUI.OnDeinspectingEvent += close;
    }
    
    void open()
    {
        frame.SetActive(false);
    }

    void close()
    {
        frame.SetActive(true);
    }
    public void ResetSpeakerDialogueUI()
    {
        currentDialogueBox.gameObject.SetActive(false);
        currentSpeakerBox.gameObject.SetActive(false);
        extraButtonsContainer.gameObject.SetActive(false);
    }

    public void ManualToggleSpeakerDialogueUI(bool p_desiredToggle)
    {
        currentDialogueBox.gameObject.SetActive(p_desiredToggle);
        currentSpeakerBox.gameObject.SetActive(p_desiredToggle);
        extraButtonsContainer.gameObject.SetActive(p_desiredToggle);
        Debug.Log(p_desiredToggle);
    }
    public void ToggleExtras()
    {
        if (canOpen)
        {
            canOpen = false;
            StartCoroutine(Out());
            if (!smallDialogueBox.activeSelf)
            {
                currentDialogueBox = smallDialogueBox;
                currentDialogueBoxImage = smallDialogueBoxImage;
                currentDialogueText = smallDialogueText;
                smallSpeakerText.text = currentSpeakerText.text;
                currentSpeakerText = smallSpeakerText;
            }
            else if (smallDialogueBox.activeSelf)
            {
                currentDialogueBox = bigDialogueBox;
                currentDialogueBoxImage = bigDialogueBoxImage;
                currentDialogueText = bigDialogueText;
                bigSpeakerText.text = currentSpeakerText.text;
                currentSpeakerText = bigSpeakerText;

            }

            if (StorylineManager.currentDialogueIndex >= StorylineManager.currentSO_Dialogues.dialogues.Count)
            {
                StorylineManager.currentDialogueIndex = StorylineManager.currentSO_Dialogues.dialogues.Count - 1;
            }

            //smallSpeakerBox.SetActive(!smallSpeakerBox.activeSelf);
            //smallDialogueBox.SetActive(!smallDialogueBox.activeSelf);
            //bigSpeakerBox.SetActive(!bigSpeakerBox.activeSelf);
            //bigDialogueBox.SetActive(!bigDialogueBox.activeSelf);
            extraButtonsContainer.SetActive(!extraButtonsContainer.activeSelf);
            if (characterDialogueUI.runningCoroutines > 0 && !CharacterDialogueUI.isSkipping)
            {
                CharacterDialogueUI.isSkipping = true;
                CharacterDialogueUI.OnIsSkipping.Invoke();
                StopAllCoroutines();
                characterDialogueUI.runningCoroutines = 0;
                // Debug.Log("READYING");

            }
            Dialogue currentDialogue = StorylineManager.currentSO_Dialogues.dialogues[StorylineManager.currentDialogueIndex];
            SetSpeech(currentDialogue.words);
            StartCoroutine(In());
        }
       
    }

    public void SetSpeakerName(List<CharacterData> p_characterDatas) // work on this
    {
        if (p_characterDatas.Count > 0)
        {
            for (int i = 0; i < p_characterDatas.Count; i++)
            {

                if (p_characterDatas[i].isSpeaking)
                {
                    currentSpeakerText.text = p_characterDatas[i].character.stageName;
                    //Debug.Log("CURRENT SPEAKER NAME: " + p_characterDatas[i].character.stageName);
                }

            }
        }
        else
        {
            currentSpeakerText.text = "NO CHARACTER ASSIGNED";
        }

    }

    void Skip()
    {
        AudioManager.instance.ForceStopAudio("typewriting");
        StopAllCoroutines();
        SetWords(currentWords);
    }

    void SetWords(string p_words)
    {
        currentDialogueText.text = p_words;
    }

    public void SetSpeech(string p_words)
    {
        p_words = p_words.Replace("<MC>", StorylineManager.instance.mainCharacter.stageName);
        p_words = p_words.Replace("<nl>", "<br>");
        currentWords = p_words;
       
        StartCoroutine(Co_TypeWriterEffect(currentDialogueText, p_words));
        

    }
    IEnumerator Out()
    {
        GameObject save = currentDialogueBox;
        Image saveI = currentDialogueBoxImage;
        TMP_Text saveT1 = currentSpeakerText;
        TMP_Text saveT2 = currentDialogueText;
        // CharacterDialogueUI.OnAddNewTransitionEvent.Invoke();
        var fadeOutSequence = DOTween.Sequence()
        .Append(saveI.DOFade(0, avatarFadeTime));
        fadeOutSequence.Join(saveT1.DOFade(0, avatarFadeTime));
        fadeOutSequence.Join(saveT2.DOFade(0, avatarFadeTime));
        fadeOutSequence.Play();
        yield return fadeOutSequence.WaitForCompletion();
        save.SetActive(false);
        //CharacterDialogueUI.OnFinishTransitionEvent.Invoke();
    }
    IEnumerator In()
    {
        GameObject save = currentDialogueBox;
        Image saveI = currentDialogueBoxImage;
        TMP_Text saveT1 = currentSpeakerText;
        TMP_Text saveT2 = currentDialogueText;
        save.SetActive(true);
        // CharacterDialogueUI.OnAddNewTransitionEvent.Invoke();
        var fadeOutSequence = DOTween.Sequence()
        .Append(saveI.DOFade(1, avatarFadeTime));
        fadeOutSequence.Join(saveT1.DOFade(1, avatarFadeTime));
        fadeOutSequence.Join(saveT2.DOFade(1, avatarFadeTime));
        fadeOutSequence.Play();
        yield return fadeOutSequence.WaitForCompletion();
        canOpen = true;
        //CharacterDialogueUI.OnFinishTransitionEvent.Invoke();
    }
    public IEnumerator Co_TypeWriterEffect(TMP_Text p_textUI, string p_fullText)
    {
        //Debug.Log("ITS PLAYING");
        CharacterDialogueUI.OnAddNewTransitionEvent.Invoke();
        string p_currentText;
        for (int i = 0; i <= p_fullText.Length; i++)
        {
            p_currentText = p_fullText.Substring(0, i);
            p_textUI.text = p_currentText;
            AudioManager.instance.AdditivePlayAudio("typewriting");
            yield return new WaitForSeconds(typewriterSpeed);
        }

        CharacterDialogueUI.OnFinishTransitionEvent.Invoke();
    }
}