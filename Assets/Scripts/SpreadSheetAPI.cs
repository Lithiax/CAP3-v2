using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;


#if UNITY_EDITOR
using UnityEditor;

[System.Serializable]
public class ReloadSheet
{
    public string spreadSheetName;
    public string spreadSheetID;
    public string sheetName;
}

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
    [SerializeField] List<SO_SpreadSheet> allso_SpreadSheets = new List<SO_SpreadSheet>();//"1sXdUXWGEicqD-EotO6DOppzJYMrR_rNhWJszIvgSiC4";
    [SerializeField] List<SO_SpreadSheet> alltempso_SpreadSheets = new List<SO_SpreadSheet>();//"1sXdUXWGEicqD-EotO6DOppzJYMrR_rNhWJszIvgSiC4";
    [SerializeField] List<ReloadSheet> redoSpreadSheets = new List<ReloadSheet>();//"1sXdUXWGEicqD-EotO6DOppzJYMrR_rNhWJszIvgSiC4";
    [SerializeField] string apiKey = "AIzaSyC8PPNhhTSLFqpcEUZZrGwTyl1m4e9xU-Q";

    private int currentSpreadSheetIndex;
    private int currentSheetIndex;

    [SerializeField] public string[] debuggerSheetRows;
    [SerializeField] public static string[] currentSheetRows;

    private List<int> dialogueIndexInSheet = new List<int>();
    private List<int> backgroundIndexInSheet = new List<int>();
    private List<int> choiceIndexInSheet = new List<int>();
    public  List<CodeReplacement> codeReplacements = new List<CodeReplacement>();
    public bool reloadSheet = false;
    public void Awake()
    {
        DialogueSpreadSheetPatternConstants.dialogueName = DialogueSpreadSheetPatternConstants.dialogueName.ToLower();
        DialogueSpreadSheetPatternConstants.choiceName = DialogueSpreadSheetPatternConstants.choiceName.ToLower();

        if (reloadSheet)
        {
            if (redoSpreadSheets.Count > 0)
            {
                StartCoroutine(Co_LoadValues(redoSpreadSheets[0].spreadSheetName, redoSpreadSheets[0].spreadSheetID, redoSpreadSheets[0].sheetName));
            }
        }
        else
        {
            StartCoroutine(Co_LoadValues(so_SpreadSheets[0].name, so_SpreadSheets[0].spreadSheetID, so_SpreadSheets[0].sheetNames[0]));
        }
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
            ReloadSheet r = new ReloadSheet();
            r.spreadSheetName = p_spreadSheetName;
            r.spreadSheetID = p_spreadSheetID;
            r.sheetName = p_sheetName;
            redoSpreadSheets.Add(r);
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
                //rawJson.Replace("\r\n", "[stop]");
                //rawJson.Replace("\n", " ");

                rawJson += "\n";

            }

        }

        //string newStrings = p_sheetName.Replace(" ", string.Empty);
        currentSheetRows = rawJson.Split(new char[] { '\n' });
        // currentSheetRows = rawJson.Split(new string[] { "[stop]" }, System.StringSplitOptions.None); 


        if (p_sheetName == "Interactible Choices") //Week1 orig
        {
            debuggerSheetRows = currentSheetRows;
        }
 
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
            GenerateScriptableObject(sheetNameFilePath,p_sheetName);
        }
        else if (System.IO.File.Exists(sheetNameFilePath + "/" + p_sheetName + ".asset")) //If it exist
        {
           
            if (p_sheetName == "Interactible Choices")
            {
                SO_InteractibleChoices example = Resources.Load<SO_InteractibleChoices>("Scriptable Objects/Dialogues/Visual Novel/" + p_spreadSheetName + "/" + p_sheetName);
                TranslateIntoInteractibleChoicesScriptableObject(example);

            }
            else
            {
                SO_Dialogues example = Resources.Load<SO_Dialogues>("Scriptable Objects/Dialogues/Visual Novel/" + p_spreadSheetName + "/" + p_sheetName);
                TranslateIntoScriptableObject(example);
            }
     
        }

    }



    void GenerateScriptableObject(string p_assetPath, string p_sheetName)
    {

        //Create scriptable object if it doesnt exist
        if (p_sheetName==("Interactible Choices"))
        {
            Debug.Log("MADE DIFF CKIND");
            SO_InteractibleChoices example = ScriptableObject.CreateInstance<SO_InteractibleChoices>();

            UnityEditor.AssetDatabase.CreateAsset(example, p_assetPath + "/" + p_sheetName + ".asset");
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.EditorUtility.FocusProjectWindow();
            UnityEditor.Selection.activeObject = example;

            TranslateIntoInteractibleChoicesScriptableObject(example);
        }
        else
        {
            SO_Dialogues example = ScriptableObject.CreateInstance<SO_Dialogues>();

            UnityEditor.AssetDatabase.CreateAsset(example, p_assetPath + "/" + p_sheetName + ".asset");
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.EditorUtility.FocusProjectWindow();
            UnityEditor.Selection.activeObject = example;

            TranslateIntoScriptableObject(example);

        }


    }
    public void TranslateIntoInteractibleChoicesScriptableObject(SO_InteractibleChoices p_soDialogue)
    {
        
        if (p_soDialogue.choiceDatas.Count > 0)
        {
            p_soDialogue.choiceDatas.Clear();
        }

        if (VisualNovelDatas.FindCharacter(GetCellString(0, 0)) != null)
        {
            p_soDialogue.characterData = new CharacterData();
            p_soDialogue.characterData.character = VisualNovelDatas.FindCharacter(GetCellString(0, 0));
        }
        //here go back
        //List<int> cueIndexInSheet = new List<int>();
        choiceIndexInSheet.Clear();
        for (int x = 0; x < CueType.GetValues(typeof(CueType)).Length - 1; x++)
        {
            string target = ((CueType) x).ToString().ToLower();
        
            for (int i = 0; i < currentSheetRows.Length; i++)
            {

                if (currentSheetRows[i].ToLower().Contains(target))
                {
                   // cueIndexInSheet.Add(i);
                    Debug.Log("PPPPPPP: " + i);
                    for (int y = i; y < currentSheetRows.Length; y++)
                    {
                        bool loopend = false;
                        if (currentSheetRows[y].ToLower().Contains(DialogueSpreadSheetPatternConstants.choiceName))
                        {
                            choiceIndexInSheet.Add(y);
                            Debug.Log("PPPPPPPPPPPPPPPPPPPP: " + y);
                        }
                        else
                        {
                            for (int r = 0; r < CueType.GetValues(typeof(CueType)).Length - 1; r++)
                            {
                                string rtarget = ((CueType)r).ToString().ToLower();
                                if (rtarget != target)
                                {
                                    if (currentSheetRows[y].ToLower().Contains(rtarget))
                                    {
                                        loopend = true;
                                        break;
                                    }
                                }
                               
                            }

                        }
                        if (loopend)
                        {
                            break;
                        }


                    }
                }
            
            
            }
         
            SetChoice(p_soDialogue,p_soDialogue.GetChoiceData(target), choiceIndexInSheet);
            choiceIndexInSheet.Clear();
  
        }
        EditorUtility.SetDirty(p_soDialogue);
        SaveNextSheet();

    }
    void SetChoice(ScriptableObject p_soDialogue,List<ChoiceData> p_choiceDatas, List<int> choiceIndexInSheet)
    {
        //Setting Choice
        for (int i = 0; i < choiceIndexInSheet.Count; i++)
        {
            int currentGeneratedChoiceIndex = choiceIndexInSheet[i];
            string targetWord = GetCellString(currentGeneratedChoiceIndex + DialogueSpreadSheetPatternConstants.choiceRowPattern, DialogueSpreadSheetPatternConstants.choiceNameColumnPattern);
            //if (!string.IsNullOrEmpty(targetWord))
            //{
            ChoiceData newChoiceData = null;
            newChoiceData = new ChoiceData();
            p_choiceDatas.Add(newChoiceData);

            //Setting Choice
            newChoiceData.words = targetWord;

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
            newChoiceData.effectID = GetCellString(currentGeneratedChoiceIndex + DialogueSpreadSheetPatternConstants.choiceRowPattern, DialogueSpreadSheetPatternConstants.effectIDColumnPattern);
            newChoiceData.effectReferenceName = GetCellString(currentGeneratedChoiceIndex + DialogueSpreadSheetPatternConstants.choiceRowPattern, DialogueSpreadSheetPatternConstants.effectReferecedNameColumnPattern);
            newChoiceData.isImmediateGoPhone = TranslateToBool(GetCellString(currentGeneratedChoiceIndex + DialogueSpreadSheetPatternConstants.choiceRowPattern, DialogueSpreadSheetPatternConstants.isImmediateGoPhoneColumnPattern));
          
            newChoiceData.isAutomaticEnabledColumnPattern = TranslateToBool(GetCellString(currentGeneratedChoiceIndex + DialogueSpreadSheetPatternConstants.choiceConditionRowPattern, DialogueSpreadSheetPatternConstants.isAutomaticEnabledColumnPattern));
            Debug.Log(newChoiceData.effectID + " " + newChoiceData.effectReferenceName);
            if (!string.IsNullOrEmpty(newChoiceData.effectID))
            {
                CodeReplacement newCodeReplacement = new CodeReplacement();
                newCodeReplacement.code = newChoiceData.effectID;
                newCodeReplacement.replacement = newChoiceData.effectReferenceName;
                codeReplacements.Add(newCodeReplacement);
            }
          
            newChoiceData.isHealthConditionInUseColumnPattern = TranslateToBool(GetCellString(currentGeneratedChoiceIndex + DialogueSpreadSheetPatternConstants.choiceConditionRowPattern, DialogueSpreadSheetPatternConstants.isHealthConditionInUseColumnPattern));
            int healthCeilingCondition = 0;
            string healthCeilingConditionString = GetCellString(currentGeneratedChoiceIndex + DialogueSpreadSheetPatternConstants.choiceConditionRowPattern, DialogueSpreadSheetPatternConstants.healthCeilingConditionColumnPattern);
            if (healthCeilingConditionString != "error")
            {
                int.TryParse(healthCeilingConditionString, out healthCeilingCondition);
            }
            newChoiceData.healthCeilingCondition = healthCeilingCondition;

            int healthFloorCondition = 0;
            string healthFloorConditionString = GetCellString(currentGeneratedChoiceIndex + DialogueSpreadSheetPatternConstants.choiceConditionRowPattern, DialogueSpreadSheetPatternConstants.healthFloorConditionColumnPattern);
            if (healthFloorConditionString != "error")
            {
                int.TryParse(healthFloorConditionString, out healthFloorCondition);
            }
            newChoiceData.healthFloorCondition = healthFloorCondition;

            newChoiceData.isEffectIDConditionInUseColumnPattern = TranslateToBool(GetCellString(currentGeneratedChoiceIndex + DialogueSpreadSheetPatternConstants.choiceConditionRowPattern, DialogueSpreadSheetPatternConstants.effectIDConditionColumnPattern));
            newChoiceData.effectIDCondition = GetCellString(currentGeneratedChoiceIndex + DialogueSpreadSheetPatternConstants.choiceConditionRowPattern, DialogueSpreadSheetPatternConstants.effectIDConditionColumnPattern);

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
            //}

            EditorUtility.SetDirty(p_soDialogue);

        }
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
            p_soDialogue.cueBankData.isEnabled = TranslateToBool(GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.isEnabledColumnPattern));
            p_soDialogue.cueBankData.gestureType = GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.gestureTypeColumnPattern);
            p_soDialogue.cueBankData.voiceType = GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.voiceTypeColumnPattern);
            p_soDialogue.cueBankData.bodyPostureType = GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.bodyPostureColumnPattern);
            p_soDialogue.cueBankData.eyeContactType = GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.eyeContactColumnPattern);
            p_soDialogue.cueBankData.proxemityType = GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.proxemityColumnPattern);

            //Setting Words
            string finalWords = OneString(currentGeneratedDialogueIndex + DialogueSpreadSheetPatternConstants.wordsRowPattern);

            newDialogue.words = finalWords;

            //Setting Backgounrd
            //Debug.LogError(SpreadSheetAPI.GetCellString(backgroundIndexInSheet[i]+1, DialogueSpreadSheetPatternConstants.backgroundCollumnPattern));
            newDialogue.backgroundSprite = VisualNovelDatas.FindBackgroundSprite(GetCellString(backgroundIndexInSheet[i] + 1, DialogueSpreadSheetPatternConstants.backgroundColumnPattern));
            newDialogue.specificEventType = VisualNovelDatas.FindEventType(GetCellString(backgroundIndexInSheet[i] + 1, DialogueSpreadSheetPatternConstants.eventTypeColumnPattern));
            newDialogue.specificEventParameter = GetCellString(backgroundIndexInSheet[i] + 1, DialogueSpreadSheetPatternConstants.eventParameterColumnPattern);
            newDialogue.backgroundMusic = GetCellString(backgroundIndexInSheet[i] + 1, DialogueSpreadSheetPatternConstants.backgroundMusicColumnPattern);
        }
  
        //p_soDialogue.isAutomaticHealthEvaluation = TranslateToBool(GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.isAutomaticHealthEvaluation));
        SetChoice(p_soDialogue,p_soDialogue.choiceDatas, choiceIndexInSheet);
        EditorUtility.SetDirty(p_soDialogue);
        SaveNextSheet();
  
    }
    public bool TranslateToBool(string p_name)
    {
        bool isFlipped = p_name.ToLower() == "true";
        return isFlipped;
    }

    public void ConnectAll()
    {

        TranslateCodeToReplacements();
        ConnectAllSheetsBranchDialogue();
    }

    public void SaveNextSheet()
    {
        if (!reloadSheet)
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

                    currentSpreadSheetIndex = 0;
                    currentSheetIndex = 0;


                    //TranslateCodeToReplacements();
                    //ConnectAllSheetsBranchDialogue();
                }
            }
            
        }
           
        else
        {
            currentSheetIndex++;

            if (currentSheetIndex < redoSpreadSheets.Count) //Sheet is Within Range
            {
                StartCoroutine(Co_LoadValues(redoSpreadSheets[currentSheetIndex].spreadSheetName, redoSpreadSheets[currentSheetIndex].spreadSheetID, redoSpreadSheets[currentSheetIndex].sheetName));
               
            }
            else //Sheet is exceeding Range
            {
                
                Debug.Log("ALL SHEETS RELOADED");
                

            }
        }
       
    }
  
    void TranslateCodeToReplacements()
    {
        Debug.Log("TRANSLATING ALL SHEETS CODES TO REPLACEMENT");
        CodeReplacement comma = new CodeReplacement();
        comma.code = "<comma>";
        comma.replacement = ",";
        codeReplacements.Add(comma);
        for (int localSpreadSheetIndex = 0; localSpreadSheetIndex < allso_SpreadSheets.Count; localSpreadSheetIndex++)
        {
            for (int localSheetIndex = 0; localSheetIndex < allso_SpreadSheets[localSpreadSheetIndex].sheetNames.Count; localSheetIndex++)
            {
                string spreadSheetFileLocation = "Scriptable Objects/Dialogues/Visual Novel/" + allso_SpreadSheets[localSpreadSheetIndex].name + "/";
                if (allso_SpreadSheets[localSpreadSheetIndex].sheetNames[localSheetIndex] != "Interactible Choices")
                {
                    Debug.Log(allso_SpreadSheets[localSpreadSheetIndex].name + " : " + allso_SpreadSheets[localSpreadSheetIndex].sheetNames[localSheetIndex]);
                    SO_Dialogues currentSheetSODialogue = Resources.Load<SO_Dialogues>(spreadSheetFileLocation + allso_SpreadSheets[localSpreadSheetIndex].sheetNames[localSheetIndex]);
                    for (int i = 0; i < currentSheetSODialogue.dialogues.Count; i++)
                    {
                        for (int x = 0; x < codeReplacements.Count; x++)
                        {
                            currentSheetSODialogue.dialogues[i].words = currentSheetSODialogue.dialogues[i].words.Replace(codeReplacements[x].code, codeReplacements[x].replacement);
                            EditorUtility.SetDirty(currentSheetSODialogue);
                        }

                    }
                    for (int i = 0; i < currentSheetSODialogue.choiceDatas.Count; i++)
                    {
                        for (int x = 0; x < codeReplacements.Count; x++)
                        {
                            if (codeReplacements[x].code == "<comma>")
                            {
                                Debug.Log("COMMA " + codeReplacements[x].replacement);
                            
                            }
                            Debug.Log("COMMA " + currentSheetSODialogue.choiceDatas[i].words);
                            
                            if (!string.IsNullOrEmpty(currentSheetSODialogue.choiceDatas[i].words))
                            {
                                currentSheetSODialogue.choiceDatas[i].words = currentSheetSODialogue.choiceDatas[i].words.Replace(codeReplacements[x].code, codeReplacements[x].replacement);
                            }
                            if (!string.IsNullOrEmpty(currentSheetSODialogue.choiceDatas[i].popUpTitle))
                            {
                                currentSheetSODialogue.choiceDatas[i].popUpTitle = currentSheetSODialogue.choiceDatas[i].popUpTitle.Replace(codeReplacements[x].code, codeReplacements[x].replacement);

                            }
                            if (!string.IsNullOrEmpty(currentSheetSODialogue.choiceDatas[i].popUpContent))
                            {
                                currentSheetSODialogue.choiceDatas[i].popUpContent = currentSheetSODialogue.choiceDatas[i].popUpContent.Replace(codeReplacements[x].code, codeReplacements[x].replacement);
                            }
                           
                            EditorUtility.SetDirty(currentSheetSODialogue);
                        }

                    }
                }
                   
            }
        }
        Debug.Log("FULLY TRANSLATED EVERYTHING");
    }

    void ConnectAllSheetsBranchDialogue()
    {
        Debug.Log("CONNECTING ALL SHEETS BRANCH DIALOGUE");
        for (int localSpreadSheetIndex = 0; localSpreadSheetIndex < allso_SpreadSheets.Count; localSpreadSheetIndex++)
        {
            for (int localSheetIndex = 0; localSheetIndex < allso_SpreadSheets[localSpreadSheetIndex].sheetNames.Count; localSheetIndex++)
            {
                string spreadSheetFileLocation = "Scriptable Objects/Dialogues/Visual Novel/" + allso_SpreadSheets[localSpreadSheetIndex].name + "/";
                if (allso_SpreadSheets[localSpreadSheetIndex].sheetNames[localSheetIndex] != "Interactible Choices")
                {
                    Debug.Log(allso_SpreadSheets[localSpreadSheetIndex].sheetNames[localSheetIndex]);
                    SO_Dialogues currentSheetSODialogue = Resources.Load<SO_Dialogues>(spreadSheetFileLocation + allso_SpreadSheets[localSpreadSheetIndex].sheetNames[localSheetIndex]);
                    for (int i = 0; i < currentSheetSODialogue.choiceDatas.Count; i++)
                    {
                        Debug.Log(allso_SpreadSheets[localSpreadSheetIndex].name + " - " + currentSheetSODialogue.name);
                        if (currentSheetSODialogue.choiceDatas[i].effectID == "<VN>")
                        {
                            string[] sheetDivided = currentSheetSODialogue.choiceDatas[i].branchDialogueName.Split('/');

                            currentSheetSODialogue.choiceDatas[i].branchDialogue = FindSODialogue(sheetDivided[0], sheetDivided[1]);
                        }
                        else if (currentSheetSODialogue.choiceDatas[i].branchDialogueName != "<Phone>")
                        {
                            if (!string.IsNullOrEmpty(currentSheetSODialogue.choiceDatas[i].branchDialogueName))
                            {
                                currentSheetSODialogue.choiceDatas[i].branchDialogue = FindSODialogue(allso_SpreadSheets[localSpreadSheetIndex].name, currentSheetSODialogue.choiceDatas[i].branchDialogueName);
                            }
                                
                        }
                      
                        EditorUtility.SetDirty(currentSheetSODialogue);
                    }
                }
                else
                {
                    Debug.Log(allso_SpreadSheets[localSpreadSheetIndex].sheetNames[localSheetIndex]);
                    SO_InteractibleChoices currentSheetSODialogue = Resources.Load<SO_InteractibleChoices>(spreadSheetFileLocation + allso_SpreadSheets[localSpreadSheetIndex].sheetNames[localSheetIndex]);
                    //for (int i = 0; i < currentSheetSODialogue.choiceDatas.Count; i++)
                    //{
                        Debug.Log(allso_SpreadSheets[localSpreadSheetIndex].name + " - " + currentSheetSODialogue.name);
                    if(allso_SpreadSheets[localSpreadSheetIndex].canAbruptEnd)
                    {
                        currentSheetSODialogue.deathSheet = FindSODialogue(allso_SpreadSheets[localSpreadSheetIndex].name, allso_SpreadSheets[localSpreadSheetIndex].sheetNames[allso_SpreadSheets[localSpreadSheetIndex].sheetNames.Count - 1]);
                    }

                    for (int x = 0; x < CueType.GetValues(typeof(CueType)).Length - 1; x++)
                        {
                            string target = ((CueType)x).ToString().ToLower();
                            List<ChoiceData> selectedChoiceDatas = currentSheetSODialogue.GetChoiceData(target);
                            Debug.Log("ddd" + target + " - "+ selectedChoiceDatas.Count);
                            for (int w =0; w < selectedChoiceDatas.Count;w++)
                            {
                                selectedChoiceDatas[w].branchDialogue = FindSODialogue(allso_SpreadSheets[localSpreadSheetIndex].name, selectedChoiceDatas[w].branchDialogueName);
                            }
                     
                        }
                       

                        EditorUtility.SetDirty(currentSheetSODialogue);
                    //}
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
        p_characterData.isSpeaking = TranslateToBool(GetCellString(p_currentGeneratedDialogueIndex + p_characterRowPattern, DialogueSpreadSheetPatternConstants.isSpeakingColumnPattern));
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

    public static string OneString(int p_desiredSheetRowCell)
    {
        string[] retrievedWords = GetCurrentSheetRow(p_desiredSheetRowCell);
        string finalWords = "";
        for (int x = 0; x < retrievedWords.Length; x++)
        {
            finalWords += retrievedWords[x];
        }
        return finalWords;
    }


}
#endif
