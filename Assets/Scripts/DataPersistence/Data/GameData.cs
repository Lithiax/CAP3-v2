using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string CurrentSceneName;
    public int[] ChatUserIDs;
    public string[] GameEffects;

    public SO_Dialogues currentSO_Dialogues;
    public int currentDialogueIndex;
    public List<CueChoice> cuesChoices;

    public string mainCharacterName;

    public List<Dialogue> loggedWords;
    public SO_InteractibleChoices so_InteractibleChoices;
    public SO_Character cueCharacter;

    public bool sideDialogue;
    public int savedDialogueIndex;
    public SO_Dialogues savedSO_Dialogues;


    public string currentBackgroundMusic;

    public List<ChatUserData> ChatUserData;
    public ProgressionData ProgressionData;

    public List<string> EffectsUsed;
    public GameData()
    {
        CurrentSceneName = "";
        ChatUserIDs = new int[0];
        GameEffects = new string[0];
  
        cuesChoices = new List<CueChoice>();

        currentSO_Dialogues = null;
        currentDialogueIndex = -1;
        so_InteractibleChoices = null;
        mainCharacterName = "";
        loggedWords = new List<Dialogue>();

        sideDialogue = false;
        savedDialogueIndex = -1;
        savedSO_Dialogues = null;

        currentBackgroundMusic = "";
        ChatUserData = new List<ChatUserData>();

        ProgressionData = new ProgressionData(1, 1);
        EffectsUsed = new List<string>();
    }

    public void DebugLogData()
    {
        //foreach (ChatUserData userData in ChatUserData)
        //{
        //    Debug.Log(userData.UserSO.profileName);
        //    Debug.Log(userData.CurrentNode.BaseNodeData.Name);
        //    Debug.Log(userData.ChatObjects.Count);
        //}
    }
}
