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
    //COROUTINE ON EACH INDEPENDENT SCRIPT
    [HeaderAttribute("REQUIRED COMPONENTS")]
    [SerializeField] private GameObject frame;


    [SerializeField] public GameObject nextDialogueButton;


    [SerializeField]
    public static List<Character> savedCharacters = new List<Character>();

    public int runningCoroutines = 0;
    public static bool isSkipping = false;
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

    public static System.Action OnAddNewTransitionEvent;
    public static System.Action OnFinishTransitionEvent;
    public static System.Action OnCheckIfSkippableEvent;

    public static System.Action OnResettingCharacterUIEvent;

    public static System.Action OnIsSkipping;

    public static System.Action OnInspectingEvent;
    public static System.Action OnDeinspectingEvent;
    [SerializeField]
    private SpeakerDialogueUI speakerDialogueUI;

    [SerializeField]
    private InputNameUI inputNameUI;
    public bool rarara = false;

    SO_Dialogues p_currentChoiceDataTest = null;

    bool popUp = false; 

    private void Awake()
    {
        onCharacterSpokenTo.AddListener(OnCharacterSpokenTo);
        //EVENTS
        OnStartChooseChoiceEvent += DisableNextDialogueButton;
        OnEndChooseChoiceEvent += ResetCharacterDialogueUI;
        OnPopUpEvent += popuptest;
        OnContinueEvent += ContinueEvent;
        OnEndEvent += ToggleNextDialogueButton;

        OnAddNewTransitionEvent += AddNewTransitionEvent;
        OnFinishTransitionEvent += FinishTransitionEvent;
        OnCheckIfSkippableEvent += CheckIfReady;


    }

    void AddNewTransitionEvent()
    {
        runningCoroutines++;
    }

    void FinishTransitionEvent()
    {
        runningCoroutines--;
        CheckIfReady();
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
        popUp = true;
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
            OnIsSkipping.Invoke();
            runningCoroutines = 0;
            // Debug.Log("READYING");

        }
        if (isStartTransitionEnabled)
        {
            TransitionUI.onFadeInAndOutTransition.Invoke(1, 0.5f, 1, 0, 0.5f,true, OnOpenCharacterDialogueUI, tatata);
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
        StorylineManager.loggedWords.Clear();
        Debug.Log("ENDED FUCK");
        SceneManager.LoadScene("FindR");
        frame.SetActive(false);
    }
    public void SkipAll()
    {
        if (StorylineManager.currentSO_Dialogues != null)
        {
            if (StorylineManager.currentSO_Dialogues.dialogues.Count > 0)
            {
                while (StorylineManager.currentDialogueIndex < StorylineManager.currentSO_Dialogues.dialogues.Count - 2)
                {
                    // yield return new WaitForSeconds(0.5f);
                    OnNextButtonUIPressed();

                    //Debug.Log(currentDialogueIndex + "/" + currentSO_Dialogues.dialogues.Count);
                    //currentDialogueIndex++;
                }

                Debug.Log("Auto skipped " + StorylineManager.currentDialogueIndex);
                //Debug.Log("Ended " + currentDialogueIndex);
            }
        }
   


    }

    void ResetCharacterDialogueUI()
    {
        //StorylineManager.currentDialogueIndex = 0;
        isSkipping = false;
        runningCoroutines = 0;
        isAlreadyEnded = false;
        nextDialogueButton.SetActive(true);

        popUp = false;
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
            Debug.Log(StorylineManager.currentDialogueIndex + " NEXT DIALOGUE " + StorylineManager.currentSO_Dialogues.dialogues[StorylineManager.currentDialogueIndex].words + " - " + isSkipping);
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

        CharactersUI.onRemoveCharactersEvent.Invoke(charactersToBeRemoved);
        CharactersUI.onAddCharactersEvent.Invoke(charactersToBeAdded);
    }

    void CheckIfReady()
    {
        if (runningCoroutines <= 0)
        {
            isSkipping = true;
            OnIsSkipping.Invoke();
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
            else
            {
                TransitionUI.instance.color = Color.black;
              
            }
            TransitionUI.onFadeInAndOutTransition.Invoke(1, 0.25f, 1, 0, 0.25f, false, p_postAction: tatata);
        }
        else if (p_specificEventType == SpecificEventType.fadeInEffect)
        {
            Debug.Log("HMM");
            //rarara = false;
            if (p_eventName == "white")
            {
                TransitionUI.instance.color = Color.white;
          
            }
            else 
            {
                TransitionUI.instance.color = Color.black;
           
            }
            TransitionUI.onFadeTransition.Invoke(1, false, false);
        }
        else if (p_specificEventType == SpecificEventType.fadeOutEffect)
        {
            //rarara = false;
            if (p_eventName == "white")
            {
                TransitionUI.instance.color = Color.white;
            }
            else
            {
                TransitionUI.instance.color = Color.black;
            }
            TransitionUI.onFadeTransition.Invoke(0, false, false);
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
        else if (p_specificEventType == SpecificEventType.shakeEffect)
        {
            CameraManager.instance.ShakeCamera();
        }
    }

    public void OnNextButtonUIPressed()
    {
        if (popUp)
        {
            Debug.Log("IT SHUD BE CLOSING");
            OnResettingCharacterUIEvent.Invoke();
            popUp = false;
        }
        else
        {

        }
        if (p_currentChoiceDataTest != null)
        {
            StorylineManager.currentSO_Dialogues = p_currentChoiceDataTest;
            p_currentChoiceDataTest = null;
            CharacterDialogueUI.OnEndChooseChoiceEvent.Invoke();
            
            return;

        }
        if (StorylineManager.currentSO_Dialogues == null && StorylineManager.paused)
        {
 
            return;
        }
        else if (StorylineManager.currentSO_Dialogues == null && !StorylineManager.paused)
        {
            OnCloseCharacterDialogueUI();
            return;
        }
       
        //Debug.Log("22222222");
        if (StorylineManager.currentDialogueIndex < StorylineManager.currentSO_Dialogues.dialogues.Count) // fix this for condition above
        {
            //Debug.Log("BUTTON PRESSED " + currentDialogueIndex + " RC: "+ runningCoroutines 
            //     + " iS: " + isSkipping
            //    + " iR: ");
            //Debug.Log(StorylineManager.currentBackgroundMusic);
            
            Dialogue currentDialogue = StorylineManager.currentSO_Dialogues.dialogues[StorylineManager.currentDialogueIndex];
            if (!string.IsNullOrEmpty(currentDialogue.backgroundMusic))
            {
                if (currentDialogue.backgroundMusic.ToLower() != "error")
                {
                    if (StorylineManager.currentBackgroundMusic.ToLower() != currentDialogue.backgroundMusic.ToLower())
                    {
                        if (currentDialogue.backgroundMusic.ToLower() != "stop")
                        {
                            if (StorylineManager.currentBackgroundMusic != "")
                            {
                                AudioManager.instance.SmoothPlayAudio(StorylineManager.currentBackgroundMusic, currentDialogue.backgroundMusic, false);
                            }
                            else
                            {
                                AudioManager.instance.AdditivePlayAudio(currentDialogue.backgroundMusic, false);
                            }

                            StorylineManager.currentBackgroundMusic = currentDialogue.backgroundMusic;
                        }
                        else
                        {
                            AudioManager.instance.SmoothStopAudio(StorylineManager.currentBackgroundMusic, false);
                            StorylineManager.currentBackgroundMusic = "";
                        }
                    }
                   
                }
            } 
           
            if (currentDialogue.specificEventType != SpecificEventType.none)
            {
                DoSpecificEvent(currentDialogue.specificEventType, currentDialogue.specificEventParameter);
            
            }
            if (runningCoroutines > 0 && !isSkipping)
            {
                isSkipping = true;
                OnIsSkipping.Invoke();
                StopAllCoroutines();
                OnIsSkipping.Invoke();
                runningCoroutines =0;
                //Debug.Log("READYING");
                return;
            
            }
          
            else if (isSkipping)// && !isReady)
            {
                //Debug.Log("READIED");
                frame.SetActive(true);
                onNewDialogueEvent.Invoke(currentDialogue);
                StorylineManager.loggedWords.Add(currentDialogue);
                SetNextDialogue();
                OnNextButtonUIPressed();
                return;

            }

            if (rarara)
            {
                CheckCachedCharacters(currentDialogue.characterDatas); //Rename and chop things into functions
                CharactersUI.onUpdateCharacterDatasEvent(currentDialogue.characterDatas);
              
               
                speakerDialogueUI.SetSpeakerName(currentDialogue.characterDatas);

                speakerDialogueUI.SetSpeech(currentDialogue.words);
            }
            BackgroundUI.onSetBackgroundEvent.Invoke(currentDialogue.backgroundSprite);
     

        }
        else if (StorylineManager.currentDialogueIndex >= StorylineManager.currentSO_Dialogues.dialogues.Count)
        {
            EndOfDialogue();
        }
    }

    void EndOfDialogue()
    {
        if (!isAlreadyEnded)
        {
            if (p_currentChoiceDataTest == null)
            {
                if (StorylineManager.sideDialogue)
                {
                    Debug.Log("IT ENDED SIDE DIALOGUE");
                    StorylineManager.sideDialogue = false;
                    StorylineManager.currentSO_Dialogues = StorylineManager.savedSO_Dialogues;
                    StorylineManager.currentDialogueIndex = StorylineManager.savedDialogueIndex;

                }
                else
                {
                    if (StorylineManager.currentSO_Dialogues.choiceDatas.Count > 1)
                    {
                        if (StorylineManager.currentSO_Dialogues.choiceDatas[0].isAutomaticEnabledColumnPattern)
                        {
                            //No choice, just evaluate
                            for (int i = 0; i < StorylineManager.currentSO_Dialogues.choiceDatas.Count; i++)
                            {

                                if (HealthUI.myDelegate.Invoke(StorylineManager.currentSO_Dialogues.choiceDatas[i].healthCeilingCondition, StorylineManager.currentSO_Dialogues.choiceDatas[i].healthFloorCondition))
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
                            ChoicesUI.OnChoosingChoiceEvent(StorylineManager.currentSO_Dialogues.choiceDatas);
 

                        }


                    }
                    else if (StorylineManager.currentSO_Dialogues.choiceDatas.Count == 1)
                    {
                        if (StorylineManager.currentSO_Dialogues.choiceDatas[0].isImmediateGoPhone)
                        {
                            if (!string.IsNullOrEmpty(StorylineManager.currentSO_Dialogues.choiceDatas[0].effectID))
                            {
                                StorylineManager.LoadPhone();
                            }
                        }
                        else
                        {
                            Debug.Log(StorylineManager.currentSO_Dialogues.choiceDatas[0].branchDialogue);
                            //Set Choice Damage
                            HealthUI.ModifyHealthEvent.Invoke(StorylineManager.currentSO_Dialogues.choiceDatas[0].healthModifier);
                            StorylineManager.currentSO_Dialogues = StorylineManager.currentSO_Dialogues.choiceDatas[0].branchDialogue;
                            StorylineManager.currentDialogueIndex = 0;
                        }
                    }
                    else if (StorylineManager.currentSO_Dialogues.choiceDatas.Count == 0)
                    {
                        if (isEndTransitionEnabled)
                        {
                            TransitionUI.onFadeInAndOutTransition.Invoke(1, 0.25f, 1, 0, 0.25f, true, p_postAction: OnCloseCharacterDialogueUI);
                            Debug.Log("0000000000000000000000000000000000");
                        }
                        else
                        {
                            OnCloseCharacterDialogueUI();
                        }
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
