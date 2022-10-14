using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class CharacterSpokenTo : UnityEvent<string> { }
public class NewDialogueEvent : UnityEvent<Dialogue> { }
public class FirstTimeFoodOnEndEvent : UnityEvent { }

[System.Serializable]
public class CharacterPresetData
{
    public CharacterPositionType characterPositionType;
    public RectTransform avatarRectTransform;
    public Transform avatarTransform;
}

public class CharacterDialogueUI : MonoBehaviour
{

    [HeaderAttribute("REQUIRED COMPONENTS")]
    [SerializeField] private GameObject frame;

    [SerializeField] private Image backgroundImage;

    [SerializeField] public GameObject nextDialogueButton;


    [SerializeField] private Transform characterUIContainerTransform;
    [SerializeField] private Transform characterObjectContainerTransform;


    [SerializeField]
    private List<Character> savedCharacters = new List<Character>();
    [SerializeField]
    private List<CharacterPresetData> characterPresetDatas = new List<CharacterPresetData>();

    [SerializeField] private CharacterUI staticCharacterPrefab;

    [HeaderAttribute("ADJUSTABLE VALUES")]

    [SerializeField] private Color32 nonSpeakerTintColor;


    [SerializeField]
    private float avatarFadeTime;
    [SerializeField]
    private float avatarDelayTime;

    public int runningCoroutines = 0;
    public bool isSkipping = false;
    private string id;

    bool isStartTransitionEnabled = true;
    bool isEndTransitionEnabled = true;

    bool isAdvancedonWorldEventEndedEvent = false;

    bool isAlreadyEnded = false;

    public static NewDialogueEvent onNewDialogueEvent = new NewDialogueEvent();

    public static CharacterSpokenTo onCharacterSpokenTo = new CharacterSpokenTo();
    public static System.Action OnStartChooseChoiceEvent;
    public static System.Action OnEndChooseChoiceEvent;


    public static System.Action OnContinueEvent;
    public static System.Action OnEndEvent;
    public static System.Action<SO_Dialogues> OnPopUpEvent;

    public static System.Action OnResettingVisualNovelUI;

    [SerializeField]
    private HealthUI healthUI;
    [SerializeField]
    private PopUpUI popUpUI;
    [SerializeField]
    private SpeakerDialogueUI speakerDialogueUI;
    [SerializeField]
    private CueBankUI cueBankUI;
    [SerializeField]
    private InputNameUI inputNameUI;
    public bool rarara = false;

    SO_Dialogues p_currentChoiceDataTest = null;


    private void Awake()
    {
        onCharacterSpokenTo.AddListener(OnCharacterSpokenTo);
        //EVENTS
        OnStartChooseChoiceEvent += DisableNextDialogueButton;
        OnEndChooseChoiceEvent += ResetCharacterDialogueUI;
        OnPopUpEvent += popuptest;
        OnContinueEvent += ContinueEvent;
        OnEndEvent += ToggleNextDialogueButton;


    }
    void ContinueEvent()
    {
        Debug.Log("CONTINUING");
        speakerDialogueUI.ManualToggleSpeakerDialogueUI(true);
        isSkipping = false;
        runningCoroutines = 0;
        nextDialogueButton.SetActive(true);
        OnNextButtonUIPressed();
    }
    private void OnEnable()
    {
        StartCoroutine(startload());
    }

    IEnumerator startload()
    {
        yield return new WaitForSeconds(1f);
        if (StorylineManager.currentSO_Dialogues != null)
        {
            onCharacterSpokenTo.Invoke("");
        }
    }
    void DisableNextDialogueButton()
    {
        nextDialogueButton.SetActive(false);
    }
    void ToggleNextDialogueButton()
    {
        nextDialogueButton.SetActive(true);
    }
    public void popuptest(SO_Dialogues p_currentChoiceData)
    {
        p_currentChoiceDataTest = p_currentChoiceData;
        nextDialogueButton.SetActive(true);
    }


    public void OnCharacterSpokenTo(string p_id)
    {
        id = p_id;
        rarara = false;
        p_currentChoiceDataTest = null;
        if (runningCoroutines > 0)
        {
            isSkipping = false;
            StopAllCoroutines();
            speakerDialogueUI.StopCoroutine();
            runningCoroutines = 0;
            // Debug.Log("READYING");

        }
        if (isStartTransitionEnabled)
        {
            TransitionUI.onFadeInAndOutTransition.Invoke(1, 0.5f, 1, 0, 0.5f, OnOpenCharacterDialogueUI, tatata);
            //Debug.Log("66666666666666666");
        }
        else
        {
            OnOpenCharacterDialogueUI();
        }

    }
    public void tatata()
    {
        rarara = true;
        speakerDialogueUI.ManualToggleSpeakerDialogueUI(true);
        //Debug.Log("11111111");
        OnNextButtonUIPressed();
    }

    public void OnOpenCharacterDialogueUI()
    {
        frame.SetActive(true);
        ResetCharacterDialogueUI();
    }

    public void OnCloseCharacterDialogueUI()
    {
        //Temporary

        SceneManager.LoadScene("FindR");
        frame.SetActive(false);
    }
    public void SkipAll()
    {

        while (StorylineManager.currentDialogueIndex < StorylineManager.currentSO_Dialogues.dialogues.Count-2)
        {
           // yield return new WaitForSeconds(0.5f);
            OnNextButtonUIPressed();

            //Debug.Log(currentDialogueIndex + "/" + currentSO_Dialogues.dialogues.Count);
            //currentDialogueIndex++;
        }
    
        Debug.Log("Auto skipped " + StorylineManager.currentDialogueIndex);
        //Debug.Log("Ended " + currentDialogueIndex);

    }

    void ResetCharacterDialogueUI()
    {
        StorylineManager.currentDialogueIndex = 0;
        isSkipping = false;
        runningCoroutines = 0;
        isAlreadyEnded = false;
        nextDialogueButton.SetActive(true);
        cueBankUI.ResetCueBankUI();
        popUpUI.CloseUI();
        
        OnNextButtonUIPressed();
        if (!rarara)
        {
            speakerDialogueUI.ResetSpeakerDialogueUI();
            //Debug.Log("00000000");
        }

    }

    void SetNextDialogue()
    {
        if (rarara)
        {
            Debug.Log(StorylineManager.currentDialogueIndex + " NEXT DIALOGUE " + StorylineManager.currentSO_Dialogues.dialogues[StorylineManager.currentDialogueIndex]);
            isSkipping = false;

            StorylineManager.currentDialogueIndex++;
            runningCoroutines = 0;
     
            if (StorylineManager.currentDialogueIndex == StorylineManager.currentSO_Dialogues.dialogues.Count)
            {
                if (isAdvancedonWorldEventEndedEvent)
                {
                    isAlreadyEnded = true;
                }
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

    IEnumerator AvatarFadeOut(Image p_avatarImage, Character p_newCharacter)
    {
        Debug.Log("FADING OUT");
        runningCoroutines++;
        var fadeOutSequence = DOTween.Sequence()
       .Append(p_avatarImage.DOFade(0, avatarFadeTime));
        fadeOutSequence.Play();
        yield return fadeOutSequence.WaitForCompletion();
        runningCoroutines--;
        savedCharacters.Remove(p_newCharacter);
        if (p_newCharacter.gameObject != null)
        {
            Destroy(p_newCharacter.gameObject);
        }
   
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

    void SetRectTransformToPreset(CharacterPositionType p_characterPositionType, SO_Character p_index)
    {
        Character foundCharacter = FindPreset(p_index);

        if (foundCharacter != null)
        {
           
            for (int i = 0; i < characterPresetDatas.Count; i++)
            {
                if (p_characterPositionType == characterPresetDatas[i].characterPositionType)
                {
                    if (foundCharacter is CharacterUI)
                    {
                        CharacterUI foundPreset = foundCharacter as CharacterUI;
                        foundPreset.avatarRectTransform.anchoredPosition = characterPresetDatas[i].avatarRectTransform.anchoredPosition;
                    }
                    else if (foundCharacter is CharacterObject)
                    {
                        CharacterObject foundPreset = foundCharacter as CharacterObject;
                        foundPreset.transform.position = characterPresetDatas[i].avatarTransform.position;
                    }
                }
            }  
        }
    }

    void SetSpeakerTint(bool p_isSpeaking, SO_Character p_index)
    {

        Character foundCharacter = FindPreset(p_index);

        if (foundCharacter != null)
        {
            if (foundCharacter is CharacterUI)
            {
                CharacterUI foundPreset = foundCharacter as CharacterUI;
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

    void SetFacialEmotion(CharacterData p_characterData)
    {
        Character foundCharacter = FindPreset(p_characterData.character);
        if (foundCharacter != null)
        {
            if (foundCharacter is CharacterObject)
            {
                if (p_characterData.faceEmotion != CharacterEmotionType.none)
                {
                    CharacterObject foundPreset = foundCharacter as CharacterObject;

                    for (int i = 0; i < foundPreset.so_Character.faceEmotionDatas.Count; i++)
                    {

                        if (foundPreset.so_Character.faceEmotionDatas[i].type == p_characterData.faceEmotion)
                        {
                            foundPreset.expressionController.CurrentExpressionIndex = foundPreset.so_Character.faceEmotionDatas[i].index;
                            break;
                        }
                    }
                }
            }
        }
    }

    void SetBodyEmotion(CharacterData p_characterData)
    {
        Character foundCharacter = FindPreset(p_characterData.character);
        if (foundCharacter != null)
        {
            if (foundCharacter is CharacterObject)
            {
      
                if (p_characterData.bodyEmotion != CharacterEmotionType.none)
                {
                    CharacterObject foundPreset = foundCharacter as CharacterObject;
                    for (int i =0; i < foundPreset.charAnim.parameters.Length; )
                    {
                        if (foundPreset.charAnim.parameters[i].name == p_characterData.bodyEmotion.ToString())
                        {
                            foundPreset.charAnim.SetTrigger(p_characterData.bodyEmotion.ToString());
                            break;
                        }
                        i++;
                        if (i >= foundPreset.charAnim.parameters.Length)
                        {
                            Debug.Log(StorylineManager.currentDialogueIndex + " Set Body Emotion is Not available");                        
                        }
                    }
                    
                }
           
            }
        }
    }

    void RemoveAvatar(List<SO_Character> p_charactersToBeRemoved)
    {
        //Debug.Log("-----REMOVING " + isSkipping);
        //Do functions to characters to be Added
        for (int i = 0; i < p_charactersToBeRemoved.Count; i++)
        {
            Character foundCharacter = FindPreset(p_charactersToBeRemoved[i]);

            if (foundCharacter != null)
            {
                if (foundCharacter is CharacterUI)
                {
                    CharacterUI foundPreset = foundCharacter as CharacterUI;
                    if (p_charactersToBeRemoved[i].avatar != null)
                    {
                        if (isSkipping)
                        {

                            // Debug.Log("REMOVINGRR " + foundPreset);
                            savedCharacters.Remove(foundPreset);
                            Destroy(foundPreset.gameObject);
                        }
                        else
                        {
                            StartCoroutine(AvatarFadeOut(foundPreset.avatarImage, foundPreset));
                        }
                    }
                    
                }
                else if (foundCharacter is CharacterObject)
                {

                    CharacterObject foundPreset = foundCharacter as CharacterObject;
                    savedCharacters.Remove(foundPreset);
                    Destroy(foundPreset.gameObject);
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
            for (int i = 0; i < savedCharacters.Count; i++)
            {
                if (savedCharacters[i] is CharacterUI)
                {
                    if (savedCharacters[i].so_Character.avatar != null)
                    {
                        CharacterUI currentCharacterUI = savedCharacters[i] as CharacterUI;
                        currentCharacterUI.avatarImage.color = new Color(1, 1, 1, 1);
                    }
                }
                
            }

        }
        else
        {
            for (int i = 0; i < p_charactersToBeAdded.Count; i++)
            {
                Character newCharacter = null;
               // Debug.Log("PRINT NAME: " + p_charactersToBeAdded[i].name);
                
                if (p_charactersToBeAdded[i].prefab != null) //Live 2D
                {
                  
                    newCharacter = Instantiate(p_charactersToBeAdded[i].prefab, characterObjectContainerTransform);

                }
                else //UI
                {
                    if (p_charactersToBeAdded[i].avatar != null)
                    {
                        newCharacter = Instantiate(staticCharacterPrefab, characterUIContainerTransform) ;
                        CharacterUI newCharacterUI = newCharacter as CharacterUI;
                        newCharacter.so_Character = p_charactersToBeAdded[i];
                   
                        StartCoroutine(AvatarFadeIn(newCharacterUI.avatarImage, p_charactersToBeAdded[i].avatar));
                    }
               

                }
                if (newCharacter != null)
                {
                    savedCharacters.Add(newCharacter);
                }
            
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

        Character foundCharacter = FindPreset(p_characterData.character);
        if (foundCharacter != null)
        {
            if (foundCharacter is CharacterUI)
            {
                CharacterUI foundPreset = foundCharacter as CharacterUI;
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
                    StartCoroutine(AvatarFlipSequence(foundPreset.avatarImage, foundPreset.avatarRectTransform, target));
                }
                
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
        for (int i = 0; i < savedCharacters.Count; i++)
        {
            oldCharacters.Add(savedCharacters[i].so_Character);
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

    Character FindPreset(SO_Character p_so_Character)
    {
       
        for (int x = 0; x < savedCharacters.Count; x++)
        {
            if (savedCharacters[x].so_Character == p_so_Character)
            {
                return savedCharacters[x];
                   
            }

        }
        return null;

      
    }

    public void CheckIfReady()
    {
        if (runningCoroutines <= 0)
        {
            Debug.Log("READYING");
            isSkipping = true;
        }
    }
    public void DoSpecificEvent(SpecificEventType p_specificEventType, string p_eventName)
    {
        if (p_specificEventType == SpecificEventType.soundEffect)
        {
            AudioManager.instance.AdditivePlayAudio(p_eventName);
        }
        else if (p_specificEventType == SpecificEventType.fadeInNOutEffect)
        {
            rarara = false;
            if (p_eventName == "white")
            {
                TransitionUI.instance.color = Color.white;

            }
            else if (p_eventName == "black")
            {
                TransitionUI.instance.color = Color.black;
              
            }
            TransitionUI.onFadeInAndOutTransition.Invoke(1, 0.25f, 1, 0, 0.25f, p_postAction: tatata);
        }
        else if (p_specificEventType == SpecificEventType.fadeInEffect)
        {
            //rarara = false;
            if (p_eventName == "white")
            {
                TransitionUI.instance.color = Color.white;
          
            }
            else if (p_eventName == "black")
            {
                TransitionUI.instance.color = Color.black;
           
            }
            TransitionUI.onFadeTransition.Invoke(1, false);
        }
        else if (p_specificEventType == SpecificEventType.fadeOutEffect)
        {
            //rarara = false;
            if (p_eventName == "white")
            {
                TransitionUI.instance.color = Color.white;
            }
            else if (p_eventName == "black")
            {
                TransitionUI.instance.color = Color.black;
            }
            TransitionUI.onFadeTransition.Invoke(0, false);
        }
        else if (p_specificEventType == SpecificEventType.inputNameEvent)
        {
            inputNameUI.ToggleUI();
            speakerDialogueUI.ResetSpeakerDialogueUI();
            nextDialogueButton.SetActive(false);
        }
        else if (p_specificEventType == SpecificEventType.phoneEvent)
        {
           //phone
        }
        else if (p_specificEventType == SpecificEventType.shakeEffects)
        {
            CameraManager.instance.ShakeCamera();
        }
    }

    public void OnNextButtonUIPressed()
    {
        if (p_currentChoiceDataTest != null)
        {
            StorylineManager.currentSO_Dialogues = p_currentChoiceDataTest;
            p_currentChoiceDataTest = null;
            CharacterDialogueUI.OnEndChooseChoiceEvent.Invoke();
          
            return;

        }
       
        //Debug.Log("22222222");
        if (StorylineManager.currentDialogueIndex < StorylineManager.currentSO_Dialogues.dialogues.Count)
        {
            //Debug.Log("BUTTON PRESSED " + currentDialogueIndex + " RC: "+ runningCoroutines 
           //     + " iS: " + isSkipping
            //    + " iR: ");
            Dialogue currentDialogue = StorylineManager.currentSO_Dialogues.dialogues[StorylineManager.currentDialogueIndex];
            if (currentDialogue.specificEventType != SpecificEventType.none)
            {
                DoSpecificEvent(currentDialogue.specificEventType, currentDialogue.specificEventParameter);
            
            }
            if (runningCoroutines > 0 && !isSkipping)
            {
                isSkipping = true;
                StopAllCoroutines();
                speakerDialogueUI.StopCoroutine();
                runningCoroutines =0;
               // Debug.Log("READYING");
            
            }
          
            else if (isSkipping)// && !isReady)
            {
                //Debug.Log("READIED");
                frame.SetActive(true);
                onNewDialogueEvent.Invoke(currentDialogue);
                SetNextDialogue();
                OnNextButtonUIPressed();
                return;

            }

            if (rarara)
            {
                CheckCachedCharacters(currentDialogue.characterDatas); //Rename and chop things into functions
                for (int i = 0; i < currentDialogue.characterDatas.Count; i++)
                {
                    //Set Character UI Rect Transform
                    //Position
                    SetRectTransformToPreset(currentDialogue.characterDatas[i].characterPosition, currentDialogue.characterDatas[i].character);
                    SetAvatarFlipOrientation(currentDialogue.characterDatas[i]);
                    SetFacialEmotion(currentDialogue.characterDatas[i]);
                    SetBodyEmotion(currentDialogue.characterDatas[i]);
                    SetSpeakerTint(currentDialogue.characterDatas[i].isSpeaking, currentDialogue.characterDatas[i].character);
                }
               
                speakerDialogueUI.SetSpeakerName(currentDialogue.characterDatas);
                cueBankUI.cueBankOpenable = false;
                speakerDialogueUI.SetSpeech(currentDialogue.words);
            }
           
            SetBackground(currentDialogue.backgroundSprite);

        }
        else if (StorylineManager.currentDialogueIndex >= StorylineManager.currentSO_Dialogues.dialogues.Count)
        {
            if (!isAlreadyEnded)
            {
                if (p_currentChoiceDataTest == null)
                {
                    if (StorylineManager.currentSO_Dialogues.choiceDatas.Count > 1)
                    {
                        if (StorylineManager.currentSO_Dialogues.isAutomaticHealthEvaluation)
                        {
                            //No choice, just evaluate
                            for (int i = 0; i < StorylineManager.currentSO_Dialogues.choiceDatas.Count; i++)
                            {
                                if (healthUI.currentHealth <= StorylineManager.currentSO_Dialogues.choiceDatas[i].healthCeilingCondition &&
                                    healthUI.currentHealth > StorylineManager.currentSO_Dialogues.choiceDatas[i].healthFloorCondition)
                                {
                                    StorylineManager.currentSO_Dialogues = StorylineManager.currentSO_Dialogues.choiceDatas[i].branchDialogue;
                                    StorylineManager.currentDialogueIndex = 0;
                                    break;
                                }
                              
                            }

                        }
                        else
                        {
                            //Creating Choices
                            Debug.Log("CREATING CHOIIIIIIIIICE");
                            nextDialogueButton.SetActive(false);
                            ChoiceManager.OnChoosingChoiceEvent(healthUI,StorylineManager.currentSO_Dialogues.choiceDatas);
                            cueBankUI.SetCueBank(StorylineManager.currentSO_Dialogues);

                        }


                    }
                    else if (StorylineManager.currentSO_Dialogues.choiceDatas.Count == 1)
                    {
                        Debug.Log(StorylineManager.currentSO_Dialogues.choiceDatas[0].branchDialogue);

             
                        //Set Choice Damage
                        HealthUI.ModifyHealthEvent.Invoke(StorylineManager.currentSO_Dialogues.choiceDatas[0].healthModifier);
                        StorylineManager.currentSO_Dialogues = StorylineManager.currentSO_Dialogues.choiceDatas[0].branchDialogue;
                        StorylineManager.currentDialogueIndex = 0;
                    }
                    else if (StorylineManager.currentSO_Dialogues.choiceDatas.Count == 0)
                    {
                        if (isEndTransitionEnabled)
                        {
                            TransitionUI.onFadeInAndOutTransition.Invoke(1, 0.25f, 1, 0, 0.25f, p_postAction: OnCloseCharacterDialogueUI);
                            Debug.Log("0000000000000000000000000000000000");
                        }
                        else
                        {
                            OnCloseCharacterDialogueUI();
                        }
                    }
                }
                else if (p_currentChoiceDataTest != null)
                {
                    StorylineManager.currentSO_Dialogues = p_currentChoiceDataTest;
                }
            }
        }
    }

}
