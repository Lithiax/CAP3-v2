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

    public static bool justLoadedVN = false;
    public static bool renamed = false;
    public static Action OnLoadedEvent;



    public static void GoBackMenu()
    {
  
        SceneManager.LoadScene("MainMenu");
    }

public static void LoadVisualNovel(GameData p_gameData)
    {


        StorylineManager.CurrentSceneName = "VisualNovel";
        SO_Character mainCharacter = Resources.Load<SO_Character>("Scriptable Objects/Characters/You");
        mainCharacter.stageName = p_gameData.mainCharacterName;
        StorylineManager.cuesChoices.Clear();
        StorylineManager.currentSO_Dialogues = p_gameData.currentSO_Dialogues;
        StorylineManager.so_InteractibleChoices = p_gameData.so_InteractibleChoices;
        renamed = p_gameData.renamed;
        justLoadedVN = true;
     
        if (so_InteractibleChoices != null)
        {
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
        }
        else
        {
            StorylineManager.currentZeroSO_Dialogues = null;
        }
         DialogueSpreadSheetPatternConstants.penelopeHealth = p_gameData.penelopeHealth;
        DialogueSpreadSheetPatternConstants.bradHealth = p_gameData.bradHealth;
        DialogueSpreadSheetPatternConstants.liamHealth = p_gameData.liamHealth;
        DialogueSpreadSheetPatternConstants.maeveHealth = p_gameData.maeveHealth;
        popUpSO_Dialogues = p_gameData.popUpSO_Dialogues;
        StorylineManager.loggedWords = p_gameData.loggedWords;

        StorylineManager.currentDialogueIndex = p_gameData.currentDialogueIndex;
        sideDialogue = p_gameData.sideDialogue;
        savedDialogueIndex = p_gameData.savedDialogueIndex;
        savedSO_Dialogues = p_gameData.savedSO_Dialogues;

        StorylineManager.firstTime = p_gameData.firstTime;

        //if (AudioManager.instance != null)
        //{
        //    if (!string.IsNullOrEmpty(currentBackgroundMusic))
        //    {
        //        //Debug.Log("HMMPF");
        //        AudioManager.instance.ForceStopAudio(currentBackgroundMusic, false);
        //    }

        //}
        //currentBackgroundMusic = "";


        paused = false;

        if (StorylineManager.currentSO_Dialogues != null)
        {
            CharacterDialogueUI.onCharacterSpokenTo?.Invoke();

        }

    }
    public static void LoadVisualNovel(string folderField, string sheetField)
    {
        StorylineManager.CurrentSceneName = "VisualNovel";
    
        StorylineManager.cuesChoices.Clear();
 
        StorylineManager.currentSO_Dialogues = Resources.Load<SO_Dialogues>("Scriptable Objects/Dialogues/Visual Novel/" + folderField + "/" + sheetField);
        StorylineManager.so_InteractibleChoices = Resources.Load<SO_InteractibleChoices>("Scriptable Objects/Dialogues/Visual Novel/" + folderField + "/" + "Interactible Choices");
        if (so_InteractibleChoices != null)
        {
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
            popUpSO_Dialogues = null;
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
        }
        else
        {
            StorylineManager.currentZeroSO_Dialogues = null;
        }


    
        StorylineManager.loggedWords.Clear();
        StorylineManager.currentDialogueIndex = 0;
        sideDialogue = false;
        savedDialogueIndex = -1;
        savedSO_Dialogues = null;

        //if (AudioManager.instance != null)
        //{
        //    if (!string.IsNullOrEmpty(currentBackgroundMusic))
        //    {
        //        //Debug.Log("HMMPF");
        //        AudioManager.instance.ForceStopAudio(currentBackgroundMusic,false);
        //    }
         
        //}
       // currentBackgroundMusic = "";
   
        paused = false;
      

        if (StorylineManager.currentSO_Dialogues != null)
        {
            CharacterDialogueUI.onCharacterSpokenTo?.Invoke();

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
    public static SO_Dialogues popUpSO_Dialogues;
    public static List<CueChoice> cuesChoices = new List<CueChoice>();

    public static string currentBackgroundMusic = "";



    public static bool paused = false;

    private void Awake()
    {
        instance = this;
    }
    public void LoadData(GameData data)
    {
        StaticUserData.ChatUserData.Clear();
        StaticUserData.ChatUserData = data.ChatUserData;
        SO_Character mainCharacter = Resources.Load<SO_Character>("Scriptable Objects/Characters/You");
        if (mainCharacter)
        {
            if (mainCharacter.stageName != "YOU" || !string.IsNullOrEmpty(mainCharacter.stageName))
            {
                mainCharacter.stageName = data.mainCharacterName;
            }
            else
            {
                mainCharacter.stageName = "YOU";
            }

        }
    }
 

    public void SaveData(ref GameData data)
    {
        float penhealth = DialogueSpreadSheetPatternConstants.penelopeHealth;
        float bradhealth = DialogueSpreadSheetPatternConstants.bradHealth;
        float liamhealth = DialogueSpreadSheetPatternConstants.liamHealth;
        float mavhealth = DialogueSpreadSheetPatternConstants.maeveHealth;
        StaticUserData.Save(ref data);
        data.renamed = renamed;
        data.penelopeHealth = penhealth;
        data.bradHealth = bradhealth;
        data.liamHealth = liamhealth;
        data.maeveHealth = mavhealth;
        data.popUpSO_Dialogues = popUpSO_Dialogues;
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
