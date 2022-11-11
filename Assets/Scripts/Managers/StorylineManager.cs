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
    public static Action OnLoadedEvent;
    [SerializeField] Button saveButtonUI;
    [SerializeField] Button loadButtonUI;
    public static void LoadVisualNovel(string folderField, string sheetField)
    {
        isPaused = false;
        SO_Character mainCharacter = Resources.Load<SO_Character>("Scriptable Objects/Characters/You");
        mainCharacter.stageName = "You";
        StorylineManager.currentSO_Dialogues = Resources.Load<SO_Dialogues>("Scriptable Objects/Dialogues/Visual Novel/" + folderField + "/" + sheetField);
        SO_InteractibleChoices so_InteractibleChoices = Resources.Load<SO_InteractibleChoices>("Scriptable Objects/Dialogues/Visual Novel/" + folderField + "/" + "Interactible Choices");
        StorylineManager.cueCharacter= so_InteractibleChoices.characterData.character;
        StorylineManager.loggedWords.Clear();
        StorylineManager.currentDialogueIndex = 0;
        sideDialogue = false;
        savedDialogueIndex = -1;
        savedSO_Dialogues = null;

        currentBackgroundMusic = "";

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
            CharacterDialogueUI.onCharacterSpokenTo.Invoke("");

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

    public static void LoadAsyncFindr()
    {
        Debug.Log("LOADING");
        isPaused = true;
        SceneManager.LoadSceneAsync("FindR", LoadSceneMode.Additive);
    }

    public static void UnloadAsyncFindr()
    {
        isPaused = false;
        SceneManager.UnloadSceneAsync("FindR", UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
    }
    public static StorylineManager instance;
    public SO_Character mainCharacter;

    public static bool isPaused;
    public static SO_Dialogues currentSO_Dialogues;
    public static int currentDialogueIndex;
    public static List<Dialogue> loggedWords = new List<Dialogue>();

    public static SO_Character cueCharacter;

    public static bool sideDialogue = false;
    public static int savedDialogueIndex;
    public static SO_Dialogues savedSO_Dialogues;
 
    public static List<CueChoice> cuesChoices = new List<CueChoice>();

    public static string currentBackgroundMusic ="";
    public IEnumerator AsyncLoadScene(string name, Action onCallBack = null)
    {
        AsyncOperation asyncLoadScene = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);

        while (!asyncLoadScene.isDone)
        {
            // loading bar =  asyncLoadScene.progress

            yield return null;
        }
        yield return new WaitForSeconds(3f);
        if (onCallBack != null)
            onCallBack?.Invoke();
    }

    void ConnectEvent()
    {
        saveButtonUI.onClick.AddListener(delegate { DataPersistenceManager.instance.SaveGame(); });
        loadButtonUI.onClick.AddListener(delegate { DataPersistenceManager.instance.LoadGame(); });
    }
    private void Awake()
    {
        instance = this;
    
        StartCoroutine(AsyncLoadScene("PauseMenu", ConnectEvent));
    }

    public void LoadData(GameData data)
    {
        Debug.Log("LOAD " + data.currentDialogueIndex);
        StorylineManager.currentSO_Dialogues = data.currentSO_Dialogues;
        StorylineManager.currentDialogueIndex = data.currentDialogueIndex;
        StorylineManager.cuesChoices = data.cuesChoices;

        mainCharacter.stageName = data.mainCharacterName;
        StorylineManager.loggedWords = data.loggedWords;

        sideDialogue = data.sideDialogue;
        savedDialogueIndex = data.savedDialogueIndex;
        savedSO_Dialogues = data.savedSO_Dialogues;

        currentBackgroundMusic = data.currentBackgroundMusic;

        CharacterDialogueUI.onCharacterSpokenTo.Invoke("");
        OnLoadedEvent.Invoke();
    }

    public void SaveData(ref GameData data)
    {
        Debug.Log("SAVED " + data.currentDialogueIndex);
        data.currentSO_Dialogues = StorylineManager.currentSO_Dialogues;
        data.currentDialogueIndex = StorylineManager.currentDialogueIndex;
        data.cuesChoices = StorylineManager.cuesChoices;

        data.mainCharacterName = mainCharacter.stageName;
        data.loggedWords = StorylineManager.loggedWords;

        data.sideDialogue = sideDialogue;
        data.savedDialogueIndex = savedDialogueIndex;
        data.savedSO_Dialogues = savedSO_Dialogues;

        data.currentBackgroundMusic = currentBackgroundMusic;


    }
}
