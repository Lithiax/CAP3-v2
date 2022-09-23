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
    private List<CharacterUI> savedCharacterUIs = new List<CharacterUI>();
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
        CheckIfReady();
    }

    void ResetCharacterDialogueUI()
    {
        currentDialogueIndex = 0;
        isSkipping = false;
        runningCoroutines = 0;
        isAlreadyEnded = false;
        nextDialogueButton.SetActive(true);
        choiceUIsContainer.SetActive(false);
        OnNextButtonUIPressed();
    }

    void NextDialogue()
    {
        Debug.Log("NEXT DIALOGUE");
        isSkipping = false;
        
        currentDialogueIndex++;
        runningCoroutines = 0;
     
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
        CheckIfReady();
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
        savedCharacterUIs.Remove(p_newCharacter);
        Destroy(p_newCharacter.gameObject);
        CheckIfReady();
    }

    IEnumerator SpeakerTintIn(Image p_avatarImage)
    {
        runningCoroutines++;
        var fadeOutSequence = DOTween.Sequence()
        .Append(p_avatarImage.DOColor(Color.white, avatarFadeTime));
        fadeOutSequence.Play();
        yield return fadeOutSequence.WaitForCompletion();
        runningCoroutines--;
        CheckIfReady();
    }

    IEnumerator SpeakerTintOut(Image p_avatarImage)
    {
        runningCoroutines++;
        var fadeInSequence = DOTween.Sequence()
        .Append(p_avatarImage.DOColor(nonSpeakerTintColor, avatarFadeTime));
        fadeInSequence.Play();
        yield return fadeInSequence.WaitForCompletion();
        runningCoroutines--;
        CheckIfReady();
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
        //Debug.Log("-----REMOVING " + isSkipping);
        //Do functions to characters to be Added
        for (int i = 0; i < p_charactersToBeRemoved.Count; i++)
        {
            CharacterUI foundPreset = FindPreset(p_charactersToBeRemoved[i]);
            if (foundPreset != null)
            {
                if (isSkipping)
                {
                
                   // Debug.Log("REMOVINGRR " + foundPreset);
                    savedCharacterUIs.Remove(foundPreset);
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
        //Debug.Log("-----ADDING " + isSkipping);
        if (isSkipping)
        {
            for (int i = 0; i < savedCharacterUIs.Count; i++)
            {
                savedCharacterUIs[i].avatarImage.color = new Color(1, 1, 1, 1);
            }

        }
        else
        {
            for (int i = 0; i < p_charactersToBeAdded.Count; i++)
            {
                CharacterUI newCharacter = Instantiate(staticCharacterPrefab,characterContainerTransform);
                savedCharacterUIs.Add(newCharacter);
                newCharacter.so_Character = p_charactersToBeAdded[i];
                //Debug.Log("ADDING " + newCharacter);
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

        CheckIfReady();
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
        if (p_characterDatas.Count > 0)
        {
            for (int i = 0; i < p_characterDatas.Count; i++)
            {

                if (p_characterDatas[i].isSpeaking)
                {
                    characterNameText.text = p_characterDatas[i].character.name;

                }

            }
        }
        else
        {
            characterNameText.text = "NO CHARACTER ASSIGNED";
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
                    if (newList[i] == oldList[x])//matching
                    {
                        break;
                    }
                    x++;
                    if (x >= oldList.Count)//new
                    {
                        charactersToBeAdded.Add(newList[i]);
                    }
                }
            }
            else
            {
                charactersToBeAdded.Add(newList[i]);
            }

        }

    }
    void IdentifyCharactersToRemove(List<SO_Character> oldList, List<SO_Character> newList, List<SO_Character> charactersToBeRemoved)
    {
        //Mark the Characters to Remove
        for (int oldIndex = 0; oldIndex < oldList.Count; oldIndex++)
        {
            if (newList.Count > 0)
            {
                for (int currentIndex = 0; currentIndex < newList.Count;)
                {
                    if (oldList[oldIndex] == newList[currentIndex]) //If Old Element in Old List Matches New Element in New List, it means it stays
                    {
                        break;
                    }

                    currentIndex++;
                    if (currentIndex >= newList.Count)
                    {
                        if (charactersToBeRemoved.Count > 0)
                        {
                            for (int removedIndex = 0; removedIndex < charactersToBeRemoved.Count;)
                            {
                                if (charactersToBeRemoved[removedIndex] == oldList[oldIndex])
                                {
                                    //It already is marked to be removed
                                    break;
                                }
                                removedIndex++;
                                if (currentIndex >= charactersToBeRemoved.Count)
                                {
                                    //It has not been already marked to be removed
                                    charactersToBeRemoved.Add(oldList[oldIndex]);
                                }


                            }
                        }
                        else
                        {
                            charactersToBeRemoved.Add(oldList[oldIndex]);
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
        for (int i = 0; i < savedCharacterUIs.Count; i++)
        {
            oldCharacters.Add(savedCharacterUIs[i].so_Character);
            //Debug.Log("saved: " + characterPresetDatas[i].name);
        }
        //Mark the Characters to Add and Characters that Exists
        IdentifyCharactersToAdd(newCharacters, oldCharacters, charactersToBeAdded);
        //Mark the Characters to Remove
        IdentifyCharactersToRemove(oldCharacters, newCharacters, charactersToBeRemoved);
        for (int i = 0; i < charactersToBeAdded.Count; i++)
        {

           // Debug.Log("ADD: " + charactersToBeAdded[i].name);
        }
        for (int i = 0; i < charactersToBeRemoved.Count; i++)
        {
           // Debug.Log("REMOVE: " + charactersToBeRemoved[i].name);
        }

        RemoveAvatar(charactersToBeRemoved);
        AddAvatar(charactersToBeAdded);
    }

    CharacterUI FindPreset(SO_Character p_so_Character)
    {
       
        for (int x = 0; x < savedCharacterUIs.Count; x++)
        {
            if (savedCharacterUIs[x].so_Character == p_so_Character)
            {
                return savedCharacterUIs[x];
                   
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
        hapticText.text = p_characterDatas.hapticType.ToString();
        vocalicText.text = p_characterDatas.vocalicType.ToString();
        kinesicText.text = p_characterDatas.kinesicType.ToString();
        oculesicText.text = p_characterDatas.oculesicType.ToString();
        physicalAppearanceText.text = p_characterDatas.physicalApperanceType.ToString();
    }

    public void CheckIfReady()
    {
        if (runningCoroutines == 0)
        {
            Debug.Log("READYING");
            isSkipping = true;
        }
    }
    public void OnNextButtonUIPressed()
    {
        
        if (currentDialogueIndex < currentSO_Dialogues.dialogues.Count)
        {
            Debug.Log("BUTTON PRESSED " + currentDialogueIndex + " RC: "+ runningCoroutines 
                + " iS: " + isSkipping
                + " iR: ");
            Dialogue currentDialogue = currentSO_Dialogues.dialogues[currentDialogueIndex];
            
            if (runningCoroutines > 0 && !isSkipping)
            {
                isSkipping = true;
                StopAllCoroutines();
                runningCoroutines=0;
                Debug.Log("READYING");
            
            }
          
            else if (isSkipping)// && !isReady)
            {
                Debug.Log("READIED");
                frame.SetActive(true);
                NextDialogue();
                OnNextButtonUIPressed();
                return;

            }
           
            CheckCachedCharacters(currentDialogue.characterDatas); //Rename and chop things into functions
            for (int i = 0; i < currentDialogue.characterDatas.Count; i++)
            {
                //Set Character UI Rect Transform
                //Position
                SetRectTransformToPreset(currentDialogue.characterDatas[i].characterPosition, currentDialogue.characterDatas[i].character);
                SetAvatarFlipOrientation(currentDialogue.characterDatas[i]);
                SetSpeakerTint(currentDialogue.characterDatas[i].isSpeaking, currentDialogue.characterDatas[i].character);
            }

            SetSpeakerName(currentDialogue.characterDatas);
            SetBackground(currentDialogue.backgroundSprite);
            SetSpeech(currentDialogue.speechTransitionType, currentDialogue.words);
            SetCueBank(currentDialogue);
        }
        //else if (currentDialogueIndex >= currentSO_Dialogues.dialogues.Count)
        //{
        //    if (!isAlreadyEnded)
        //    {
        //        if (currentSO_Dialogues.choiceDatas.Count > 1)
        //        {
        //            CreateChoiceUIs();
        //        }
        //        else if (currentSO_Dialogues.choiceDatas.Count == 1)
        //        {
        //            if (isEndTransitionEnabled)
        //            {
        //                TransitionUI.onFadeInAndOutTransition.Invoke(1, 0.25f, 1, 0, 0.25f, OnCloseCharacterDialogueUI);
        //            }
        //            else
        //            {
        //                OnCloseCharacterDialogueUI();
        //            }
        //        }
        //        else
        //        {
        //            ResetCharacterDialogueUI();
        //        }
        //    }
        //}
    }
}
