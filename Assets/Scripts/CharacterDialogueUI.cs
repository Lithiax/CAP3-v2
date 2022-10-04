using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class CharacterSpokenToEvent : UnityEvent<string, SO_Dialogues> { }
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
    [SerializeField] private TMP_Text currentCharacterNameText;
    [SerializeField] private TMP_Text currentDialogueText;

    [SerializeField] private Image backgroundImage;

    [SerializeField] public GameObject nextDialogueButton;
    [SerializeField] private GameObject choiceUIsContainer;
    private Transform choiceUIsContainerTransform;
    private RectTransform choiceUIsContainerRectTransform;

    [SerializeField] private Transform characterUIContainerTransform;
    [SerializeField] private Transform characterObjectContainerTransform;
    [SerializeField] private ChoiceUI choiceUIPrefab;

    [SerializeField]
    private List<Character> savedCharacters = new List<Character>();
    [SerializeField]
    private List<CharacterPresetData> characterPresetDatas = new List<CharacterPresetData>();

    [SerializeField]
    private GameObject cueBankContainer;
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

    private int currentDialogueIndex;

    private int runningCoroutines = 0;
    private bool isSkipping = false;
    private string id;


    private bool isBig = false;


    [SerializeField]
    private GameObject extraButtonsContainer;

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

    bool isStartTransitionEnabled = true;
    bool isEndTransitionEnabled = true;

    bool isAdvancedonWorldEventEndedEvent = false;

    bool isAlreadyEnded = false;

    public static CharacterSpokenToEvent onCharacterSpokenToEvent = new CharacterSpokenToEvent();

    //[HideInInspector]
    public SO_Dialogues currentSO_Dialogues;

    [SerializeField]
    private List<int> dialogueIndexInSheet = new List<int>();
    [SerializeField]
    private List<int> backgroundIndexInSheet = new List<int>();
    [SerializeField]
    private List<int> choiceIndexInSheet = new List<int>();

    [SerializeField]
    private HealthUI healthUI;



    private void Awake()
    {
        onCharacterSpokenToEvent.AddListener(OnCharacterSpokenTo);
        choiceUIsContainerTransform = choiceUIsContainer.transform;
        choiceUIsContainerRectTransform = choiceUIsContainer.GetComponent<RectTransform>();

        DialogueSpreadSheetPatternConstants.dialogueName = DialogueSpreadSheetPatternConstants.dialogueName.ToLower();
        DialogueSpreadSheetPatternConstants.choiceName = DialogueSpreadSheetPatternConstants.choiceName.ToLower();
    }

    public void OnEnable()
    {
        SpreadSheetAPI.OnFinishedLoadingValues += TranslateIntoScriptableObject;

    }
    public void OnCharacterSpokenTo(string p_id, SO_Dialogues p_SO_Dialogue)
    {
        id = p_id;
        TranslateIntoScriptableObject(); // FIX

        if (isStartTransitionEnabled)
        {
            TransitionUI.onFadeInAndOutTransition.Invoke(1, 0.5f, 1, 0, 0.5f, OnOpenCharacterDialogueUI);
        }
        else
        {
            OnOpenCharacterDialogueUI();
        }


    }

    public void TranslateIntoScriptableObject()
    {
        dialogueIndexInSheet.Clear();
        choiceIndexInSheet.Clear();
        backgroundIndexInSheet.Clear();
        for (int i = 0; i < SpreadSheetAPI.instance.testerFormattedSheetRows.Length; i++)
        {
            if (SpreadSheetAPI.instance.testerFormattedSheetRows[i].ToLower().Contains(DialogueSpreadSheetPatternConstants.dialogueName))
            {
                dialogueIndexInSheet.Add(i);
            }
            else if (SpreadSheetAPI.instance.testerFormattedSheetRows[i].ToLower().Contains(DialogueSpreadSheetPatternConstants.choiceName))
            {
                choiceIndexInSheet.Add(i);
            }
            else if (SpreadSheetAPI.instance.testerFormattedSheetRows[i].ToLower().Contains("background sprite"))
            {
                backgroundIndexInSheet.Add(i);
            }
        }

        //for (int i = 0; i < dialogueIndexInSheet.Count; i++)
        //{
        //    Debug.Log("dialogue indexes: " + dialogueIndexInSheet[i]);
        //}
        //for (int i = 0; i < choiceIndexInSheet.Count; i++)
        //{
        //    Debug.Log("choice indexes: " + choiceIndexInSheet[i]);
        //}
        //Reset
        currentSO_Dialogues.dialogues.Clear();
        currentSO_Dialogues.choiceDatas.Clear();


        for (int i = 0; i < dialogueIndexInSheet.Count; i++)
        {
            int currentGeneratedDialogueIndex = dialogueIndexInSheet[i];
  
            Dialogue newDialogue = new Dialogue();
            currentSO_Dialogues.dialogues.Add(newDialogue);
            //Debug.Log(dialogueIndexInSheet[i]);
            //Debug.Log(backgroundIndexInSheet[i]);
            //Setting Character Data
            string characterOne = SpreadSheetAPI.GetCellString(currentGeneratedDialogueIndex + DialogueSpreadSheetPatternConstants.characterOneRowPattern, DialogueSpreadSheetPatternConstants.characterCollumnPattern).ToLower();
            string characterTwo = SpreadSheetAPI.GetCellString(currentGeneratedDialogueIndex + DialogueSpreadSheetPatternConstants.characterTwoRowPattern, DialogueSpreadSheetPatternConstants.characterCollumnPattern).ToLower();
            string characterThree = SpreadSheetAPI.GetCellString(currentGeneratedDialogueIndex + DialogueSpreadSheetPatternConstants.characterThreeRowPattern, DialogueSpreadSheetPatternConstants.characterCollumnPattern).ToLower();

            if (characterOne != "none")
            {   
                CharacterData newCharacterDataOne = new CharacterData();
                TranslateToCharacterData(newCharacterDataOne, currentGeneratedDialogueIndex,DialogueSpreadSheetPatternConstants.characterOneRowPattern);
                newDialogue.characterDatas.Add(newCharacterDataOne);
            }
            if (characterTwo != "none")
            {
                CharacterData newCharacterDataTwo = new CharacterData();
                TranslateToCharacterData(newCharacterDataTwo, currentGeneratedDialogueIndex, DialogueSpreadSheetPatternConstants.characterTwoRowPattern);
                newDialogue.characterDatas.Add(newCharacterDataTwo);
            }
            if (characterThree != "none")
            {
                CharacterData newCharacterDataThree = new CharacterData();
                TranslateToCharacterData(newCharacterDataThree, currentGeneratedDialogueIndex, DialogueSpreadSheetPatternConstants.characterThreeRowPattern);
                newDialogue.characterDatas.Add(newCharacterDataThree);
            }

            //Setting Que Bank
            currentSO_Dialogues.isEnabled = VisualNovelDatas.TranslateIsSpeaking(SpreadSheetAPI.GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.isEnabledCollumnPattern));
            currentSO_Dialogues.hapticType = VisualNovelDatas.FindHapticType(SpreadSheetAPI.GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.hapticTypeCollumnPattern));
            currentSO_Dialogues.vocalicType = VisualNovelDatas.FindVocalicType(SpreadSheetAPI.GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.vocalicTypeCollumnPattern));
            currentSO_Dialogues.kinesicType = VisualNovelDatas.FindKinesicType(SpreadSheetAPI.GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.kinesicCollumnPattern));
            currentSO_Dialogues.oculesicType = VisualNovelDatas.FindOculesicType(SpreadSheetAPI.GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.oculesicCollumnPattern));
            currentSO_Dialogues.physicalApperanceType = VisualNovelDatas.FindPhysicalAppearanceType(SpreadSheetAPI.GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.physicalAppearanceCollumnPattern));

            //Setting Words
 
            string[] retrievedWords = SpreadSheetAPI.GetCurrentSheetRow(currentGeneratedDialogueIndex + DialogueSpreadSheetPatternConstants.wordsRowPattern);
            string finalWords = "";
            for (int x = 0; x < retrievedWords.Length; x++)
            {
                finalWords += retrievedWords[x];
            }
            newDialogue.words = finalWords;

            //Setting Backgounrd
            //Debug.LogError(SpreadSheetAPI.GetCellString(backgroundIndexInSheet[i]+1, DialogueSpreadSheetPatternConstants.backgroundCollumnPattern));
            newDialogue.backgroundSprite = VisualNovelDatas.FindBackgroundSprite(SpreadSheetAPI.GetCellString(backgroundIndexInSheet[i]+1, DialogueSpreadSheetPatternConstants.backgroundCollumnPattern));

         
        }

        //Setting Choice
        for (int i = 0; i < choiceIndexInSheet.Count; i++)
        {
            int currentGeneratedChoiceIndex = 0;
            currentGeneratedChoiceIndex = choiceIndexInSheet[i];

            ChoiceData newChoiceData = null;
            newChoiceData = new ChoiceData();
            currentSO_Dialogues.choiceDatas.Add(newChoiceData);

            //Setting Choice
            newChoiceData.words = SpreadSheetAPI.GetCellString(currentGeneratedChoiceIndex + DialogueSpreadSheetPatternConstants.choiceRowPattern, DialogueSpreadSheetPatternConstants.choiceNameCollumnPattern);
            newChoiceData.branchDialogueName = SpreadSheetAPI.GetCellString(currentGeneratedChoiceIndex + DialogueSpreadSheetPatternConstants.choiceRowPattern, DialogueSpreadSheetPatternConstants.nextDialogueSheetNameCollumnPattern);
            int test = 0;
            string testnum = SpreadSheetAPI.GetCellString(currentGeneratedChoiceIndex + DialogueSpreadSheetPatternConstants.choiceRowPattern, DialogueSpreadSheetPatternConstants.choiceDamagePattern);
            if (testnum != "error")
            {
                int.TryParse(testnum, out test);
            }
            newChoiceData.damage = test;
            newChoiceData.eventID = SpreadSheetAPI.GetCellString(currentGeneratedChoiceIndex + DialogueSpreadSheetPatternConstants.choiceRowPattern, DialogueSpreadSheetPatternConstants.eventID);
        }

    }
    public void TranslateToCharacterData(CharacterData p_characterData, int p_currentGeneratedDialogueIndex, int p_characterRowPattern)
    {
        p_characterData.character = VisualNovelDatas.FindCharacter(SpreadSheetAPI.GetCellString(p_currentGeneratedDialogueIndex + p_characterRowPattern, DialogueSpreadSheetPatternConstants.characterCollumnPattern));
        p_characterData.faceEmotion = VisualNovelDatas.FindFaceEmotion(SpreadSheetAPI.GetCellString(p_currentGeneratedDialogueIndex + p_characterRowPattern, DialogueSpreadSheetPatternConstants.faceEmotionCollumnPattern));
        p_characterData.bodyEmotion = VisualNovelDatas.FindFaceEmotion(SpreadSheetAPI.GetCellString(p_currentGeneratedDialogueIndex + p_characterRowPattern, DialogueSpreadSheetPatternConstants.bodyEmotionCollumnPattern));
        p_characterData.characterPosition = VisualNovelDatas.FindBodyPosition(SpreadSheetAPI.GetCellString(p_currentGeneratedDialogueIndex + p_characterRowPattern, DialogueSpreadSheetPatternConstants.characterPositionCollumnPattern));
        p_characterData.isFlipped = VisualNovelDatas.TranslateIsFlipped(SpreadSheetAPI.GetCellString(p_currentGeneratedDialogueIndex + p_characterRowPattern, DialogueSpreadSheetPatternConstants.isFlippedCollumnPattern));
        p_characterData.isSpeaking = VisualNovelDatas.TranslateIsSpeaking(SpreadSheetAPI.GetCellString(p_currentGeneratedDialogueIndex + p_characterRowPattern, DialogueSpreadSheetPatternConstants.isSpeakingCollumnPattern));
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

        while (currentDialogueIndex < currentSO_Dialogues.dialogues.Count-2)
        {
           // yield return new WaitForSeconds(0.5f);
            OnNextButtonUIPressed();

            //Debug.Log(currentDialogueIndex + "/" + currentSO_Dialogues.dialogues.Count);
            //currentDialogueIndex++;
        }
        Debug.Log("Auto skipped " + currentDialogueIndex);
        //Debug.Log("Ended " + currentDialogueIndex);

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
        cueBankContainer.gameObject.SetActive(false);

        OnNextButtonUIPressed();
    }

    void NextDialogue()
    {
        Debug.Log(currentDialogueIndex + " NEXT DIALOGUE " + dialogueIndexInSheet[currentDialogueIndex]);
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
    public void ToggleCueBankUI()
    {
        if (currentSO_Dialogues.isEnabled)
        {
            cueBankContainer.SetActive(!cueBankContainer.activeSelf);
        }
        
    }
    public void ToggleExtras()
    {
        if (!smallDialogueBox.activeSelf)
        {
            currentDialogueText = smallDialogueText;
            smallSpeakerText.text = currentCharacterNameText.text;
            currentCharacterNameText = smallSpeakerText;
        }
        else if (smallDialogueBox.activeSelf)
        {
            currentDialogueText = bigDialogueText;
            bigSpeakerText.text = currentCharacterNameText.text;
            currentCharacterNameText = bigSpeakerText;

        }
        Dialogue currentDialogue = currentSO_Dialogues.dialogues[currentDialogueIndex];
        smallSpeakerBox.SetActive(!smallSpeakerBox.activeSelf);
        smallDialogueBox.SetActive(!smallDialogueBox.activeSelf);
        bigSpeakerBox.SetActive(!bigSpeakerBox.activeSelf);
        bigDialogueBox.SetActive(!bigDialogueBox.activeSelf);
        extraButtonsContainer.SetActive(!extraButtonsContainer.activeSelf);
        if (runningCoroutines > 0 && !isSkipping)
        {
            isSkipping = true;
            StopAllCoroutines();
            runningCoroutines = 0;
            // Debug.Log("READYING");

        }


     
        SetSpeech(currentDialogue.words);
 
        isBig = !isBig;
    }

    public void SetChoiceDamage(int p_modifier)
    {
        healthUI.ModifyHealthEvent.Invoke(p_modifier);
    }
    public void ChooseChoiceUI(int index)
    {
        nextDialogueButton.SetActive(true);
        if (choiceUIsContainer.activeSelf)
        {
            for (int i = 0; i < choiceUIsContainerTransform.childCount; i++)
            {
                Destroy(choiceUIsContainerTransform.GetChild(i).gameObject);

            }
        }
        choiceUIsContainer.SetActive(false);
        SpreadSheetAPI.SetCurrentIndexToSheet(currentSO_Dialogues.choiceDatas[index].branchDialogueName);
        SetChoiceDamage(currentSO_Dialogues.choiceDatas[index].damage);
        DialogueSpreadSheetPatternConstants.effects.Add(currentSO_Dialogues.choiceDatas[index].eventID);
        //TEMPORARY JUST TO SEE
        //for (int i=0; i < DialogueSpreadSheetPatternConstants.effects.Count; i++)
        //{
        //    Debug.Log("ALL THE LISTED EFFECTS: "+  DialogueSpreadSheetPatternConstants.effects[i]);
        //}
      
        ResetCharacterDialogueUI();
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
                            Debug.Log(currentDialogueIndex + " Set Body Emotion is Not available");                        
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

    void SetSpeakerName(List<CharacterData> p_characterDatas) // work on this
    {
        if (p_characterDatas.Count > 0)
        {
            for (int i = 0; i < p_characterDatas.Count; i++)
            {

                if (p_characterDatas[i].isSpeaking)
                {
                    currentCharacterNameText.text = p_characterDatas[i].character.name;

                }

            }
        }
        else
        {
            currentCharacterNameText.text = "NO CHARACTER ASSIGNED";
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
    void SetSpeech(string p_words)
    {
        if (isSkipping)
        {
            SetWords(p_words);
        }
        else
        {
            StartCoroutine(Co_TypeWriterEffect(currentDialogueText, p_words));
        }
   
    }
    void SetWords(string p_words)
    {
        currentDialogueText.text = p_words;
    }

    void SetCueBank(SO_Dialogues p_characterDatas)
    {
        if (p_characterDatas.isEnabled)
        {
            cueBankContainer.gameObject.SetActive(false);
        }
        else
        {
            cueBankContainer.gameObject.SetActive(true);
            hapticText.text = p_characterDatas.hapticType.ToString();
            vocalicText.text = p_characterDatas.vocalicType.ToString();
            kinesicText.text = p_characterDatas.kinesicType.ToString();
            oculesicText.text = p_characterDatas.oculesicType.ToString();
            physicalAppearanceText.text = p_characterDatas.physicalApperanceType.ToString();
           
        }
    }

    public void CheckIfReady()
    {
        if (runningCoroutines == 0)
        {
            //Debug.Log("READYING");
            isSkipping = true;
        }
    }


    public void OnNextButtonUIPressed()
    {
        
        if (currentDialogueIndex < currentSO_Dialogues.dialogues.Count)
        {
            //Debug.Log("BUTTON PRESSED " + currentDialogueIndex + " RC: "+ runningCoroutines 
           //     + " iS: " + isSkipping
            //    + " iR: ");
            Dialogue currentDialogue = currentSO_Dialogues.dialogues[currentDialogueIndex];
            
            if (runningCoroutines > 0 && !isSkipping)
            {
                isSkipping = true;
                StopAllCoroutines();
                runningCoroutines=0;
               // Debug.Log("READYING");
            
            }
          
            else if (isSkipping)// && !isReady)
            {
              //  Debug.Log("READIED");
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
                SetFacialEmotion(currentDialogue.characterDatas[i]);
                SetBodyEmotion(currentDialogue.characterDatas[i]);
                SetSpeakerTint(currentDialogue.characterDatas[i].isSpeaking, currentDialogue.characterDatas[i].character);
            }

            SetSpeakerName(currentDialogue.characterDatas);
            SetBackground(currentDialogue.backgroundSprite);
            SetSpeech(currentDialogue.words);
          
        }
        else if (currentDialogueIndex >= currentSO_Dialogues.dialogues.Count)
        {
            if (!isAlreadyEnded)
            {
                if (currentSO_Dialogues.choiceDatas.Count > 1)
                {
                    CreateChoiceUIs();
                    SetCueBank(currentSO_Dialogues);
                }
                else if (currentSO_Dialogues.choiceDatas.Count == 1)
                {
                    Debug.Log(currentSO_Dialogues.choiceDatas[0].branchDialogueName);
                    SpreadSheetAPI.SetCurrentIndexToSheet(currentSO_Dialogues.choiceDatas[0].branchDialogueName);
                    SetChoiceDamage(currentSO_Dialogues.choiceDatas[0].damage);
                    ResetCharacterDialogueUI();

                }
                else if (currentSO_Dialogues.choiceDatas.Count == 0)
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
                
            }
        }
    }

}
