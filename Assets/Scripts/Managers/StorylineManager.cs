using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
[System.Serializable]
public class CodeReplacement
{
    public string code;
    public string replacement;
}
[System.Serializable]
public class LocalCueChoice
{
    public bool wasChosen;
    public ChoiceData choiceData;
}
[System.Serializable]
public class CueChoice
{
    [SerializeField]
    public CueType cueType;
    public List<LocalCueChoice> cueChoiceDatas = new List<LocalCueChoice>();

}
public class StorylineManager : MonoBehaviour, IDataPersistence
{
    public static bool firstTime = false;
    public static Action OnLoadedEvent;
    public static void LoadVisualNovel(SO_Dialogues so_Dialogue,SO_InteractibleChoices so_interactable)
    {
        SO_Character mainCharacter = Resources.Load<SO_Character>("Scriptable Objects/Characters/You");
        mainCharacter.stageName = "You";

        StorylineManager.currentSO_Dialogues = so_Dialogue;
        StorylineManager.so_InteractibleChoices = so_interactable;
        if (so_InteractibleChoices.deathSheet != null)
        {
            StorylineManager.currentZeroSO_Dialogues = so_InteractibleChoices.deathSheet;
        }
        else
        {
            StorylineManager.currentZeroSO_Dialogues = null;
        }

        if (so_InteractibleChoices.characterData != null)
        {
            DialogueSpreadSheetPatternConstants.cueCharacter = so_InteractibleChoices.characterData.character;
        }
        else
        {
            DialogueSpreadSheetPatternConstants.cueCharacter = null;
        }

        StorylineManager.loggedWords.Clear();
        StorylineManager.currentDialogueIndex = 0;
        sideDialogue = false;
        savedDialogueIndex = -1;
        savedSO_Dialogues = null;

        currentBackgroundMusic = "";
        paused = false;
        for (int i = 0; i < so_InteractibleChoices.choiceDatas.Count; i++)
        {
            CueChoice newCueChoice = new CueChoice();
            StorylineManager.cuesChoices.Add(newCueChoice);
            newCueChoice.cueType = so_InteractibleChoices.choiceDatas[i].cueType;
            for (int x = 0; x < so_InteractibleChoices.choiceDatas[i].choiceDatas.Count; x++)
            {
                LocalCueChoice newLocalCueChoice = new LocalCueChoice();
                newCueChoice.cueChoiceDatas.Add(newLocalCueChoice);
                newLocalCueChoice.choiceData = so_InteractibleChoices.choiceDatas[i].choiceDatas[x];
                newLocalCueChoice.wasChosen = false;
            }


        }

        if (StorylineManager.currentSO_Dialogues != null)
        {
            Debug.Log("DIALOGUE LOADED");
            CharacterDialogueUI.onCharacterSpokenTo.Invoke();

        }
        else
        {
            Debug.Log("IT DOESNT WORK");
        }

    }
    public static void LoadVisualNovel(string folderField, string sheetField)
    {
        SO_Character mainCharacter = Resources.Load<SO_Character>("Scriptable Objects/Characters/You");
        mainCharacter.stageName = "You";
        
        StorylineManager.currentSO_Dialogues = Resources.Load<SO_Dialogues>("Scriptable Objects/Dialogues/Visual Novel/" + folderField + "/" + sheetField);
        StorylineManager.so_InteractibleChoices = Resources.Load<SO_InteractibleChoices>("Scriptable Objects/Dialogues/Visual Novel/" + folderField + "/" + "Interactible Choices");
        if (so_InteractibleChoices.deathSheet != null)
        {
            StorylineManager.currentZeroSO_Dialogues = so_InteractibleChoices.deathSheet;
        }
        else
        {
            StorylineManager.currentZeroSO_Dialogues = null;
        }

        if (so_InteractibleChoices.characterData != null)
        {
            DialogueSpreadSheetPatternConstants.cueCharacter = so_InteractibleChoices.characterData.character;
        }
        else
        {
            DialogueSpreadSheetPatternConstants.cueCharacter = null;
        }
   
        StorylineManager.loggedWords.Clear();
        StorylineManager.currentDialogueIndex = 0;
        sideDialogue = false;
        savedDialogueIndex = -1;
        savedSO_Dialogues = null;

        currentBackgroundMusic = "";
        paused = false;
        for (int i = 0; i< so_InteractibleChoices.choiceDatas.Count;i++)
        {
            CueChoice newCueChoice = new CueChoice();
            StorylineManager.cuesChoices.Add(newCueChoice);
            newCueChoice.cueType = so_InteractibleChoices.choiceDatas[i].cueType;
            for (int x=0;x<so_InteractibleChoices.choiceDatas[i].choiceDatas.Count;x++)
            {
                LocalCueChoice newLocalCueChoice = new LocalCueChoice();
                newCueChoice.cueChoiceDatas.Add(newLocalCueChoice);
                newLocalCueChoice.choiceData = so_InteractibleChoices.choiceDatas[i].choiceDatas[x];
                newLocalCueChoice.wasChosen = false;
            }
         
           
        }
      
        if (StorylineManager.currentSO_Dialogues != null)
        {
            Debug.Log("DIALOGUE LOADED");
            CharacterDialogueUI.onCharacterSpokenTo.Invoke();

        }
        else
        {
            Debug.Log("IT DOESNT WORK");
        }

    }

    public static List<LocalCueChoice> GetCueChoiceDatas(string p_cueTypeValue)
    {
        for (int i = 0; i < cuesChoices.Count; i++)
        {
            if (cuesChoices[i].cueType.ToString().ToLower() == p_cueTypeValue.ToLower())
            {
                Debug.Log("XXX: RETURN SOMETHING");
                return cuesChoices[i].cueChoiceDatas;

            }
        }
        Debug.Log("XXX: RETURN NOTHING " + p_cueTypeValue);

        return null;
    }

    public static LocalCueChoice GetLocalCueChoice(ChoiceData p_currentChoiceData)
    {
        for (int i=0; i<cuesChoices.Count; i++)
        {
            for (int x=0; x<cuesChoices[i].cueChoiceDatas.Count;x++)
            {
               if (cuesChoices[i].cueChoiceDatas[x].choiceData == p_currentChoiceData)
                {
                    return cuesChoices[i].cueChoiceDatas[x];
                }
            }
        }
        return null;

    }
    public static StorylineManager instance;
    public SO_Character mainCharacter;

    public static SO_Dialogues currentZeroSO_Dialogues;
    public static SO_Dialogues currentSO_Dialogues;
    public static int currentDialogueIndex;
    public static List<Dialogue> loggedWords = new List<Dialogue>();

    public static SO_InteractibleChoices so_InteractibleChoices;
    public static bool sideDialogue = false;
    public static int savedDialogueIndex;
    public static SO_Dialogues savedSO_Dialogues;
 
    public static List<CueChoice> cuesChoices = new List<CueChoice>();

    public static string currentBackgroundMusic ="";



    public static bool paused = false;

    private void Awake()
    {
        instance = this;
        //StartCoroutine(AsyncLoadScene("PauseMenu", Finished));
    }

    public void LoadData(GameData data)
    {
        Debug.Log("LOAD " + data.currentDialogueIndex);
    
        StorylineManager.so_InteractibleChoices = data.so_InteractibleChoices;
        StorylineManager.currentSO_Dialogues = data.currentSO_Dialogues;
        StorylineManager.currentDialogueIndex = data.currentDialogueIndex;
        StorylineManager.cuesChoices = data.cuesChoices;

        mainCharacter.stageName = data.mainCharacterName;
        StorylineManager.loggedWords = data.loggedWords;

        sideDialogue = data.sideDialogue;
        savedDialogueIndex = data.savedDialogueIndex;
        savedSO_Dialogues = data.savedSO_Dialogues;

        currentBackgroundMusic = data.currentBackgroundMusic;

       // CharacterDialogueUI.onCharacterSpokenTo.Invoke();
        //OnLoadedEvent.Invoke();
    }

    public void SaveData(ref GameData data)
    {
        Debug.Log("SAVED " + data.currentDialogueIndex);
        data.CurrentSceneName = "VisualNovel";
        data.currentSO_Dialogues = StorylineManager.currentSO_Dialogues;
        data.currentDialogueIndex = StorylineManager.currentDialogueIndex;
        data.so_InteractibleChoices = StorylineManager.so_InteractibleChoices;
        data.cuesChoices = StorylineManager.cuesChoices;

        data.mainCharacterName = mainCharacter.stageName;
        data.loggedWords = StorylineManager.loggedWords;

        data.sideDialogue = sideDialogue;
        data.savedDialogueIndex = savedDialogueIndex;
        data.savedSO_Dialogues = savedSO_Dialogues;

        data.currentBackgroundMusic = currentBackgroundMusic;
    

    }
    public static void LoadPhone()
    {
        paused = true;
        Debug.Log("LOADING");
        SceneManager.LoadSceneAsync("FindR", LoadSceneMode.Additive);
    }

    public static void UnloadPhone()
    {
        paused = false;
        SceneManager.UnloadSceneAsync("FindR");
    }

    public IEnumerator AsyncLoadScene(string name, Action onCallBack = null)
    {
        AsyncOperation asyncLoadScene = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);

        while (!asyncLoadScene.isDone)
        {
            // loading bar =  asyncLoadScene.progress

            yield return null;
        }


        if (onCallBack != null)
            onCallBack?.Invoke();
    }

   
}
