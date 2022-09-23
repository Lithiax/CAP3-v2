using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;
public class CharacterSpokenToEvent : UnityEvent<string, SO_Dialogues> { }
public class FirstTimeFoodOnEndEvent : UnityEvent { }

//[System.Serializable]
//public class CharacterPresetData
//{
//    public SO_Character so_Character;
//    public RectTransform avatarRectTransform;
    
//    [HideInInspector] public Image avatarImage;


//    //public void Initialize()
//    //{
//    //    avatarImage = avatarRectTransform.GetComponent<Image>();
//    //}
//}

public class CharacterDialogueUI : MonoBehaviour
{
    [HeaderAttribute("REQUIRED COMPONENTS")]
    [SerializeField] private GameObject frame;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private TMP_Text dialogueText;

    [SerializeField] private Image backgroundImage;

    [SerializeField] public GameObject nextDialogueButton;
    [SerializeField] private GameObject choiceUIsContainer;
    private Transform choiceUIsContainerTransform;
    private RectTransform choiceUIsContainerRectTransform;

    [SerializeField] private Transform characterContainerTransform;
    [SerializeField] private ChoiceUI choiceUIPrefab;

    [SerializeField]
    private List<CharacterUI> characterPresetDatas = new List<CharacterUI>();
   // [SerializeField] private List<SO_Character> oldCharacters = new List<SO_Character>();
    [SerializeField] private RectTransform leftCharacterPresetRectTransform;
    [SerializeField] private RectTransform centerCharacterPresetRectTransform;
    [SerializeField] private RectTransform rightCharacterPresetRectTransform;

    [SerializeField] private TMP_Text hapticText;
    [SerializeField] private TMP_Text vocalicText;
    [SerializeField] private TMP_Text kinesicText;
    [SerializeField] private TMP_Text oculesicText;
    [SerializeField] private TMP_Text physicalAppearanceText;

    [SerializeField] private CharacterUI staticCharacterPrefab;

    [HeaderAttribute("ADJUSTABLE VALUES")]

    [SerializeField] private Color32 nonSpeakerTintColor;
    [SerializeField] private float typewriterSpeed = 0.1f;

    [SerializeField]
    private float avatarFadeTime;
    [SerializeField]
    private float avatarDelayTime;

    [HideInInspector]
    public SO_Dialogues currentSO_Dialogues;

    private int currentDialogueIndex;

    //private new List<IEnumerator> cachedCoroutines = new List<IEnumerator>();// canSkip = false;
    private int runningCoroutines = 0;
    private bool isSkipping = false;
    private string id;

    bool isStartTransitionEnabled = true;
    bool isEndTransitionEnabled = true;

    bool isAdvancedonWorldEventEndedEvent = false;

    bool isAlreadyEnded = false;

    public static CharacterSpokenToEvent onCharacterSpokenToEvent = new CharacterSpokenToEvent();
  

    private void Awake()
    {
        onCharacterSpokenToEvent.AddListener(OnCharacterSpokenTo);
        choiceUIsContainerTransform = choiceUIsContainer.transform;
        choiceUIsContainerRectTransform = choiceUIsContainer.GetComponent<RectTransform>();

        //for (int i =0; i< characterPresetDatas.Count; i++)
        //{
        //    characterPresetDatas[i].Initialize();
        //}

    }

    public void OnCharacterSpokenTo(string p_id, SO_Dialogues p_SO_Dialogue)
    {
        id = p_id;
        currentSO_Dialogues = p_SO_Dialogue;

        if (isStartTransitionEnabled)
        {
            TransitionUI.onFadeInAndOutTransition.Invoke(1, 0.5f, 1, 0, 0.5f, OnOpenCharacterDialogueUI);
        }
        else
        {
            OnOpenCharacterDialogueUI();
        }


    }

    public void OnOpenCharacterDialogueUI()
    {
        frame.SetActive(true);
        ResetCharacterDialogueUI();
    }

    public void OnCloseCharacterDialogueUI()
    {
        frame.SetActive(false);
    }

    public IEnumerator Co_TypeWriterEffect(TMP_Text p_textUI, string p_fullText)
    {
        runningCoroutines++;
        string p_currentText;
        for (int i = 0; i <= p_fullText.Length; i++)
        {
            p_currentText = p_fullText.Substring(0, i);
            p_textUI.text = p_currentText;
            yield return new WaitForSeconds(typewriterSpeed);
        }
        runningCoroutines--;
    }

    void ResetCharacterDialogueUI()
    {
        currentDialogueIndex = 0;
        //canSkip = false;
        runningCoroutines = 0;
        isAlreadyEnded = false;
        nextDialogueButton.SetActive(true);
        choiceUIsContainer.SetActive(false);
        OnNextButtonUIPressed();
    }

    void NextDialogue()
    {
        Debug.Log("NEXT DIALOGUE");
        currentDialogueIndex++;
        //canSkip = false;
        runningCoroutines = 0;
        isSkipping = false;
        if (currentDialogueIndex == currentSO_Dialogues.dialogues.Count)
        {
            if (isAdvancedonWorldEventEndedEvent)
            {
                isAlreadyEnded = true;
            }
        }
    }

    IEnumerator AvatarFadeIn(Image p_avatarImage, Sprite p_sprite)
    {
        runningCoroutines++;
        p_avatarImage.sprite = p_sprite;
        var fadeInSequence = DOTween.Sequence()
        .Append(p_avatarImage.DOFade(1, avatarFadeTime));
        fadeInSequence.Play();
        yield return fadeInSequence.WaitForCompletion();
        runningCoroutines--;
    }

    IEnumerator AvatarFadeOut(Image p_avatarImage, CharacterUI p_newCharacter)
    {
        Debug.Log("FADING OUT");
        runningCoroutines++;
        var fadeOutSequence = DOTween.Sequence()
       .Append(p_avatarImage.DOFade(0, avatarFadeTime));
        fadeOutSequence.Play();
        yield return fadeOutSequence.WaitForCompletion();
        runningCoroutines--;
        characterPresetDatas.Remove(p_newCharacter);
        Destroy(p_newCharacter.gameObject);
    }

    IEnumerator SpeakerTintIn(Image p_avatarImage)
    {
        runningCoroutines++;
        var fadeOutSequence = DOTween.Sequence()
        .Append(p_avatarImage.DOColor(Color.white, avatarFadeTime));
        fadeOutSequence.Play();
        yield return fadeOutSequence.WaitForCompletion();
        runningCoroutines--;
    }

    IEnumerator SpeakerTintOut(Image p_avatarImage)
    {
        runningCoroutines++;
        var fadeInSequence = DOTween.Sequence()
        .Append(p_avatarImage.DOColor(nonSpeakerTintColor, avatarFadeTime));
        fadeInSequence.Play();
        yield return fadeInSequence.WaitForCompletion();
        runningCoroutines--;
    }
    void CreateChoiceUIs()
    {  
        nextDialogueButton.SetActive(false);
        choiceUIsContainer.SetActive(true);
        for (int i =0; i <currentSO_Dialogues.choiceDatas.Count; i++)
        {
            ChoiceUI newChoiceUI = Instantiate(choiceUIPrefab, choiceUIsContainerTransform);
            newChoiceUI.InitializeValues(currentSO_Dialogues.choiceDatas[i].words, "");
            int index = i;
            newChoiceUI.GetComponent<Button>().onClick.AddListener(delegate { ChooseChoiceUI(index); });
            LayoutRebuilder.ForceRebuildLayoutImmediate(choiceUIsContainerRectTransform);
        }
    }

    public void ChooseChoiceUI(int index)
    {
        nextDialogueButton.SetActive(true);
        choiceUIsContainer.SetActive(false);
        currentSO_Dialogues = currentSO_Dialogues.choiceDatas[index].so_branchDialogue;
        ResetCharacterDialogueUI();
    }

    void SetRectTransformToPreset(CharacterPositionType p_characterPositionType, SO_Character p_index)
    {
        CharacterUI foundPreset = FindPreset(p_index);
        if (foundPreset != null)
        {
            if (p_characterPositionType == CharacterPositionType.left)
            {
                foundPreset.avatarRectTransform.anchoredPosition = leftCharacterPresetRectTransform.anchoredPosition;
            }
            else if (p_characterPositionType == CharacterPositionType.center)
            {
                foundPreset.avatarRectTransform.anchoredPosition = centerCharacterPresetRectTransform.anchoredPosition;
            }
            else if (p_characterPositionType == CharacterPositionType.right)
            {
                foundPreset.avatarRectTransform.anchoredPosition = rightCharacterPresetRectTransform.anchoredPosition;
            }
        }
    }

    void SetSpeakerTint(bool p_isSpeaking, SO_Character p_index)
    {

        CharacterUI foundPreset = FindPreset(p_index);
        if (foundPreset != null)
        {
            if (isSkipping)
            {
                //Tint
                if (p_isSpeaking)
                {
                    //Add reference coroutine so when player skips it can be referenced and stopped
                    foundPreset.avatarImage.color = new Color(1, 1, 1, 1);
                }
                else
                {
                    foundPreset.avatarImage.color = nonSpeakerTintColor;

                }
            }
            else
            {
                //Tint
                if (p_isSpeaking)
                {
                    //Add reference coroutine so when player skips it can be referenced and stopped
                    StartCoroutine(SpeakerTintIn(foundPreset.avatarImage));

                }
                else
                {
                    StartCoroutine(SpeakerTintOut(foundPreset.avatarImage));

                }
            }
             

        }
        
    }

    void SetBackground(Sprite p_backgroundSprite)
    {
        if (p_backgroundSprite != null)
        {
            backgroundImage.sprite = p_backgroundSprite;
            backgroundImage.color = new Color32(255, 255, 255, 255);
        }
        else if (p_backgroundSprite == null)
        {
            backgroundImage.color = new Color32(0, 0, 0, 0);
        }
    }


    void RemoveAvatar(List<SO_Character> p_charactersToBeRemoved)
    {
        //Do functions to characters to be Added
        for (int i = 0; i < p_charactersToBeRemoved.Count; i++)
        {
            Debug.Log("REMOVING " + p_charactersToBeRemoved[i] + " - " + p_charactersToBeRemoved.Count);
            CharacterUI foundPreset = FindPreset(p_charactersToBeRemoved[i]);

            Debug.Log("REMOVINGDD ");
            if (foundPreset != null)
            {
                if (isSkipping)
                {
                
                    Debug.Log("REMOVINGRR " + foundPreset);
                    Destroy(foundPreset.gameObject);
                }
                else
                {
                    StartCoroutine(AvatarFadeOut(foundPreset.avatarImage, foundPreset));
                }
            }
           
        }
    }

  

    void AddAvatar(List<SO_Character> p_charactersToBeAdded)
    {
        //Do functions to characters to be Added
        if (isSkipping)
        {
            for (int i = 0; i < characterPresetDatas.Count; i++)
            {
                characterPresetDatas[i].avatarImage.color = new Color(1, 1, 1, 1);
            }

        }
        else
        {
            for (int i = 0; i < p_charactersToBeAdded.Count; i++)
            {
                CharacterUI newCharacter = Instantiate(staticCharacterPrefab,characterContainerTransform);
                characterPresetDatas.Add(newCharacter);
                newCharacter.so_Character = p_charactersToBeAdded[i];
                StartCoroutine(AvatarFadeIn(newCharacter.avatarImage, p_charactersToBeAdded[i].avatar));
            }
        }
    }

    public IEnumerator AvatarFlipSequence(Image p_avatarImage, RectTransform p_avatarRectTransform, Quaternion p_quaternion)
    {
        var fadeOutSequence = DOTween.Sequence()
        .Append(p_avatarImage.DOFade(0, avatarFadeTime));
        fadeOutSequence.Play();
        yield return fadeOutSequence.WaitForCompletion();
        
        p_avatarRectTransform.rotation = p_quaternion;
    
        var fadeInSequence = DOTween.Sequence()
        .Append(p_avatarImage.DOFade(1, avatarFadeTime));
        fadeInSequence.Play();
        yield return fadeInSequence.WaitForCompletion();
    }
   

    void SetAvatarFlipOrientation(CharacterData p_characterData) // work on this
    {

        CharacterUI foundPreset = FindPreset(p_characterData.character);
        if (foundPreset != null)
        {
            Quaternion target;
            if (p_characterData.isFlipped)
            {
                target = Quaternion.Euler(0f, 180f, 0f);
            }
            else
            {
                target = Quaternion.Euler(0f, 0f, 0f);
            }
            if (foundPreset.avatarRectTransform.rotation != target)
            {
                StartCoroutine(AvatarFlipSequence(foundPreset.avatarImage,foundPreset.avatarRectTransform, target));
            }
               
        }
        
        
    }

    void SetSpeakerName(List<CharacterData> p_characterDatas) // work on this
    {
        for (int i = 0; i < p_characterDatas.Count; i++)
        {
            
            if (p_characterDatas[i].isSpeaking)
            {
                characterNameText.text = p_characterDatas[i].character.name;

            }

        }
    }
    void IdentifyCharactersToAdd(List<SO_Character> newList, List<SO_Character> oldList,List<SO_Character> charactersToBeAdded)
    {
       
        //Mark the Characters to Add and Characters that Exists
        for (int i = 0; i < newList.Count; i++)
        {
            if (oldList.Count > 0)
            {
                for (int x = 0; x < oldList.Count;)
                {
                    x++;
                    if (x >= oldList.Count)//new
                    {
                        charactersToBeAdded.Add(oldList[x]);
                    }
                }
            }
            else
            {
                charactersToBeAdded.Add(newList[i]);
            }

        }

    }
    void IdentifyCharactersToRemove(List<SO_Character> oldList, List<SO_Character> p_currentCharacter, List<SO_Character> charactersToBeRemoved)
    {
        //Mark the Characters to Remove
        for (int oldIndex = 0; oldIndex < oldList.Count; oldIndex++)
        {
            if (p_currentCharacter.Count > 0)
            {
                for (int currentIndex = 0; currentIndex < p_currentCharacter.Count;)
                {
                    if (oldList[oldIndex] == p_currentCharacter[currentIndex])
                    {
                        break;
                    }

                    currentIndex++;
                    if (currentIndex >= p_currentCharacter.Count)
                    {
                        if (charactersToBeRemoved.Count > 0)
                        {
                            for (int removedIndex = 0; removedIndex < charactersToBeRemoved.Count;)
                            {
                                if (charactersToBeRemoved[removedIndex] == oldList[currentIndex])
                                {
                                    //It already is marked to be removed
                                    break;
                                }
                                removedIndex++;
                                if (currentIndex >= charactersToBeRemoved.Count)
                                {
                                    //It has not been already marked to be removed
                                    charactersToBeRemoved.Add(oldList[currentIndex]);
                                }


                            }
                        }
                        else
                        {
                            Debug.Log(oldList.Count);
                            Debug.Log(oldList[currentIndex]);
                            charactersToBeRemoved.Add(oldList[currentIndex]);
                        }



                    }
                }
            }
            else
            {
                charactersToBeRemoved.Add(oldList[oldIndex]);
            }

        }

    }

    void CheckCachedCharacters(List<CharacterData> p_characterDatas) //Rename and chop things into functions
    {
        List<SO_Character> newCharacters = new List<SO_Character>();
        List<SO_Character> oldCharacters = new List<SO_Character>();
        List<SO_Character> charactersToBeRemoved = new List<SO_Character>();
        List<SO_Character> charactersToBeAdded = new List<SO_Character>();

        for (int i = 0; i < p_characterDatas.Count; i++)
        {
            newCharacters.Add(p_characterDatas[i].character);
            //Debug.Log("CURRENT: " + newCharacters[i].name);
        }
        for (int i = 0; i < characterPresetDatas.Count; i++)
        {
            oldCharacters.Add(characterPresetDatas[i].so_Character);
            //Debug.Log("saved: " + characterPresetDatas[i].name);
        }
        if (newCharacters.Count > characterPresetDatas.Count)
        {
            //Mark the Characters to Add and Characters that Exists
            IdentifyCharactersToAdd(newCharacters, oldCharacters, charactersToBeAdded);
        }
        else
        {
            //Mark the Characters to Remove
            IdentifyCharactersToRemove(oldCharacters, newCharacters, charactersToBeRemoved);
         
        }
        for (int i = 0; i < charactersToBeAdded.Count; i++)
        {

            Debug.Log("ADD: " + charactersToBeAdded[i].name);
        }
        for (int i = 0; i < charactersToBeRemoved.Count; i++)
        {
            Debug.Log("REMOVE: " + charactersToBeRemoved[i].name);
        }

        RemoveAvatar(charactersToBeRemoved);
        AddAvatar(charactersToBeAdded);
        
     

        //so_Characters.Clear();
        //for (int i = 0; i < characterPresetDatas.Count; i++)
        //{
        //    so_Characters.Add(characterPresetDatas[i].so_Character);
        //    //if (characterPresetDatas[i].so_Character != null)
        //    //{
        //    //    SetSpeakerTint(currentSO_Dialogues.dialogues[currentDialogueIndex].characterDatas[i].isSpeaking, currentSO_Dialogues.dialogues[currentDialogueIndex].characterDatas[i].character);
        //    //}
        //}


    }

    CharacterUI FindPreset(SO_Character p_so_Character)
    {
       
        for (int x = 0; x < characterPresetDatas.Count; x++)
        {
            if (characterPresetDatas[x].so_Character == p_so_Character)
            {
                return characterPresetDatas[x];
                   
            }

        }
        return null;

      
    }
    void SetSpeech(SpeechTransitionType p_Type, string p_words)
    {
        if (isSkipping || p_Type == SpeechTransitionType.None)
        {
            SetWords(p_words);
        }
        else if (p_Type == SpeechTransitionType.Typewriter)
        {
            StartCoroutine(Co_TypeWriterEffect(dialogueText, p_words));
        }
   
    }
    void SetWords(string p_words)
    {
        dialogueText.text = p_words;
    }

    void SetCueBank(Dialogue p_characterDatas)
    {
        //for (int i =0; i < p_characterDatas.Count; i++)
        //{
        //    if (p_characterDatas[i].isSpeaking)
        //    {
        //        hapticText.text = p_characterDatas[i].hapticType.ToString();
        //        vocalicText.text = p_characterDatas[i].vocalicType.ToString();
        //        kinesicText.text = p_characterDatas[i].kinesicType.ToString();
        //        oculesicText.text = p_characterDatas[i].oculesicType.ToString();
        //        physicalAppearanceText.text = p_characterDatas[i].physicalApperanceType.ToString();
        //        break;
        //    }

        //}
       
        hapticText.text = p_characterDatas.hapticType.ToString();
        vocalicText.text = p_characterDatas.vocalicType.ToString();
        kinesicText.text = p_characterDatas.kinesicType.ToString();
        oculesicText.text = p_characterDatas.oculesicType.ToString();
        physicalAppearanceText.text = p_characterDatas.physicalApperanceType.ToString();
          

    }
    public void OnNextButtonUIPressed()
    {
        
        if (currentDialogueIndex < currentSO_Dialogues.dialogues.Count)
        {
            Dialogue currentDialogue = currentSO_Dialogues.dialogues[currentDialogueIndex];
            
            if (runningCoroutines > 0)
            {
                isSkipping = true;
                StopAllCoroutines();
                runningCoroutines=0;
            }
            else
            {
                frame.SetActive(true);
                //oldCharacters.Clear();
                //for (int i = 0; i < characterPresetDatas.Count; i++)
                //{
                //    oldCharacters.Add(characterPresetDatas[i].so_Character);
                //    //if (characterPresetDatas[i].so_Character != null)
                //    //{
                //    //    SetSpeakerTint(currentSO_Dialogues.dialogues[currentDialogueIndex].characterDatas[i].isSpeaking, currentSO_Dialogues.dialogues[currentDialogueIndex].characterDatas[i].character);
                //    //}
                //}
                NextDialogue();
            }
            CheckCachedCharacters(currentDialogue.characterDatas); //Rename and chop things into functions
            for (int i = 0; i < currentDialogue.characterDatas.Count; i++)
            {
                //Set Character UI Rect Transform
                //Position
                SetRectTransformToPreset(currentDialogue.characterDatas[i].characterPosition, currentDialogue.characterDatas[i].character);
                SetAvatarFlipOrientation(currentDialogue.characterDatas[i]);
               // SetSpeakerTint(currentSO_Dialogues.dialogues[currentDialogueIndex].characterDatas[i].isSpeaking, currentSO_Dialogues.dialogues[currentDialogueIndex].characterDatas[i].character);
            }

            SetSpeakerName(currentDialogue.characterDatas);
            SetBackground(currentDialogue.backgroundSprite);
            SetSpeech(currentDialogue.speechTransitionType, currentDialogue.words);
            SetCueBank(currentDialogue);
        }
        else if (currentDialogueIndex >= currentSO_Dialogues.dialogues.Count)
        {
            if (!isAlreadyEnded)
            {
                if (currentSO_Dialogues.choiceDatas.Count > 1)
                {
                    CreateChoiceUIs();
                }
                else if (currentSO_Dialogues.choiceDatas.Count == 1)
                {
                    if (isEndTransitionEnabled)
                    {
                        TransitionUI.onFadeInAndOutTransition.Invoke(1, 0.25f, 1, 0, 0.25f, OnCloseCharacterDialogueUI);
                    }
                    else
                    {
                        OnCloseCharacterDialogueUI();
                    }
                }
                else
                {
                    ResetCharacterDialogueUI();
                }
            }
        }
    }
}
