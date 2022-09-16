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

[System.Serializable]
public class CharacterPresetData
{
    public RectTransform avatarRectTransform;
    [HideInInspector] public Image avatarImage;

    public RectTransform emoteBubbleStartRectTransform;

    public RectTransform emoteBubbleEndRectTransform;


    public void Initialize()
    {
        avatarImage = avatarRectTransform.GetComponent<Image>();
    }
}

public class CharacterDialogueUI : MonoBehaviour
{
    [HeaderAttribute("REQUIRED COMPONENTS")]
    [SerializeField] private GameObject frame;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private TMP_Text dialogueText;

    [SerializeField] private Image backgroundImage;

    private GameObject emoticonObject;
    private Image emoticonImage;
    private RectTransform emoticonRectTransform;
    [SerializeField] private Image emoticonBubbleImage;
    private RectTransform emoticonBubbleRectTransform;

    [SerializeField] private Animator emoticonAnim;

    [SerializeField] public GameObject nextDialogueButton;
    [SerializeField] private GameObject choiceUIsContainer;
    private Transform choiceUIsContainerTransform;
    private RectTransform choiceUIsContainerRectTransform;
    [SerializeField] private ChoiceUI choiceUIPrefab;

    [SerializeField]
    private List<CharacterPresetData> characterPresetDatas = new List<CharacterPresetData>();
    [SerializeField] private List<Image> charactersAvatarImage = new List<Image>();

    [SerializeField] private RectTransform leftCharacterPresetRectTransform;
    [SerializeField] private RectTransform centerCharacterPresetRectTransform;
    [SerializeField] private RectTransform rightCharacterPresetRectTransform;

    [HeaderAttribute("ADJUSTABLE VALUES")]

    [SerializeField] private Color32 nonSpeakerTintColor;
    [SerializeField] private float typewriterSpeed = 0.1f;

    [SerializeField]
    private float avatarFadeTime;
    [SerializeField]
    private float avatarDelayTime;

    [SerializeField]
    private float emoticonFadeTime;
    [SerializeField] private RectTransform defaultEmoticonSize;
    [SerializeField] private RectTransform targetEmoticonSize;
    [SerializeField] private float emoticonSizeTime;

    [SerializeField]
    private float emoticonBubbleFadeTime;
    [SerializeField] private RectTransform defaultEmoticonBubbleSize;
    [SerializeField] private RectTransform targetEmoticonBubbleSize;
    [SerializeField] private float emoticonBubbleSizeTime;


    [HideInInspector]
    public SO_Character character;

    [HideInInspector]
    public SO_Dialogues currentSO_Dialogues;



    private int currentDialogueIndex;

    private bool isPaused = false;
    private bool allowNext;

    private string id;

    bool isStartTransitionEnabled = true;
    bool isEndTransitionEnabled = true;

    bool isAdvancedonWorldEventEndedEvent = false;

    bool isAlreadyEnded = false;
    //bool firstTime = true;
    Sprite cachedSprite;
    int lastCharacterData;

    bool firstfirstTimeTutorial = false;
    public bool firstTimeTutorial = false;
    public bool firstTimeTut = false;

    public IEnumerator runningCoroutine;
    public IEnumerator runningEmotionCoroutine;
    public IEnumerator runningAvatarCoroutine;
    public static CharacterSpokenToEvent onCharacterSpokenToEvent = new CharacterSpokenToEvent();
    public static FirstTimeFoodOnEndEvent onFirstTimeFoodOnEndEvent = new FirstTimeFoodOnEndEvent();
    bool canDo = false;

   [SerializeField] private bool isFoodFirstTime = false;
    //public void Skip()
    //{
    //    firstfirstTimeTutorial = true;
    //}
    //void FirstTimeFoodOnEndEvent()
    //{
    //    isFoodFirstTime = true;
    //}
    private void Awake()
    {
        emoticonRectTransform = emoticonAnim.GetComponent<RectTransform>();
        emoticonObject = emoticonAnim.gameObject;
        emoticonImage = emoticonAnim.GetComponent<Image>();
        //onFirstTimeFoodOnEndEvent.AddListener(FirstTimeFoodOnEndEvent);
        emoticonBubbleRectTransform = emoticonBubbleImage.GetComponent<RectTransform>();

        firstfirstTimeTutorial = false;
        isFoodFirstTime = false;
        onCharacterSpokenToEvent.AddListener(OnCharacterSpokenTo);

        choiceUIsContainerTransform = choiceUIsContainer.transform;
        choiceUIsContainerRectTransform = choiceUIsContainer.GetComponent<RectTransform>();

        for (int i =0; i< characterPresetDatas.Count; i++)
        {
            characterPresetDatas[i].Initialize();
        }
        
        //Panday.onPandaySpokenToEvent.AddListener(UniqueGameplayModeChangedEvent);
        //UIManager.onGameplayModeChangedEvent.AddListener(GameplayModeChangedEvent);

    }

    public void OnCharacterSpokenTo(string p_id, SO_Dialogues p_SO_Dialogue)
    {
        id = p_id;
        currentSO_Dialogues = p_SO_Dialogue;
        // Debug.Log(id + " EVENT WITH NAME " + currentSO_Dialogues.name + " IS CURRENT DIALOGUE " + " IS CHARACTERSPOKENTO CALLED");

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
        //PlayerManager.instance.canPressPanel = false;
 
        //PlayerJoystick.onUpdateJoystickEnabledEvent.Invoke(false);
        //PlayerManager.instance.playerMovement.enabled = false;
        //PlayerManager.instance.playerMovement.joystick.gameObject.SetActive(false);
        canDo = true;
        // Debug.Log("WHO IS CALLING");
        frame.SetActive(true);
        //Debug.Log(id + " EVENT WITH NAME " + currentSO_Dialogues.name + " IS CURRENT DIALOGUE " + " OPENING");
        ResetCharacterDialogueUI();
        //TimeManager.onPauseGameTime.Invoke(false);

        //UIManager.onGameplayModeChangedEvent.Invoke(true);
    }

    public void OnCloseCharacterDialogueUI()
    {

        

     
        frame.SetActive(false);

        //UIManager.onGameplayModeChangedEvent.Invoke(false);

    }

    public IEnumerator Co_TypeWriterEffect(TMP_Text p_textUI, string p_fullText)
    {
        string p_currentText;
        for (int i = 0; i <= p_fullText.Length; i++)
        {
            p_currentText = p_fullText.Substring(0, i);
            p_textUI.text = p_currentText;
            yield return new WaitForSeconds(typewriterSpeed);
        }
    }


    void ResetEmotionUI()
    {

        emoticonImage.color = new Color(emoticonImage.color.r, emoticonImage.color.g, emoticonImage.color.b, 0);

        emoticonRectTransform.sizeDelta = defaultEmoticonSize.sizeDelta;

        emoticonObject.SetActive(false);

        emoticonBubbleImage.color = new Color(emoticonBubbleImage.color.r, emoticonBubbleImage.color.g, emoticonBubbleImage.color.b, 0);

        emoticonBubbleRectTransform.sizeDelta = defaultEmoticonBubbleSize.sizeDelta;
    }

    void ResetCharacterDialogueUI()
    {
        cachedSprite = null;
        //  firstTime = true;
        currentDialogueIndex = 0;
        allowNext = false;
        isAlreadyEnded = false;
        nextDialogueButton.SetActive(true);
        choiceUIsContainer.SetActive(false);
        // Debug.Log("WHO IS CALLING");

        ResetAllCharacterAvatarImage();



        ResetEmotionUI();
        OnNextButtonUIPressed();
    }

    void ResetAllCharacterAvatarImage()
    {
        for (int i = 0; i < charactersAvatarImage.Count; i++)
        {
            charactersAvatarImage[i].color = new Color(charactersAvatarImage[i].color.r, charactersAvatarImage[i].color.g, charactersAvatarImage[i].color.b, 0);

        }
    }
    void NextDialogue()
    {
        Debug.Log("NEXT DIALOGUE");
        currentDialogueIndex++;
        //ResetEmotionUI();
        //  Debug.Log("WHO IS CALLING");

        if (currentDialogueIndex == currentSO_Dialogues.dialogues.Count)
        {
            if (isAdvancedonWorldEventEndedEvent)
            {
                //event
                //int currentQuestChainIndex = storylineData.currentQuestChainIndex;
                //int currentQuestLineIndex = storylineData.currentQuestLineIndex;
                //StorylineManager.onWorldEventEndedEvent.Invoke(id, currentQuestChainIndex, currentQuestLineIndex);
                //Debug.Log("ADVANCE CALLING");
                isAlreadyEnded = true;


            }
        }
    }

    public IEnumerator AvatarFade(Image p_avatarImage,Sprite p_sprite)
    {
        var sequence = DOTween.Sequence()
       .Append(p_avatarImage.DOFade(0, avatarFadeTime));
        sequence.Play();

        yield return sequence.WaitForCompletion();
        p_avatarImage.sprite = p_sprite;
        var sequence2 = DOTween.Sequence()
        .Append(p_avatarImage.DOFade(1, avatarFadeTime));
        sequence2.Play();

        yield return sequence2.WaitForCompletion();
    }
    public void CreateChoiceUIs()
    {
        
        nextDialogueButton.SetActive(false);
        choiceUIsContainer.SetActive(true);
        for (int i =0; i <currentSO_Dialogues.so_Choices.Count; i++)
        {
            ChoiceUI newChoiceUI = Instantiate(choiceUIPrefab, choiceUIsContainerTransform);
            newChoiceUI.InitializeValues(currentSO_Dialogues.so_Choices[i].choiceName, "");
            //newChoiceUI.transform.SetParent(choiceUIsContainer.transform);
            int index = i;
            newChoiceUI.GetComponent<Button>().onClick.AddListener(delegate { ChooseChoiceUI(index); });
            LayoutRebuilder.ForceRebuildLayoutImmediate(choiceUIsContainerRectTransform);
           
        }

        
    }

    public void ChooseChoiceUI(int index)
    {
        nextDialogueButton.SetActive(true);
        choiceUIsContainer.SetActive(false);
        currentSO_Dialogues = currentSO_Dialogues.so_Choices[index].so_choiceBranchDialogue;
        ResetCharacterDialogueUI();
    }

    public void OnNextButtonUIPressed()
    {
        Debug.Log("Next Button UI Clicked");
        if (runningEmotionCoroutine != null)
        {
            StopCoroutine(runningEmotionCoroutine);
            runningEmotionCoroutine = null;

            runningEmotionCoroutine = Co_EmotionOut(lastCharacterData);
            StartCoroutine(runningEmotionCoroutine);
        }

        if (currentDialogueIndex < currentSO_Dialogues.dialogues.Count)
        {

            Dialogue currentDialogue = currentSO_Dialogues.dialogues[currentDialogueIndex];

            int currentSpeakingCharacterIndex = -1;
            if (currentDialogue.characterDatas.Count < charactersAvatarImage.Count)
            {
                ResetAllCharacterAvatarImage();
                
            }
            for (int i = 0; i < currentDialogue.characterDatas.Count;i++)
            {
            
                //Set Character UI Rect Transform
                //Position
                if (currentDialogue.characterDatas[i].characterPosition == CharacterPositionType.left)
                {
                    charactersAvatarImage[i].GetComponent<RectTransform>().position = leftCharacterPresetRectTransform.position;

                }
                else if (currentDialogue.characterDatas[i].characterPosition == CharacterPositionType.center)
                {
                    charactersAvatarImage[i].GetComponent<RectTransform>().position = centerCharacterPresetRectTransform.position;

                }
                else if (currentDialogue.characterDatas[i].characterPosition == CharacterPositionType.right)
                {
                    charactersAvatarImage[i].GetComponent<RectTransform>().position = rightCharacterPresetRectTransform.position;

                }
                //Scale

                //Tint
                if (currentDialogue.characterDatas[i].isSpeaking)
                {
                    currentSpeakingCharacterIndex = i;
               
                    charactersAvatarImage[i].color = new Color(255,255,255);
                }
                else
                {
                    charactersAvatarImage[i].color = nonSpeakerTintColor;
                }
                characterNameText.text = currentDialogue.characterDatas[i].character.name;
                if (currentDialogue.backgroundSprite != null)
                {
                    backgroundImage.sprite = currentDialogue.backgroundSprite;
                    backgroundImage.color = new Color32(255, 255, 255, 255);
                }
                else if (currentDialogue.backgroundSprite == null)
                {
                    backgroundImage.color = new Color32(0, 0, 0, 0);
                }
                //if (cachedSprite != currentDialogue.characterDatas[i].character.avatar)
                //{

              
                    //if (runningAvatarCoroutine != null)
                    //{
                    //    StopCoroutine(runningAvatarCoroutine);
                    //    runningAvatarCoroutine = null;
                    //}
         
                    runningAvatarCoroutine = AvatarFade(charactersAvatarImage[i], currentDialogue.characterDatas[i].character.avatar);
                    StartCoroutine(runningAvatarCoroutine);
                    cachedSprite = currentDialogue.characterDatas[i].character.avatar;
               // }

                CharacterData currentSpeakingCharacter = null;
                if (currentSpeakingCharacterIndex != -1)
                {
                    currentSpeakingCharacter = currentSpeakingCharacter = currentDialogue.characterDatas[currentSpeakingCharacterIndex];
                }

                if (currentSpeakingCharacter != null)
                {
                    if ((int)currentSpeakingCharacter.emotion == 10)
                    {
                        emoticonObject.SetActive(false);
                        emoticonAnim.SetInteger("enum", (int)(int)currentSpeakingCharacter.emotion);

                     

                    }
                    else
                    {
                        lastCharacterData = currentSpeakingCharacterIndex;
                        runningEmotionCoroutine = Co_EmotionIn(charactersAvatarImage[currentSpeakingCharacterIndex], currentDialogue.characterDatas[currentSpeakingCharacterIndex].character.avatar, currentSpeakingCharacterIndex, currentDialogue.characterDatas[currentSpeakingCharacterIndex].emotion);
                        StartCoroutine(runningEmotionCoroutine);
                    }
                  
                }
            }
          
            if (runningCoroutine != null)
            {
                StopCoroutine(runningCoroutine);
                runningCoroutine = null;
            }
            if (currentDialogue.speechTransitionType == SpeechTransitionType.Typewriter)
            {
                if (runningCoroutine == null)
                {

                    dialogueText.text = currentDialogue.words;

                    if (allowNext == true)
                    {
                        NextDialogue();
                    }

                }
                else
                {

                    if (runningCoroutine != null)
                    {
                        StopCoroutine(runningCoroutine);
                        runningCoroutine = null;
                    }
                    runningCoroutine = Co_TypeWriterEffect(dialogueText, currentDialogue.words);
                    StartCoroutine(runningCoroutine);
                }

            }
            else
            {
                dialogueText.text = currentDialogue.words;

                if (allowNext == true)
                {
                    NextDialogue();
                }
            }

            if (allowNext == false) //check this ITS HAPPENS FIRST TIME
            {
                frame.SetActive(true);
                allowNext = true;
                NextDialogue();

            }

        }
        else if (currentDialogueIndex >= currentSO_Dialogues.dialogues.Count)        //TEMPORARY END CONVO, BUT EVENTUALLY SHOW AND GIVE QUEST
        {
            if (canDo)
            {
                canDo = false;
                if (!isAlreadyEnded)
                {
                    //Debug.Log("OUTSIDE");

                    if (!isAdvancedonWorldEventEndedEvent)
                    {
                        //Debug.Log("NORMAL");
                        //event
                        //StorylineData storylineData = StorylineManager.GetStorylineDataFromID(id);
                        //int currentQuestChainIndex = storylineData.currentQuestChainIndex;
                        //int currentQuestLineIndex = storylineData.currentQuestLineIndex;
                        //StorylineManager.onWorldEventEndedEvent.Invoke(id, currentQuestChainIndex, currentQuestLineIndex);
                    }
                    if (currentSO_Dialogues.so_Choices.Count > 0)
                    {
   
                        CreateChoiceUIs();
                    }
                    else
                    {
                        if (currentSO_Dialogues.nextChapterDialogue == null)
                        {
                            // Debug.Log("AUTO CLOSE BEING DONE");
                            if (isEndTransitionEnabled)
                            {

                                //Debug.Log("END TRANSIONING");
                                // TransitionUI.onFadeInAndOutTransition.Invoke(1, 0.5f, 1, 0, 0.5f, OnCloseCharacterDialogueUI);
                                TransitionUI.onFadeInAndOutTransition.Invoke(1, 0.25f, 1, 0, 0.25f, OnCloseCharacterDialogueUI);

                            }
                            else
                            {
                                // Debug.Log("END WITHOUT TRANSIONING");
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
    }
    IEnumerator Co_EmotionIn(Image p_avatarImage,Sprite p_avatar,int p_characterDataIndex, CharacterEmotionType p_emotion)
    {
        p_avatarImage.color = new Color(p_avatarImage.color.r, p_avatarImage.color.g, p_avatarImage.color.b, 0);
        p_avatarImage.sprite = p_avatar;// currentDialogue.character.avatars[(int)currentDialogue.emotion];

        var sequence = DOTween.Sequence()
        .Append(p_avatarImage.DOFade(1, avatarFadeTime));
        sequence.Play();
        yield return sequence.WaitForCompletion();
        yield return new WaitForSeconds(avatarDelayTime);
        var sequenceTwo = DOTween.Sequence()
        .Append(emoticonBubbleImage.DOFade(1, emoticonBubbleFadeTime));
        sequenceTwo.Join(emoticonBubbleRectTransform.DOSizeDelta(characterPresetDatas[p_characterDataIndex].emoteBubbleEndRectTransform.sizeDelta, emoticonBubbleSizeTime, false));
        sequenceTwo.Play();
        yield return new WaitForSeconds(emoticonSizeTime / 2);//sequence.WaitForCompletion();
        emoticonObject.SetActive(true);
        var sequenceThree = DOTween.Sequence()
        .Append(emoticonImage.DOFade(1, emoticonFadeTime));
        sequenceThree.Join(emoticonRectTransform.DOSizeDelta(targetEmoticonSize.sizeDelta, emoticonSizeTime, false));
        sequenceThree.Play();
        emoticonAnim.SetInteger("enum", (int)p_emotion);
    

    }


    IEnumerator Co_EmotionOut(int p_characterDataIndex)
    {
        var sequenceThree = DOTween.Sequence()
         .Append(emoticonImage.DOFade(0, emoticonFadeTime));
        sequenceThree.Join(emoticonRectTransform.DOSizeDelta(defaultEmoticonSize.sizeDelta, emoticonSizeTime, false));
        sequenceThree.Play();
        yield return new WaitForSeconds(emoticonSizeTime / 2);//sequence.WaitForCompletion();

        var sequenceTwo = DOTween.Sequence()
         .Append(emoticonBubbleImage.DOFade(0, emoticonBubbleFadeTime));
        sequenceTwo.Join(emoticonBubbleRectTransform.DOSizeDelta(characterPresetDatas[p_characterDataIndex].emoteBubbleStartRectTransform.sizeDelta, emoticonBubbleSizeTime, false));
        sequenceTwo.Play();
        yield return sequenceTwo.WaitForCompletion();
        emoticonObject.SetActive(false);
    }
}
