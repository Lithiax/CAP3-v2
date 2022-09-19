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
    public SO_Character so_Character;
    public RectTransform avatarRectTransform;
    [HideInInspector] public Image avatarImage;


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

    [SerializeField] public GameObject nextDialogueButton;
    [SerializeField] private GameObject choiceUIsContainer;
    private Transform choiceUIsContainerTransform;
    private RectTransform choiceUIsContainerRectTransform;
    [SerializeField] private ChoiceUI choiceUIPrefab;

    [SerializeField]
    private List<CharacterPresetData> characterPresetDatas = new List<CharacterPresetData>();
    [SerializeField] private List<SO_Character> so_Characters = new List<SO_Character>();
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


    public IEnumerator runningCoroutine;
    public IEnumerator runningEmotionCoroutine;
    public IEnumerator runningAvatarCoroutine;
    public static CharacterSpokenToEvent onCharacterSpokenToEvent = new CharacterSpokenToEvent();
    public static FirstTimeFoodOnEndEvent onFirstTimeFoodOnEndEvent = new FirstTimeFoodOnEndEvent();
    bool canDo = false;

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

       //ResetAllCharacterAvatarImage();
        OnNextButtonUIPressed();
    }

    //void ResetAllCharacterAvatarImage()
    //{
    //    for (int i = 0; i < characterPresetDatas.Count; i++)
    //    {
    //        characterPresetDatas[i].avatarImage.color = new Color(characterPresetDatas[i].avatarImage.color.r, characterPresetDatas[i].avatarImage.color.g, characterPresetDatas[i].avatarImage.color.b, 0);

    //    }
    //}
    void NextDialogue()
    {
        Debug.Log("NEXT DIALOGUE");
        currentDialogueIndex++;

        if (currentDialogueIndex == currentSO_Dialogues.dialogues.Count)
        {
            if (isAdvancedonWorldEventEndedEvent)
            {
                isAlreadyEnded = true;
            }
        }
    }
    //public IEnumerator SetAvatar(Image p_avatarImage, Sprite p_sprite)
    //{
    //    var fadeInSequence = DOTween.Sequence()
    //   .Append(p_avatarImage.DOFade(0, avatarFadeTime));
    //    fadeInSequence.Play();

    //    yield return fadeInSequence.WaitForCompletion();
    //    p_avatarImage.sprite = p_sprite;
    //    var fadeOutSequence = DOTween.Sequence()
    //    .Append(p_avatarImage.DOFade(1, avatarFadeTime));
    //    fadeOutSequence.Play();

    //    yield return fadeOutSequence.WaitForCompletion();
    //}
    //public void SetAvatar(Image p_avatarImage, Sprite p_sprite)
    //{
    //    var fadeInSequence = DOTween.Sequence()
    //   .Append(p_avatarImage.DOFade(0, avatarFadeTime));
    //    fadeInSequence.Play();

    //    yield return fadeInSequence.WaitForCompletion();
    //    p_avatarImage.sprite = p_sprite;
    //    var fadeOutSequence = DOTween.Sequence()
    //    .Append(p_avatarImage.DOFade(1, avatarFadeTime));
    //    fadeOutSequence.Play();

    //    yield return fadeOutSequence.WaitForCompletion();
    //}
    public IEnumerator AvatarFadeIn(Image p_avatarImage, Sprite p_sprite)
    {
        p_avatarImage.sprite = p_sprite;
        var fadeInSequence = DOTween.Sequence()
        .Append(p_avatarImage.DOFade(1, avatarFadeTime));
        fadeInSequence.Play();
        yield return fadeInSequence.WaitForCompletion();
    }

    public IEnumerator AvatarFadeOut(Image p_avatarImage)
    {
        var fadeOutSequence = DOTween.Sequence()
       .Append(p_avatarImage.DOFade(0, avatarFadeTime));
        fadeOutSequence.Play();
        yield return fadeOutSequence.WaitForCompletion();
    }

    public IEnumerator SpeakerTintIn(Image p_avatarImage)
    {
        var fadeOutSequence = DOTween.Sequence()
        .Append(p_avatarImage.DOColor(Color.white, avatarFadeTime));
        fadeOutSequence.Play();
        yield return fadeOutSequence.WaitForCompletion();
    }

    public IEnumerator SpeakerTintOut(Image p_avatarImage)
    {
        var fadeInSequence = DOTween.Sequence()
        .Append(p_avatarImage.DOColor(nonSpeakerTintColor, avatarFadeTime));
        fadeInSequence.Play();
        yield return fadeInSequence.WaitForCompletion();
    }
    public void CreateChoiceUIs()
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
    void SetRectTransformToPreset(CharacterPositionType p_characterPositionType, int p_index)
    {
        if (p_characterPositionType == CharacterPositionType.left)
        {
            characterPresetDatas[p_index].avatarRectTransform.anchoredPosition = leftCharacterPresetRectTransform.anchoredPosition;
        }
        else if (p_characterPositionType == CharacterPositionType.center)
        {
            characterPresetDatas[p_index].avatarRectTransform.anchoredPosition = centerCharacterPresetRectTransform.anchoredPosition;
        }
        else if (p_characterPositionType == CharacterPositionType.right)
        {
            characterPresetDatas[p_index].avatarRectTransform.anchoredPosition = rightCharacterPresetRectTransform.anchoredPosition;
        }
    }

    void SetSpeakerTint(bool p_isSpeaking, int p_index)
    {
        //Tint
        if (p_isSpeaking)
        {
            //Add reference coroutine so when player skips it can be referenced and stopped
            StartCoroutine(SpeakerTintIn(characterPresetDatas[p_index].avatarImage));

        }
        else
        {
            StartCoroutine(SpeakerTintOut(characterPresetDatas[p_index].avatarImage));
   
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
        //Do functions to characters to be Removed
        for (int i = 0; i < p_charactersToBeRemoved.Count; i++)
        {
            CharacterPresetData foundPreset = FindPreset(p_charactersToBeRemoved[i]);

            if (foundPreset.so_Character == p_charactersToBeRemoved[i])
            {
                //make this fade out
                Debug.Log("FADING OUT: " + characterPresetDatas[i].avatarImage.gameObject.name);
                StartCoroutine(AvatarFadeOut(foundPreset.avatarImage));

                //foundPreset.avatarImage.color = new Color(foundPreset.avatarImage.color.r, foundPreset.avatarImage.color.g, foundPreset.avatarImage.color.b, 0);
                foundPreset.so_Character = null;
                break;
            }

        }


    }

    void AddAvatar(List<SO_Character> p_charactersToBeAdded)
    {
        //Do functions to characters to be Added
        for (int i = 0; i < p_charactersToBeAdded.Count; i++)
        {
            for (int x = 0; x < characterPresetDatas.Count; x++)
            {
                if (characterPresetDatas[x].so_Character == null)
                {
                    Debug.Log("FADING IN: " + characterPresetDatas[i].avatarImage.gameObject.name);
                    characterPresetDatas[x].so_Character = p_charactersToBeAdded[i];
                    //runningAvatarCoroutine = AvatarFade(characterPresetDatas[x].avatarImage, charactersToBeAdded[i].avatar);

                    //StartCoroutine(runningAvatarCoroutine); //make recursive
                    StartCoroutine(AvatarFadeIn(characterPresetDatas[x].avatarImage, p_charactersToBeAdded[i].avatar));
                    break;
                }
            }
            //    CharacterPresetData foundPreset = FindPreset(charactersToBeAdded[i]);
            //if (foundPreset != null)
            //{
            //    runningAvatarCoroutine = AvatarFade(foundPreset.avatarImage, charactersToBeAdded[i].avatar);

            //    StartCoroutine(runningAvatarCoroutine); //make recursive

            //}


        }
    }
    void IdentifyCharactersToAdd(List<SO_Character> one, List<SO_Character> two, List<SO_Character> tempCharacters, List<SO_Character> charactersToBeAdded)
    {
        //Mark the Characters to Add and Characters that Exists
        for (int i = 0; i < one.Count; i++)
        {
            if (two.Count > 0)
            {
                for (int x = 0; x < two.Count;)
                {
                    if (one[i] == so_Characters[x])
                    {
                        //Not new
                        tempCharacters.Add(two[x]);
                    }

                    x++;
                    if (x >= so_Characters.Count)
                    {
                        charactersToBeAdded.Add(two[x]);
                        //new

                    }
                }
            }
            else
            {
                charactersToBeAdded.Add(one[i]);
            }

        }
        
    }

    void IdentifyCharactersToRemove(List<SO_Character> one, List<SO_Character> two, List<SO_Character> charactersToBeRemoved)
    {
        //Mark the Characters to Remove
        for (int i = 0; i < one.Count; i++)
        {
            if (two.Count > 0)
            {
                for (int x = 0; x < two.Count;)
                {
                    if (so_Characters[i] == two[x])
                    {
                        break;
                    }

                    x++;
                    if (x >= two.Count)
                    {
                        if (charactersToBeRemoved.Count > 0)
                        {
                            for (int y = 0; y < charactersToBeRemoved.Count;)
                            {
                                if (charactersToBeRemoved[y] == one[x])
                                {
                                    //It already is marked to be removed
                                    break;
                                }
                                y++;
                                if (x >= charactersToBeRemoved.Count)
                                {
                                    //It has not been already marked to be removed
                                    charactersToBeRemoved.Add(one[x]);
                                }


                            }
                        }
                        else
                        {
                            charactersToBeRemoved.Add(one[x]);
                        }



                    }
                }
            }
            else
            {
                charactersToBeRemoved.Add(one[i]);
            }

        }
       
    }
    void CacheCharacter()

    {

    }

    void CheckCachedCharacters(List<CharacterData> p_characterDatas) //Rename and chop things into functions
    {
        List<SO_Character> currentDialogueCharacters = new List<SO_Character>();
        List<SO_Character> tempCharacters = new List<SO_Character>();
        List<SO_Character> charactersToBeRemoved = new List<SO_Character>();
        List<SO_Character> charactersToBeAdded = new List<SO_Character>();

        for (int i = 0; i < p_characterDatas.Count; i++)
        {
            currentDialogueCharacters.Add(p_characterDatas[i].character);
           
        }

        //temporary make this dynamic
        if (currentDialogueCharacters.Count > so_Characters.Count)
        {
            //Mark the Characters to Add and Characters that Exists
            IdentifyCharactersToAdd(currentDialogueCharacters, so_Characters, tempCharacters, charactersToBeAdded);
        }
        else
        {
            //Mark the Characters to Remove
            IdentifyCharactersToRemove(so_Characters, currentDialogueCharacters, charactersToBeRemoved);
         
        }

        RemoveAvatar(charactersToBeRemoved);
        AddAvatar(charactersToBeAdded);

        so_Characters.Clear();
        for (int i = 0; i < characterPresetDatas.Count; i++)
        {
            so_Characters.Add(characterPresetDatas[i].so_Character);
            if (characterPresetDatas[i].so_Character != null)
            {
                SetSpeakerTint(currentSO_Dialogues.dialogues[currentDialogueIndex].characterDatas[i].isSpeaking, i);
            }
        }
    

    }

    CharacterPresetData FindPreset(SO_Character p_so_Character)
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
    public void OnNextButtonUIPressed()
    {
        Debug.Log("Next Button UI Clicked");
        //if (runningEmotionCoroutine != null)
        //{
        //    StopCoroutine(runningEmotionCoroutine);
        //    runningEmotionCoroutine = null;

        //    runningEmotionCoroutine = Co_EmotionOut(lastCharacterData);
        //    StartCoroutine(runningEmotionCoroutine);
        //}

        if (currentDialogueIndex < currentSO_Dialogues.dialogues.Count)
        {

            Dialogue currentDialogue = currentSO_Dialogues.dialogues[currentDialogueIndex];

            int currentSpeakingCharacterIndex = -1;
            //if (currentDialogue.characterDatas.Count < charactersAvatarImage.Count)
            //{
            //    ResetAllCharacterAvatarImage();
                
            //}
            for (int i = 0; i < currentDialogue.characterDatas.Count;i++)
            {

                //Set Character UI Rect Transform
                //Position
                SetRectTransformToPreset(currentDialogue.characterDatas[i].characterPosition,i);

                //Scale
              


                characterNameText.text = currentDialogue.characterDatas[i].character.name;
                SetBackground(currentDialogue.backgroundSprite);
                //if (cachedSprite != currentDialogue.characterDatas[i].character.avatar)
                //{


                //if (runningAvatarCoroutine != null)
                //{
                //    StopCoroutine(runningAvatarCoroutine);
                //    runningAvatarCoroutine = null;
                //}


                // }
              
                CharacterData currentSpeakingCharacter = null;
                if (currentSpeakingCharacterIndex != -1)
                {
                    currentSpeakingCharacter = currentSpeakingCharacter = currentDialogue.characterDatas[currentSpeakingCharacterIndex];
                }

           
            }
            CheckCachedCharacters(currentDialogue.characterDatas); //Rename and chop things into functions
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

                 
                    if (currentSO_Dialogues.choiceDatas.Count > 1)
                    {
   
                        CreateChoiceUIs();
                    }
                    else if (currentSO_Dialogues.choiceDatas.Count == 1)
                    {
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
    #region Old Code Storage
    //IEnumerator Co_EmotionIn(Image p_avatarImage,Sprite p_avatar,int p_characterDataIndex, CharacterEmotionType p_emotion)
    //{
    //    p_avatarImage.color = new Color(p_avatarImage.color.r, p_avatarImage.color.g, p_avatarImage.color.b, 0);
    //    p_avatarImage.sprite = p_avatar;// currentDialogue.character.avatars[(int)currentDialogue.emotion];

    //    var sequence = DOTween.Sequence()
    //    .Append(p_avatarImage.DOFade(1, avatarFadeTime));
    //    sequence.Play();
    //    yield return sequence.WaitForCompletion();
    //    yield return new WaitForSeconds(avatarDelayTime);
    //    var sequenceTwo = DOTween.Sequence()
    //    .Append(emoticonBubbleImage.DOFade(1, emoticonBubbleFadeTime));
    //    sequenceTwo.Join(emoticonBubbleRectTransform.DOSizeDelta(characterPresetDatas[p_characterDataIndex].emoteBubbleEndRectTransform.sizeDelta, emoticonBubbleSizeTime, false));
    //    sequenceTwo.Play();
    //    yield return new WaitForSeconds(emoticonSizeTime / 2);//sequence.WaitForCompletion();
    //    emoticonObject.SetActive(true);
    //    var sequenceThree = DOTween.Sequence()
    //    .Append(emoticonImage.DOFade(1, emoticonFadeTime));
    //    sequenceThree.Join(emoticonRectTransform.DOSizeDelta(targetEmoticonSize.sizeDelta, emoticonSizeTime, false));
    //    sequenceThree.Play();
    //    emoticonAnim.SetInteger("enum", (int)p_emotion);


    //}


    //IEnumerator Co_EmotionOut(int p_characterDataIndex)
    //{
    //    var sequenceThree = DOTween.Sequence()
    //     .Append(emoticonImage.DOFade(0, emoticonFadeTime));
    //    sequenceThree.Join(emoticonRectTransform.DOSizeDelta(defaultEmoticonSize.sizeDelta, emoticonSizeTime, false));
    //    sequenceThree.Play();
    //    yield return new WaitForSeconds(emoticonSizeTime / 2);//sequence.WaitForCompletion();

    //    var sequenceTwo = DOTween.Sequence()
    //     .Append(emoticonBubbleImage.DOFade(0, emoticonBubbleFadeTime));
    //    sequenceTwo.Join(emoticonBubbleRectTransform.DOSizeDelta(characterPresetDatas[p_characterDataIndex].emoteBubbleStartRectTransform.sizeDelta, emoticonBubbleSizeTime, false));
    //    sequenceTwo.Play();
    //    yield return sequenceTwo.WaitForCompletion();
    //    emoticonObject.SetActive(false);
    //}
    #endregion

}
