using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class SpreadSheetReader : MonoBehaviour
{
    public static SpreadSheetReader instance;
    public void Awake()
    {
        instance = this;

        
    }

    private void Start()
    {
        LoadLocalFile(temporarySheetNameToLoad, StorylineManager.instance.temp.currentSO_Dialogues);
    }

    [SerializeField] public static string[] formattedSheetRows;
    [SerializeField] public string[] testerFormattedSheetRows;

    [SerializeField]
    public List<int> dialogueIndexInSheet = new List<int>();
    [SerializeField]
    public List<int> backgroundIndexInSheet = new List<int>();
    [SerializeField]
    public List<int> choiceIndexInSheet = new List<int>();

    public static Action OnFinishedLoadingValues;

    public string temporarySheetNameToLoad;

    public static void LoadLocalFile(string p_fileName, SO_Dialogues p_sceneToLoad = null)
    {
        Debug.Log("Loading " + p_fileName);
        string newString = p_fileName.Replace(" ", string.Empty);
        string rawJson = JSONFileHandler.ReadFromJSON(newString + ".json");
        formattedSheetRows = rawJson.Split(new char[] { '\n' });
        if (p_sceneToLoad != null)
        {
            SpreadSheetReader.instance.ReadLocalFile(newString + ".json", p_sceneToLoad);
        }


    }
    public void ReadLocalFile(string p_fileName, SO_Dialogues p_sceneToLoad = null)
    {

        testerFormattedSheetRows = formattedSheetRows;//tester;
        TranslateIntoScriptableObject(p_sceneToLoad);

    }

    public void TranslateIntoScriptableObject(SO_Dialogues p_soDialogue)
    {
        //TransitionUI.instance.tester.text = "traa: ";
        if (p_soDialogue != null)
        {
            dialogueIndexInSheet.Clear();
            choiceIndexInSheet.Clear();
            backgroundIndexInSheet.Clear();
            for (int i = 0; i < testerFormattedSheetRows.Length; i++)
            {
                if (testerFormattedSheetRows[i].ToLower().Contains(DialogueSpreadSheetPatternConstants.dialogueName))
                {
                    dialogueIndexInSheet.Add(i);
                }
                else if (testerFormattedSheetRows[i].ToLower().Contains(DialogueSpreadSheetPatternConstants.choiceName))
                {
                    choiceIndexInSheet.Add(i);
                }
                else if (testerFormattedSheetRows[i].ToLower().Contains("background sprite"))
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
            if (p_soDialogue.dialogues.Count > 0)
            {
                p_soDialogue.dialogues.Clear();
            }
            if (p_soDialogue.choiceDatas.Count > 0)
            {
                p_soDialogue.choiceDatas.Clear();
            }




            for (int i = 0; i < dialogueIndexInSheet.Count; i++)
            {
                int currentGeneratedDialogueIndex = dialogueIndexInSheet[i];

                Dialogue newDialogue = new Dialogue();
                p_soDialogue.dialogues.Add(newDialogue);
                //Debug.Log(dialogueIndexInSheet[i]);
                //Debug.Log(backgroundIndexInSheet[i]);
                //Setting Character Data
                string characterOne = GetCellString(currentGeneratedDialogueIndex + DialogueSpreadSheetPatternConstants.characterOneRowPattern, DialogueSpreadSheetPatternConstants.characterColumnPattern).ToLower();
                string characterTwo = GetCellString(currentGeneratedDialogueIndex + DialogueSpreadSheetPatternConstants.characterTwoRowPattern, DialogueSpreadSheetPatternConstants.characterColumnPattern).ToLower();
                string characterThree = GetCellString(currentGeneratedDialogueIndex + DialogueSpreadSheetPatternConstants.characterThreeRowPattern, DialogueSpreadSheetPatternConstants.characterColumnPattern).ToLower();

                if (characterOne != "none")
                {
                    CharacterData newCharacterDataOne = new CharacterData();
                    TranslateToCharacterData(newCharacterDataOne, currentGeneratedDialogueIndex, DialogueSpreadSheetPatternConstants.characterOneRowPattern);
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
                p_soDialogue.isEnabled = VisualNovelDatas.TranslateIsSpeaking(GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.isEnabledColumnPattern));
                p_soDialogue.hapticType = VisualNovelDatas.FindHapticType(GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.hapticTypeColumnPattern));
                p_soDialogue.vocalicType = VisualNovelDatas.FindVocalicType(GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.vocalicTypeColumnPattern));
                p_soDialogue.kinesicType = VisualNovelDatas.FindKinesicType(GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.kinesicColumnPattern));
                p_soDialogue.oculesicType = VisualNovelDatas.FindOculesicType(GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.oculesicColumnPattern));
                p_soDialogue.physicalApperanceType = VisualNovelDatas.FindPhysicalAppearanceType(GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.physicalAppearanceColumnPattern));

                //Setting Words

                string[] retrievedWords = GetCurrentSheetRow(currentGeneratedDialogueIndex + DialogueSpreadSheetPatternConstants.wordsRowPattern);
                string finalWords = "";
                for (int x = 0; x < retrievedWords.Length; x++)
                {
                    finalWords += retrievedWords[x];
                }
                newDialogue.words = finalWords;

                //Setting Backgounrd
                //Debug.LogError(SpreadSheetAPI.GetCellString(backgroundIndexInSheet[i]+1, DialogueSpreadSheetPatternConstants.backgroundCollumnPattern));
                newDialogue.backgroundSprite = VisualNovelDatas.FindBackgroundSprite(GetCellString(backgroundIndexInSheet[i] + 1, DialogueSpreadSheetPatternConstants.backgroundColumnPattern));
                newDialogue.specificEventType = VisualNovelDatas.FindEventType(GetCellString(backgroundIndexInSheet[i] + 1, DialogueSpreadSheetPatternConstants.eventTypeColumnPattern));
                newDialogue.specificEventParameter = GetCellString(backgroundIndexInSheet[i] + 1, DialogueSpreadSheetPatternConstants.eventParameterColumnPattern);

            }

            //Setting Choice
            for (int i = 0; i < choiceIndexInSheet.Count; i++)
            {
                int currentGeneratedChoiceIndex = 0;
                currentGeneratedChoiceIndex = choiceIndexInSheet[i];

                ChoiceData newChoiceData = null;
                newChoiceData = new ChoiceData();
                p_soDialogue.choiceDatas.Add(newChoiceData);

                //Setting Choice
                newChoiceData.words = GetCellString(currentGeneratedChoiceIndex + DialogueSpreadSheetPatternConstants.choiceRowPattern, DialogueSpreadSheetPatternConstants.choiceNameColumnPattern);
               
                string branchingDialogueName = GetCellString(currentGeneratedChoiceIndex + DialogueSpreadSheetPatternConstants.choiceRowPattern, DialogueSpreadSheetPatternConstants.nextDialogueSheetNameColumnPattern);
                newChoiceData.branchDialogueName = branchingDialogueName;


                int healthModifier = 0;
                string healthModifierString = GetCellString(currentGeneratedChoiceIndex + DialogueSpreadSheetPatternConstants.choiceRowPattern, DialogueSpreadSheetPatternConstants.healthModifierColumnPattern);
                if (healthModifierString != "error")
                {
                    int.TryParse(healthModifierString, out healthModifier);
                }
                newChoiceData.healthModifier = healthModifier;
               
                int healthCeilingCondition = 0;
                string healthCeilingConditionString = GetCellString(currentGeneratedChoiceIndex + DialogueSpreadSheetPatternConstants.choiceRowPattern, DialogueSpreadSheetPatternConstants.healthCeilingConditionColumnPattern);
                if (healthCeilingConditionString != "error")
                {
                    int.TryParse(healthCeilingConditionString, out healthCeilingCondition);
                }
                newChoiceData.healthCeilingCondition = healthCeilingCondition;

                int healthFloorCondition = 0;
                string healthFloorConditionString = GetCellString(currentGeneratedChoiceIndex + DialogueSpreadSheetPatternConstants.choiceRowPattern, DialogueSpreadSheetPatternConstants.healthFloorConditionColumnPattern);
                if (healthFloorConditionString != "error")
                {
                    int.TryParse(healthFloorConditionString, out healthFloorCondition);
                }
                newChoiceData.healthFloorCondition = healthFloorCondition;

                newChoiceData.effectID = GetCellString(currentGeneratedChoiceIndex + DialogueSpreadSheetPatternConstants.choiceRowPattern, DialogueSpreadSheetPatternConstants.effectIDColumnPattern);

                newChoiceData.popUpTitle = GetCellString(currentGeneratedChoiceIndex + DialogueSpreadSheetPatternConstants.popUpRowPattern, DialogueSpreadSheetPatternConstants.popUpTitleColumnPattern);

                string[] retrievedPopUpContentStrings = GetCurrentSheetRow(currentGeneratedChoiceIndex + DialogueSpreadSheetPatternConstants.popUpRowPattern);
                //for (int x = 0; x < retrievedPopUpContentStrings.Length; x++)
                //{
                //    Debug.Log("POP UP WHOLE: " + retrievedPopUpContentStrings[x]);
                //}
                if (DialogueSpreadSheetPatternConstants.popUpContentColumnPattern < retrievedPopUpContentStrings.Length)
                {
                    //Debug.Log("POP UP START: " + retrievedPopUpContentStrings[DialogueSpreadSheetPatternConstants.popUpContentColumnPattern]);
                    string finalPopUpContentString = "";
                    for (int x = DialogueSpreadSheetPatternConstants.popUpContentColumnPattern; x < retrievedPopUpContentStrings.Length; x++)
                    {
                        finalPopUpContentString += retrievedPopUpContentStrings[x];
                    }
                    newChoiceData.popUpContent = finalPopUpContentString;
                }
                //else
                //{
                //    newChoiceData.popUpText = "error";
                //}
               

            }
            Debug.Log(" FOUND");
        }
        else
        {
            Debug.Log("NONE FOUND");
        }
        OnFinishedLoadingValues?.Invoke();

    }


    public void TranslateToCharacterData(CharacterData p_characterData, int p_currentGeneratedDialogueIndex, int p_characterRowPattern)
    {
        p_characterData.character = VisualNovelDatas.FindCharacter(GetCellString(p_currentGeneratedDialogueIndex + p_characterRowPattern, DialogueSpreadSheetPatternConstants.characterColumnPattern));
        p_characterData.faceEmotion = VisualNovelDatas.FindFaceEmotion(GetCellString(p_currentGeneratedDialogueIndex + p_characterRowPattern, DialogueSpreadSheetPatternConstants.faceEmotionColumnPattern));
        p_characterData.bodyEmotion = VisualNovelDatas.FindFaceEmotion(GetCellString(p_currentGeneratedDialogueIndex + p_characterRowPattern, DialogueSpreadSheetPatternConstants.bodyEmotionColumnPattern));
        p_characterData.characterPosition = VisualNovelDatas.FindBodyPosition(GetCellString(p_currentGeneratedDialogueIndex + p_characterRowPattern, DialogueSpreadSheetPatternConstants.characterPositionColumnPattern));
        p_characterData.isFlipped = VisualNovelDatas.TranslateIsFlipped(GetCellString(p_currentGeneratedDialogueIndex + p_characterRowPattern, DialogueSpreadSheetPatternConstants.isFlippedColumnPattern));
        p_characterData.isSpeaking = VisualNovelDatas.TranslateIsSpeaking(GetCellString(p_currentGeneratedDialogueIndex + p_characterRowPattern, DialogueSpreadSheetPatternConstants.isSpeakingColumnPattern));
    }

    public static string[] GetCurrentSheetRow(int p_desiredSheetRowCell)
    {
        return formattedSheetRows[p_desiredSheetRowCell].Split(new char[] { ',' });
    }

    public static string GetCellString(int p_desiredSheetRowCell, int p_desiredSheetCollumnCell)
    {
        string[] currentSheetRow = GetCurrentSheetRow(p_desiredSheetRowCell);
        //Debug.Log(p_desiredSheetRowCell + " GETTING " + p_desiredSheetCollumnCell);
        if (currentSheetRow.Length <= p_desiredSheetCollumnCell)
        {
            return "error";
        }
        return currentSheetRow[p_desiredSheetCollumnCell];
    }

    public static string GetRowString(int p_desiredSheetRowCell)
    {
        string[] currentSheetRow = GetCurrentSheetRow(p_desiredSheetRowCell);
        string resultText = "";

        for (int sheetCollumnIndex = 0; sheetCollumnIndex < currentSheetRow.Length - 1;)
        {
            resultText += currentSheetRow[sheetCollumnIndex];
            sheetCollumnIndex++;
            if (sheetCollumnIndex < currentSheetRow.Length - 1)
            {
                resultText += ",";
            }
        }
        return resultText;
    }
}
