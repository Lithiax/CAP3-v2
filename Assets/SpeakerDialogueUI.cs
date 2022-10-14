using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class SpeakerDialogueUI : MonoBehaviour
{
    [SerializeField] private float typewriterSpeed = 0.1f;
    [SerializeField] CharacterDialogueUI characterDialogueUI;
    public GameObject frame;
  
    [SerializeField]
    private GameObject currentDialogueBox;

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
    [SerializeField]
    private GameObject bigDialogueBox;

    [SerializeField]
    private TMP_Text smallSpeakerText;
    [SerializeField]
    private TMP_Text bigSpeakerText;

    [SerializeField]
    private GameObject extraButtonsContainer;

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
        if (!smallDialogueBox.activeSelf)
        {
            currentDialogueText = smallDialogueText;
            smallSpeakerText.text = currentSpeakerText.text;
            currentSpeakerText = smallSpeakerText;
        }
        else if (smallDialogueBox.activeSelf)
        {
            currentDialogueText = bigDialogueText;
            bigSpeakerText.text = currentSpeakerText.text;
            currentSpeakerText = bigSpeakerText;

        }
        if (StorylineManager.currentDialogueIndex >= StorylineManager.currentSO_Dialogues.dialogues.Count)
        {
            StorylineManager.currentDialogueIndex = StorylineManager.currentSO_Dialogues.dialogues.Count - 1;
        }
        Dialogue currentDialogue = StorylineManager.currentSO_Dialogues.dialogues[StorylineManager.currentDialogueIndex];
        smallSpeakerBox.SetActive(!smallSpeakerBox.activeSelf);
        smallDialogueBox.SetActive(!smallDialogueBox.activeSelf);
        bigSpeakerBox.SetActive(!bigSpeakerBox.activeSelf);
        bigDialogueBox.SetActive(!bigDialogueBox.activeSelf);
        extraButtonsContainer.SetActive(!extraButtonsContainer.activeSelf);
        if (characterDialogueUI.runningCoroutines > 0 && !characterDialogueUI.isSkipping)
        {
            characterDialogueUI.isSkipping = true;
            StopAllCoroutines();
            characterDialogueUI.runningCoroutines = 0;
            // Debug.Log("READYING");

        }

        SetSpeech(currentDialogue.words);

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
    public void StopCoroutine()
    {
        StopAllCoroutines();
    }
    void SetWords(string p_words)
    {
        currentDialogueText.text = p_words;
    }

    public void SetSpeech(string p_words)
    {
        p_words = p_words.Replace("<MC>", StorylineManager.instance.mainCharacter.stageName);
        if (characterDialogueUI.isSkipping)
        {
            //Debug.Log("stop writing");
            AudioManager.instance.ForceStopAudio("typewriting");
            SetWords(p_words);
        }
        else
        {
            StartCoroutine(Co_TypeWriterEffect(currentDialogueText, p_words));
        }

    }

    public IEnumerator Co_TypeWriterEffect(TMP_Text p_textUI, string p_fullText)
    {
        //Debug.Log("ITS PLAYING");
        characterDialogueUI.runningCoroutines++;
        string p_currentText;
        for (int i = 0; i <= p_fullText.Length; i++)
        {
            p_currentText = p_fullText.Substring(0, i);
            p_textUI.text = p_currentText;
            AudioManager.instance.AdditivePlayAudio("typewriting");
            yield return new WaitForSeconds(typewriterSpeed);
        }
        characterDialogueUI.runningCoroutines--;

        characterDialogueUI.CheckIfReady();
    }
}
