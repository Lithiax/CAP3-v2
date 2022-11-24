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

    public bool firstTime;

    public bool sideDialogue;
    
    public int savedDialogueIndex;
    public SO_Dialogues savedSO_Dialogues;


    public string currentBackgroundMusic;

    public List<ChatUserData> ChatUserData;
    public ProgressionData ProgressionData;
    public float penelopeHealth = 50;
    public float bradHealth = 50;
    public float liamHealth = 50;
    public float maeveHealth = 50;

    public static List<string> effects = new List<string>();
    public List<string> EffectsUsed;
    public GameData()
    {
        CurrentSceneName = "";
        ChatUserIDs = new int[0];
        GameEffects = new string[0];
        penelopeHealth = 50;
        bradHealth = 50;
        liamHealth = 50;
        maeveHealth = 50;
        cuesChoices = new List<CueChoice>();

        currentSO_Dialogues = null;
        currentDialogueIndex = -1;
        so_InteractibleChoices = null;
        mainCharacterName = "";
        loggedWords = new List<Dialogue>();
        firstTime = false;
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
