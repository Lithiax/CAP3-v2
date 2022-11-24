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
    public static void GoBackMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public static void LoadVisualNovel(GameData p_gameData)
    {
        if (StorylineManager.currentBackgroundMusic != null)
        {
            AudioManager.instance.SmoothStopAudio(StorylineManager.currentBackgroundMusic, false);
        }

        StorylineManager.CurrentSceneName = "VisualNovel";
        SO_Character mainCharacter = Resources.Load<SO_Character>("Scriptable Objects/Characters/You");
        mainCharacter.stageName = p_gameData.mainCharacterName;
        StorylineManager.cuesChoices.Clear();
        StorylineManager.currentSO_Dialogues = p_gameData.currentSO_Dialogues;
        StorylineManager.so_InteractibleChoices = p_gameData.so_InteractibleChoices;
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
        DialogueSpreadSheetPatternConstants.penelopeHealth = p_gameData.penelopeHealth;
        DialogueSpreadSheetPatternConstants.bradHealth = p_gameData.bradHealth;
        DialogueSpreadSheetPatternConstants.liamHealth = p_gameData.liamHealth;
        DialogueSpreadSheetPatternConstants.maeveHealth = p_gameData.maeveHealth;
        StorylineManager.loggedWords.Clear();
        StorylineManager.currentDialogueIndex = p_gameData.currentDialogueIndex;
        sideDialogue = p_gameData.sideDialogue;
        savedDialogueIndex = p_gameData.savedDialogueIndex;
        savedSO_Dialogues = p_gameData.savedSO_Dialogues;
        StorylineManager.firstTime = p_gameData.firstTime;
        currentBackgroundMusic = p_gameData.currentBackgroundMusic;
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
            // Debug.Log("DIALOGUE LOADED");
            CharacterDialogueUI.onCharacterSpokenTo.Invoke();

        }
        else
        {
            // Debug.Log("IT DOESNT WORK");
        }

    }
    public static void LoadVisualNovel(string folderField, string sheetField)
    {
        StorylineManager.CurrentSceneName = "VisualNovel";
        SO_Character mainCharacter = Resources.Load<SO_Character>("Scriptable Objects/Characters/You");
        mainCharacter.stageName = "You";
        StorylineManager.cuesChoices.Clear();
        StorylineManager.currentSO_Dialogues = Resources.Load<SO_Dialogues>("Scriptable Objects/Dialogues/Visual Novel/" + folderField + "/" + sheetField);
        StorylineManager.so_InteractibleChoices = Resources.Load<SO_InteractibleChoices>("Scriptable Objects/Dialogues/Visual Novel/" + folderField + "/" + "Interactible Choices");
        // Debug.Log("INTELLIGIENCE: " + StorylineManager.so_InteractibleChoices);
        if (so_InteractibleChoices.deathSheet != null)
        {
            Debug.Log("DEATH SHEET LOADED");
            StorylineManager.currentZeroSO_Dialogues = so_InteractibleChoices.deathSheet;
        }
        else
        {
            StorylineManager.currentZeroSO_Dialogues = null;
        }

        if (so_InteractibleChoices.characterData != null)
        {
            DialogueSpreadSheetPatternConstants.cueCharacter = so_InteractibleChoices.characterData.character;
            Debug.Log("TARGET CHARACTER FOUND: " + DialogueSpreadSheetPatternConstants.cueCharacter);
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
            //  Debug.Log("DIALOGUE LOADED");
            CharacterDialogueUI.onCharacterSpokenTo.Invoke();

        }
        else
        {
            //  Debug.Log("IT DOESNT WORK");
        }

    }

    public static List<LocalCueChoice> GetCueChoiceDatas(string p_cueTypeValue)
    {
        for (int i = 0; i < cuesChoices.Count; i++)
        {
            if (cuesChoices[i].cueType.ToString().ToLower() == p_cueTypeValue.ToLower())
            {
                // Debug.Log("XXX: RETURN SOMETHING");
                return cuesChoices[i].cueChoiceDatas;

            }
        }
        //     Debug.Log("XXX: RETURN NOTHING " + p_cueTypeValue);

        return null;
    }

    public static LocalCueChoice GetLocalCueChoice(ChoiceData p_currentChoiceData)
    {
        for (int i = 0; i < cuesChoices.Count; i++)
        {
            for (int x = 0; x < cuesChoices[i].cueChoiceDatas.Count; x++)
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
    public static string CurrentSceneName;
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

    public static string currentBackgroundMusic = "";



    public static bool paused = false;

    private void Awake()
    {
        instance = this;
        //StartCoroutine(AsyncLoadScene("PauseMenu", Finished));
    }
    public void LoadData(GameData data)
    {

    }
    //public void LoadData(GameData data)
    //{
    //   // Debug.Log("LOAD " + data.currentDialogueIndex);
    //    StorylineManager.CurrentSceneName = data.CurrentSceneName;
    //    StorylineManager.so_InteractibleChoices = data.so_InteractibleChoices;
    //    StorylineManager.currentSO_Dialogues = data.currentSO_Dialogues;
    //    StorylineManager.currentDialogueIndex = data.currentDialogueIndex;
    //    StorylineManager.cuesChoices = data.cuesChoices;

    //    mainCharacter.stageName = data.mainCharacterName;
    //    StorylineManager.loggedWords = data.loggedWords;

    //    sideDialogue = data.sideDialogue;
    //    savedDialogueIndex = data.savedDialogueIndex;
    //    savedSO_Dialogues = data.savedSO_Dialogues;

    //    currentBackgroundMusic = data.currentBackgroundMusic;

    //    // CharacterDialogueUI.onCharacterSpokenTo.Invoke();
    //    //OnLoadedEvent.Invoke();
    //}

    public void SaveData(ref GameData data)
    {
        //  Debug.Log("SAVED " + data.currentDialogueIndex);
        data.penelopeHealth = DialogueSpreadSheetPatternConstants.penelopeHealth;
        data.bradHealth = DialogueSpreadSheetPatternConstants.bradHealth;
        data.liamHealth = DialogueSpreadSheetPatternConstants.liamHealth;
        data.maeveHealth = DialogueSpreadSheetPatternConstants.maeveHealth;
        data.CurrentSceneName = StorylineManager.CurrentSceneName;
        data.currentSO_Dialogues = StorylineManager.currentSO_Dialogues;
        data.currentDialogueIndex = StorylineManager.currentDialogueIndex;
        data.so_InteractibleChoices = StorylineManager.so_InteractibleChoices;
        data.cuesChoices = StorylineManager.cuesChoices;
        data.firstTime = StorylineManager.firstTime;
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
        //  Debug.Log("LOADING");
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
