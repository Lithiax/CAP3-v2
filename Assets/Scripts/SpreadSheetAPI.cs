using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class SpreadSheetAPI : MonoBehaviour
{
    //Google sheet >>> File >>> Share >>> Publish to web
    //Share >>> Restricted to Anyone can have access

    //Google Credentials https://console.cloud.google.com/apis/credentials
    //SimpleJSON JSON Parser Library Package https://drive.google.com/drive/folders/126VXfBnr-K6KCeTS4RqPJsatl_tfReiL
    //Published Google Sheet Format https://sheets.googleapis.com/v4/spreadsheets/<Spreadsheet id>/values/<Sheet name>?key=<API key>

    //New:https://sheets.googleapis.com/v4/spreadsheets/18Z4onzDE8gProsuP7TYJBkRCdYykBw9IstRoFffS_KM/values/Week1?key=AIzaSyBsDkCdfQlc4nieOTYf6eAq3xYafiCiEOM
    //new api key: AIzaSyBsDkCdfQlc4nieOTYf6eAq3xYafiCiEOM

    public static SpreadSheetAPI instance;

    [SerializeField] private string fileName;

    [SerializeField] private string sheetNameTester = "Settings";

    public bool temp = false;
    //[SerializeField]
    //public int currentIndex;

    //[SerializeField]
    //public List<string> sheets = new List<string>();

    bool isUpdatingEditorValues = true;
    public static Action OnFinishedLoadingValues;

    [SerializeField] public string[] testerFormattedSheetRows;
    [SerializeField] private static string[] formattedSheetRows;

    [SerializeField]
    public List<int> dialogueIndexInSheet = new List<int>();
    [SerializeField]
    public List<int> backgroundIndexInSheet = new List<int>();
    [SerializeField]
    public List<int> choiceIndexInSheet = new List<int>();

    public bool firstTime = true;
    public string currentSheetName;

    List<string> sheetNames = new List<string>();
    public void Awake()
    {
        instance = this;
        StartCoroutine(Co_LoadValues(sheetNameTester));
    
    }

    public void Reload()
    {
        StartCoroutine(Co_LoadValues(sheetNameTester));
    }

    public static void SetCurrentIndexToSheet(string p_sheetName)
    {
        instance.StartCoroutine(instance.Co_LoadValues(p_sheetName));

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

    public IEnumerator Co_LoadValues(string p_sheetName)
    {
        Debug.Log("HERE");
        if (isUpdatingEditorValues)
        {
            string rawJson = "";


            UnityWebRequest www = UnityWebRequest.Get("https://sheets.googleapis.com/v4/spreadsheets/18Z4onzDE8gProsuP7TYJBkRCdYykBw9IstRoFffS_KM/values/" + p_sheetName + "?key=AIzaSyBsDkCdfQlc4nieOTYf6eAq3xYafiCiEOM");
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError ||
                www.timeout > 2)  //Computer has no internet access/cannot connect to website, opt to use json file saved
            {

                Debug.Log(p_sheetName + " CURRENTLY OFFLINE DUE TO ERROR: " + www.error);
                rawJson = JSONFileHandler.ReadFromJSON(fileName);

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
                JSONFileHandler.SaveToJSON(fileName, rawJson);
          
            }
            formattedSheetRows = rawJson.Split(new char[] { '\n' });
            testerFormattedSheetRows = formattedSheetRows;//tester;

        }
        else
        {

        }
        if (firstTime)
        {
            firstTime = false;
            CheckSheets();
        }
        else
        {
            TransitionUI.instance.tester.text = "TRYING TO LOAD: ";
            if (Resources.Load<SO_Dialogues>("Scriptable Objects/" + currentSheetName + "/" + p_sheetName)) //System.IO.File.Exists("Assets/Resources/Scriptable Objects/" + currentSheetName + "/" + p_sheetName + ".asset"))
            {
      
                SO_Dialogues roomCache = Resources.Load<SO_Dialogues>("Scriptable Objects/" + currentSheetName + "/" + p_sheetName);
                TranslateIntoScriptableObject(roomCache);
                Debug.Log("EXISTS");
                TransitionUI.instance.tester.text = "EXISTS ";
            }
            else
            {
                TransitionUI.instance.tester.text = "doesnt exist ";
            }
        }
       

    }

    public void CheckSheets()
    {
        currentSheetName = SpreadSheetAPI.instance.testerFormattedSheetRows[1].Substring(0, SpreadSheetAPI.instance.testerFormattedSheetRows[1].Length - 1);
        sheetNames.Clear();
        for (int i = 3; i < SpreadSheetAPI.instance.testerFormattedSheetRows.Length; i++)
        {
            if (SpreadSheetAPI.instance.testerFormattedSheetRows[i] != "")
            {
                if (!SpreadSheetAPI.instance.testerFormattedSheetRows[i].ToLower().Contains("end sheets"))
                {

                    string newName = SpreadSheetAPI.instance.testerFormattedSheetRows[i].Substring(0, SpreadSheetAPI.instance.testerFormattedSheetRows[i].Length - 1);
                    Debug.Log(SpreadSheetAPI.instance.testerFormattedSheetRows[i] + " - " + newName);
                    sheetNames.Add(newName);
                }
                else if (SpreadSheetAPI.instance.testerFormattedSheetRows[i].ToLower().Contains("end sheets"))
                {
                    Debug.Log("BREAKING");
                    break;
                }
            }
          
        }
       // currentSheetName = currentSheetName.Substring(0, currentSheetName.Length - 1);
        string sheetNameFilePath = "Assets/Resources/Scriptable Objects/" + currentSheetName;
        if (System.IO.Directory.Exists(sheetNameFilePath))
        {

        }
        else
        {
            Debug.Log("CREATING NEW FOLDER");
            System.IO.Directory.CreateDirectory(sheetNameFilePath);

        }
        
  
        for (int i = 0; i < sheetNames.Count; i++)
        {
            //if (System.IO.File.Exists(sheetNameFilePath+"/"+ sheetNames[i] +".asset"))
            //{
            //    SO_Dialogues roomCache = Resources.Load<SO_Dialogues>("Scriptable Objects/" + currentSheetName + "/" + sheetNames[i] + ".asset");
            //    TranslateIntoScriptableObject(roomCache);
            //    Debug.Log("EXISTS");
            //}
            //i++;
            //if (i >= sheetNames.Count)
            //    {

            //}
            //else 
            if (!System.IO.File.Exists(sheetNameFilePath + "/" + sheetNames[i] + ".asset"))
            {
                //Create scriptable object if it doesnt exist
                SO_Dialogues example = ScriptableObject.CreateInstance<SO_Dialogues>();
                // path has to start at "Assets"

                //#if UNITY_EDITOR
                //AssetDatabase.CreateAsset(example, sheetNameFilePath + "/" + sheetNames[i] + ".asset");
                //AssetDatabase.SaveAssets();
                //AssetDatabase.Refresh();
                //EditorUtility.FocusProjectWindow();
                //Selection.activeObject = example;
                //#endif //BRING BACK


                Debug.Log("CREATED NEW SCRIPTABLE OBJECT: " + example.name + " IN PATH: " + sheetNameFilePath);
    
            }
            //Update sheet
            if (Resources.Load < SO_Dialogues >("Scriptable Objects/" + currentSheetName + "/" + sheetNames[i]))//System.IO.File.Exists(sheetNameFilePath + "/" + sheetNames[i] + ".asset"))
            {
                if (i == 0)
                {
                    StorylineManager.instance.temp.currentSO_Dialogues =Resources.Load<SO_Dialogues>("Scriptable Objects/" + currentSheetName + "/" + sheetNames[i]);
                }
   
                StartCoroutine(Co_LoadValues(sheetNames[i]));
                //SO_Dialogues roomCache = Resources.Load<SO_Dialogues>("Scriptable Objects/" + currentSheetName + "/" + sheetNames[i]);
                //TranslateIntoScriptableObject(roomCache);
                Debug.Log("try exist");

            }
        }
        TransitionUI.instance.tester.text = "la: ";

        //SO_Dialogues[] roomCache;

        //roomCache = Resources.LoadAll<SO_Dialogues>("ScriptableObject/" + currentSheetName);


        //for (int i = 0; i < roomCache.Length; i++)
        //{

        //}
        //OnFinishedLoadingValues?.Invoke();

    }
    public void TranslateIntoScriptableObject(SO_Dialogues p_soDialogue)
    {
        TransitionUI.instance.tester.text = "traa: ";
        if (p_soDialogue != null)
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
                string characterOne = SpreadSheetAPI.GetCellString(currentGeneratedDialogueIndex + DialogueSpreadSheetPatternConstants.characterOneRowPattern, DialogueSpreadSheetPatternConstants.characterCollumnPattern).ToLower();
                string characterTwo = SpreadSheetAPI.GetCellString(currentGeneratedDialogueIndex + DialogueSpreadSheetPatternConstants.characterTwoRowPattern, DialogueSpreadSheetPatternConstants.characterCollumnPattern).ToLower();
                string characterThree = SpreadSheetAPI.GetCellString(currentGeneratedDialogueIndex + DialogueSpreadSheetPatternConstants.characterThreeRowPattern, DialogueSpreadSheetPatternConstants.characterCollumnPattern).ToLower();

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
                p_soDialogue.isEnabled = VisualNovelDatas.TranslateIsSpeaking(SpreadSheetAPI.GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.isEnabledCollumnPattern));
                p_soDialogue.hapticType = VisualNovelDatas.FindHapticType(SpreadSheetAPI.GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.hapticTypeCollumnPattern));
                p_soDialogue.vocalicType = VisualNovelDatas.FindVocalicType(SpreadSheetAPI.GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.vocalicTypeCollumnPattern));
                p_soDialogue.kinesicType = VisualNovelDatas.FindKinesicType(SpreadSheetAPI.GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.kinesicCollumnPattern));
                p_soDialogue.oculesicType = VisualNovelDatas.FindOculesicType(SpreadSheetAPI.GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.oculesicCollumnPattern));
                p_soDialogue.physicalApperanceType = VisualNovelDatas.FindPhysicalAppearanceType(SpreadSheetAPI.GetCellString(DialogueSpreadSheetPatternConstants.cueBankRowPattern, DialogueSpreadSheetPatternConstants.physicalAppearanceCollumnPattern));

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
                newDialogue.backgroundSprite = VisualNovelDatas.FindBackgroundSprite(SpreadSheetAPI.GetCellString(backgroundIndexInSheet[i] + 1, DialogueSpreadSheetPatternConstants.backgroundCollumnPattern));


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
                newChoiceData.words = SpreadSheetAPI.GetCellString(currentGeneratedChoiceIndex + DialogueSpreadSheetPatternConstants.choiceRowPattern, DialogueSpreadSheetPatternConstants.choiceNameCollumnPattern);
                //newChoiceData.branchDialogueName = SpreadSheetAPI.GetCellString(currentGeneratedChoiceIndex + DialogueSpreadSheetPatternConstants.choiceRowPattern, DialogueSpreadSheetPatternConstants.nextDialogueSheetNameCollumnPattern);
                string branchingDialogueName = SpreadSheetAPI.GetCellString(currentGeneratedChoiceIndex + DialogueSpreadSheetPatternConstants.choiceRowPattern, DialogueSpreadSheetPatternConstants.nextDialogueSheetNameCollumnPattern);
                if (System.IO.File.Exists("Assets/Resources/Scriptable Objects/" + currentSheetName + branchingDialogueName + ".asset"))
                {
                    SO_Dialogues roomCache = Resources.Load<SO_Dialogues>("Scriptable Objects/" + currentSheetName + "/" + branchingDialogueName);
                    newChoiceData.branchingToSO_Dialogues = roomCache;


                }
                    
                int test = 0;
                string testnum = SpreadSheetAPI.GetCellString(currentGeneratedChoiceIndex + DialogueSpreadSheetPatternConstants.choiceRowPattern, DialogueSpreadSheetPatternConstants.choiceDamagePattern);
                if (testnum != "error")
                {
                    int.TryParse(testnum, out test);
                }
                newChoiceData.damage = test;
                newChoiceData.eventID = SpreadSheetAPI.GetCellString(currentGeneratedChoiceIndex + DialogueSpreadSheetPatternConstants.choiceRowPattern, DialogueSpreadSheetPatternConstants.eventID);
                int healthCondition = 0;
                string healthConditionString = SpreadSheetAPI.GetCellString(currentGeneratedChoiceIndex + DialogueSpreadSheetPatternConstants.choiceRowPattern, DialogueSpreadSheetPatternConstants.healthCondition);
                if (healthConditionString != "error")
                {
                    int.TryParse(healthConditionString, out healthCondition);
                }
                newChoiceData.healthCondition = healthCondition;
            }
            Debug.Log(" FOUND");
        }
        else
        {
            Debug.Log("NONE FOUND");
        }
        CheckAll();

    }

    public void CheckAll()
    {
        int count = 0;
        if (!temp)
        {
            SO_Dialogues test = Resources.Load<SO_Dialogues>("Scriptable Objects/" + currentSheetName + "/week1");
            if (test) //System.IO.File.Exists("Assets/Resources/Scriptable Objects/" + currentSheetName + "/" + p_sheetName + ".asset"))
            {
                if (test.dialogues.Count==0 && test.choiceDatas.Count == 0)
                {
          
                }
                else
                {
                    temp = true;
                    OnFinishedLoadingValues?.Invoke();
                    TransitionUI.instance.tester.text = "good";
                }
      
                //            temp = true;
                //            OnFinishedLoadingValues?.Invoke();
            }

            //System.IO.DirectoryInfo d = new System.IO.DirectoryInfo("Assets/Resources/Scriptable Objects/" + currentSheetName);
            //System.IO.FileInfo[] fis = d.GetFiles();
            //foreach (System.IO.FileInfo fi in fis)
            //{
            //    if (fi.Extension.Equals(".asset", StringComparison.OrdinalIgnoreCase))
            //    {
            //        count++;
            //    }

            //}

            //Debug.Log("COUNTER FOUND: " + count);
            //TransitionUI.instance.tester.text = count.ToString();
            //for (int i = 0; i < count;)
            //{
            //    if (System.IO.File.Exists("Assets/Resources/Scriptable Objects/" + currentSheetName + "/" + "week1" + ".asset"))
            //    // if (System.IO.File.Exists("Assets/Resources/Scriptable Objects/" + currentSheetName + "/" + sheetNames[i] + ".asset"))
            //    {
            //        SO_Dialogues roomCache = Resources.Load<SO_Dialogues>("Scriptable Objects/" + currentSheetName + "/" + "week1");
            //        //SO_Dialogues roomCache = Resources.Load<SO_Dialogues>("Scriptable Objects/" + currentSheetName + "/" + sheetNames[i]);
            //        if (roomCache.dialogues.Count == 0 && roomCache.choiceDatas.Count == 0)
            //        {
            //            Debug.Log(roomCache.name + "DDD");
            //            break;
            //        }
            //        i++;
            //        if (i >= count)
            //        {
            //            Debug.Log("TRYING TO START");
            //            TransitionUI.instance.tester.text = "good";
            //            temp = true;
            //            OnFinishedLoadingValues?.Invoke();
            //        }


            //    }
            //    Debug.Log("COUNTER ENDED: " + count);
            //}
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
}
