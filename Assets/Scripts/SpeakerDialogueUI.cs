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
    string so = "";
    bool typinLoop = false;
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

    private void OnDestroy()
    {
        CharacterDialogueUI.OnIsSkipping -= Skip;
        CharacterDialogueUI.OnInspectingEvent -= open;
        CharacterDialogueUI.OnDeinspectingEvent -= close;
    }

    public void LogBox()
    {
        PauseMenu.isPausingEvent.Invoke();
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
    }
    public void SetSpeakerName(List<CharacterData> p_characterDatas) // work on this
    {
        if (p_characterDatas.Count > 0)
        {
            for (int i = 0; i < p_characterDatas.Count; i++)
            {

                if (p_characterDatas[i].isSpeaking)
                {
                    if (p_characterDatas[i].character != null)
                    {

                      
                        if (string.IsNullOrEmpty(p_characterDatas[i].character.stageName) && p_characterDatas[i].character.idName != "You")
                        {
                            
                            currentSpeakerText.text = "NO CHARACTER SET";
                            
                        }
                        else
                        {
                            if (p_characterDatas[i].character.idName != "You")
                            {
                                currentSpeakerText.text = p_characterDatas[i].character.stageName;
                            }
                            else
                            {
                                if (!StorylineManager.renamed)
                                {

                                    currentSpeakerText.text = "YOU";
                                }
                                else
                                {
                                    currentSpeakerText.text = p_characterDatas[i].character.stageName;
                                }
                            }
                            
                          
                        }
                     
                    }
             
                }

            }
        }
        else
        {
            currentSpeakerText.text = " ";
        }

    }

    void Skip()
    {
        typinLoop = false;
        AudioManager.instance.ForceStopAudio(so);
        StopAllCoroutines();
        SetWords(currentWords);
    }

    public void SetWords(string p_words)
    {
        currentDialogueText.text = p_words;
    }
 
    public void SetSpeech(string p_words, string character = "")
    {
        p_words = p_words.Replace("<MC>", StorylineManager.instance.mainCharacter.stageName);
        currentWords = p_words;
        typinLoop = true;
        typinLoop = true;
        StartCoroutine(Co_TypeWriterEffect(currentDialogueText, p_words, character));


    }

    public IEnumerator Co_TypeWriterEffect(TMP_Text p_textUI, string p_fullText, string character)
    {
        CharacterDialogueUI.OnAddNewTransitionEvent.Invoke();
        so = "";

        if (character.ToLower() == "maeve")
            {
                so = character;
            }
            else if (character.ToLower() == "liam")
            {
                so = character;
            }
            else if (character.ToLower() == "brad")
            {
                so = character;
            }
            else if (character.ToLower() == "penelope")
            {
                so = character;
            }
            else
            {
                so = "Typewriting";
            }

        string p_currentText;
        bool eve = false;
        int i = 0;
        while (typinLoop && i < 500)
        {

            if (i < p_fullText.Length)
            {
                if (p_fullText[i] == '<')
                {

                    eve = true;
                    continue;
                }
                else if (p_fullText[i] == '>')
                {

                    eve = false;
                    continue;
                }

                if (!eve)
                {
                    AudioManager.instance.AdditivePlayAudio(so);
                    p_currentText = p_fullText.Substring(0, i);
                    p_textUI.text = p_currentText;
                    yield return new WaitForSeconds(typewriterSpeed);
                }
                i++;
            }
            else
            {
                typinLoop = false;
            }
  
        }
      
        CharacterDialogueUI.OnFinishTransitionEvent.Invoke();
    }
}
