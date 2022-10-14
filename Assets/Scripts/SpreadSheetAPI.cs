using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;


#if UNITY_EDITOR
using UnityEditor;


public class SpreadSheetAPI : MonoBehaviour
{
    //Google sheet >>> File >>> Share >>> Publish to web
    //Share >>> Restricted to Anyone can have access
    //https://docs.google.com/spreadsheets/d/e/2PACX-1vR1_kBC9z3QpoGu5IG_ya1ZodxPQf63vyjtZdWTPadbo-Up8Qj6xyEM4UzQ8fpP-7SadbNU2VlJI-fV/pubhtml

    //Google Credentials https://console.cloud.google.com/apis/credentials
    //SimpleJSON JSON Parser Library Package https://drive.google.com/drive/folders/126VXfBnr-K6KCeTS4RqPJsatl_tfReiL
    //Published Google Sheet Format https://sheets.googleapis.com/v4/spreadsheets/<Spreadsheet id>/values/<Sheet name>?key=<API key>

    //API Key: AIzaSyC8PPNhhTSLFqpcEUZZrGwTyl1m4e9xU-Q
    //"https://sheets.googleapis.com/v4/spreadsheets/" + spreadSheetID + "/values/" + p_sheetName + "?key=" + apiKey);

    [SerializeField] List<SO_SpreadSheet> so_SpreadSheets = new List<SO_SpreadSheet>();//"1sXdUXWGEicqD-EotO6DOppzJYMrR_rNhWJszIvgSiC4";

    [SerializeField] string apiKey = "AIzaSyC8PPNhhTSLFqpcEUZZrGwTyl1m4e9xU-Q";

    private int currentSpreadSheetIndex;
    private int currentSheetIndex;

    [SerializeField] public static string[] currentSheetRows;

    private List<int> dialogueIndexInSheet = new List<int>();
    private List<int> backgroundIndexInSheet = new List<int>();
    private List<int> choiceIndexInSheet = new List<int>();

    public void Awake()
    {
        DialogueSpreadSheetPatternConstants.dialogueName = DialogueSpreadSheetPatternConstants.dialogueName.ToLower();
        DialogueSpreadSheetPatternConstants.choiceName = DialogueSpreadSheetPatternConstants.choiceName.ToLower();
        StartCoroutine(Co_LoadValues(so_SpreadSheets[0].name, so_SpreadSheets[0].spreadSheetID,so_SpreadSheets[0].sheetNames[0]));
    
    }

    public IEnumerator Co_LoadValues(string p_spreadSheetName, string p_spreadSheetID, string p_sheetName)
    {
        string rawJson = "";

        UnityWebRequest www = UnityWebRequest.Get("https://sheets.googleapis.com/v4/spreadsheets/" + p_spreadSheetID + "/values/" + p_sheetName + "?key=" + apiKey);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError ||
            www.result == UnityWebRequest.Result.ProtocolError ||
            www.timeout > 2)  //Computer has no internet access/cannot connect to website, opt to use json file saved
        {
           
            Debug.Log(p_sheetName + " CURRENTLY OFFLINE DUE TO ERROR: " + www.error);
            Debug.Log("Spread Sheet Name: " + p_spreadSheetName + " Spread Sheet ID: " + p_spreadSheetID + " Sheet Name: " + p_sheetName);
        }
        else//Computer has internet access/can connect to website, opt to update the previous json file saved with new values from web
        {
            string rawSheet = www.downloadHandler.text;


            foreach (var rawSheetRowsValuesObject in JSON.Parse(rawSheet)["values"]) //Only get values in sheet
            {
                var rawSheetRowsObject = JSON.Parse(rawSheetRowsValuesObject.ToString());

                List<string> rawSheetRowsStrings;
                rawSheetRowsStrings = rawSheetRowsObject[0].AsStringList;

                foreach (string cell in rawSheetRowsStrings) //Adding every values to a single thread of string
                {
                    rawJson += cell + ",";
                }
                rawJson += "\n";

            }
          
        }
      
        string newStrings = p_sheetName.Replace(" ", string.Empty);
        currentSheetRows = rawJson.Split(new char[] { '\n' });
        FindScriptableObject(p_spreadSheetName, p_sheetName);
    }

    void FindScriptableObject(string p_spreadSheetName, string p_sheetName)
    {
        string sheetNameFilePath = "Assets/Resources/Scriptable Objects/Dialogues/Visual Novel/" + p_spreadSheetName;
        //Create Parent Directory if it doesnt exist
        if (!System.IO.Directory.Exists(sheetNameFilePath))
        {
            System.IO.Directory.CreateDirectory(sheetNameFilePath);

        }
        if (!System.IO.File.Exists(sheetNameFilePath + "/" + p_sheetName + ".asset"))  //If it doesnt exist
        {
            GenerateScriptableObject(sheetNameFilePath + "/" + p_sheetName + ".asset");
        }
        else if (System.IO.File.Exists(sheetNameFilePath + "/" + p_sheetName + ".asset")) //If it exist
        {
            SO_Dialogues example = Resources.Load<SO_Dialogues>("Scriptable Objects/Dialogues/Visual Novel/" + p_spreadSheetName + "/" + p_sheetName);
            TranslateIntoScriptableObject(example);
        }

    }

    void GenerateScriptableObject(string p_assetPath)
    {
        
        //Create scriptable object if it doesnt exist
        SO_Dialogues example = ScriptableObject.CreateInstance<SO_Dialogues>();
      
        UnityEditor.AssetDatabase.CreateAsset(example, p_assetPath);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
        UnityEditor.EditorUtility.FocusProjectWindow();
        UnityEditor.Selection.activeObject = example;

        TranslateIntoScriptableObject(example);
    }
    public void TranslateIntoScriptableObject(SO_Dialogues p_soDialogue)
    {
        dialogueIndexInSheet.Clear();
        choiceIndexInSheet.Clear();
        backgroundIndexInSheet.Clear();

        for (int i = 0; i < currentSheetRows.Length; i++)
        {
            if (currentSheetRows[i].ToLower().Contains(DialogueSpreadSheetPatternConstants.dialogueName))
            {
                dialogueIndexInSheet.Add(i);
            }
            else if (currentSheetRows[i].ToLower().Contains(DialogueSpreadSheetPatternConstants.choiceName))
            {
                choiceIndexInSheet.Add(i);
            }
            else if (currentSheetRows[i].ToLower().Contains("background sprite"))
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
            p_soDialogue.cueBankData.isEnabled = VisualNovelDatas.TranslateIsSpeaking(GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.isEnabledColumnPattern));
            p_soDialogue.cueBankData.hapticType = VisualNovelDatas.FindHapticType(GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.hapticTypeColumnPattern));
            p_soDialogue.cueBankData.vocalicType = VisualNovelDatas.FindVocalicType(GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.vocalicTypeColumnPattern));
            p_soDialogue.cueBankData.kinesicType = VisualNovelDatas.FindKinesicType(GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.kinesicColumnPattern));
            p_soDialogue.cueBankData.oculesicType = VisualNovelDatas.FindOculesicType(GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.oculesicColumnPattern));
            p_soDialogue.cueBankData.physicalApperanceType = VisualNovelDatas.FindPhysicalAppearanceType(GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.physicalAppearanceColumnPattern));

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
        p_soDialogue.isAutomaticHealthEvaluation = VisualNovelDatas.TranslateIsSpeaking(GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.isAutomaticHealthEvaluation));
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
            if (branchingDialogueName == "error")
            {
                branchingDialogueName = "";
            }
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
        SaveNextSheet();
  
    }

    public void SaveNextSheet()
    {
        currentSheetIndex++;
       
        if (currentSheetIndex < so_SpreadSheets[currentSpreadSheetIndex].sheetNames.Count) //Sheet is Within Range
        {
            StartCoroutine(Co_LoadValues(so_SpreadSheets[currentSpreadSheetIndex].name, so_SpreadSheets[currentSpreadSheetIndex].spreadSheetID, so_SpreadSheets[currentSpreadSheetIndex].sheetNames[currentSheetIndex]));
        }
        else //Sheet is exceeding Range
        {
            currentSpreadSheetIndex++;
            currentSheetIndex = 0;
            if (currentSpreadSheetIndex < so_SpreadSheets.Count)
            {
                
                StartCoroutine(Co_LoadValues(so_SpreadSheets[currentSpreadSheetIndex].name, so_SpreadSheets[currentSpreadSheetIndex].spreadSheetID, so_SpreadSheets[currentSpreadSheetIndex].sheetNames[currentSheetIndex]));
            }
            else
            {
                Debug.Log("ALL SHEETS LOADED");
                Debug.Log("CONNECTING ALL SHEETS BRANCH DIALOGUE");
                ConnectAllSheetsBranchDialogue();
            }
        }
    }

    void ConnectAllSheetsBranchDialogue()
    {

        for (int localSpreadSheetIndex = 0; localSpreadSheetIndex < so_SpreadSheets.Count; localSpreadSheetIndex++)
        {
            for (int localSheetIndex = 0; localSheetIndex < so_SpreadSheets[localSpreadSheetIndex].sheetNames.Count; localSheetIndex++)
            {
                string spreadSheetFileLocation = "Scriptable Objects/Dialogues/Visual Novel/" + so_SpreadSheets[localSpreadSheetIndex].name + "/";
                SO_Dialogues currentSheetSODialogue = Resources.Load<SO_Dialogues>(spreadSheetFileLocation + so_SpreadSheets[localSpreadSheetIndex].sheetNames[localSheetIndex]);
                for (int i = 0; i < currentSheetSODialogue.choiceDatas.Count; i++)
                {
                    Debug.Log(so_SpreadSheets[localSpreadSheetIndex].name + " - " + currentSheetSODialogue.name);
                    currentSheetSODialogue.choiceDatas[i].branchDialogue = FindSODialogue(so_SpreadSheets[localSpreadSheetIndex].name,currentSheetSODialogue.choiceDatas[i].branchDialogueName);
                    EditorUtility.SetDirty(currentSheetSODialogue);
                }
            }
        }
        Debug.Log("FULLY CONNECTED EVERYTHING");
    }

    SO_Dialogues FindSODialogue(string p_so_SpreadSheetName, string p_branchDialogueName)
    {
        string spreadSheetFileLocation = "Scriptable Objects/Dialogues/Visual Novel/" + p_so_SpreadSheetName + "/";
        SO_Dialogues currentSheetSODialogue = Resources.Load<SO_Dialogues>(spreadSheetFileLocation + p_branchDialogueName);
        if (currentSheetSODialogue != null)
        {
            return currentSheetSODialogue;
        }
        else
        {
            Debug.LogWarning("DIDNT FIND ANYTHING FOR: " + spreadSheetFileLocation + p_branchDialogueName);
            return null;
        }

 
    }

    void TranslateToCharacterData(CharacterData p_characterData, int p_currentGeneratedDialogueIndex, int p_characterRowPattern)
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
        return currentSheetRows[p_desiredSheetRowCell].Split(new char[] { ',' });
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
#endif
