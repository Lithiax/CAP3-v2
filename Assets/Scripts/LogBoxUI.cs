using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LogBoxUI : MonoBehaviour
{
    [SerializeField] private LogTextUI prefab;
    [SerializeField]
    private GameObject logBox;

    [SerializeField] private Transform containerTransform;
    [SerializeField] private bool couldUpdate = false;
    private GameObject saved;

    private void Awake()
    {
        CharacterDialogueUI.onNewDialogueEvent.AddListener(UpdateDialogueLog);
        StorylineManager.OnLoadedEvent += OnLoaded;
        PauseMenu.isPausingEvent += ToggleLogBox;
    }

    private void OnDestroy()
    {
        StorylineManager.OnLoadedEvent -= OnLoaded;
        PauseMenu.isPausingEvent -= ToggleLogBox;
    }

    private void OnLoaded()
    {
        DestroyLogs();
        Create();
    }
    public void ToggleLogBox()
    {
        //couldUpdate = !couldUpdate;
        if (logBox.activeSelf == false)
        {
            if (StorylineManager.currentSO_Dialogues != null)
            {
                if (StorylineManager.currentDialogueIndex < StorylineManager.currentSO_Dialogues.dialogues.Count)
                {
                    UpdateDialogueLogLat(StorylineManager.currentSO_Dialogues.dialogues[StorylineManager.currentDialogueIndex]);
                }
                else if (StorylineManager.currentDialogueIndex >= StorylineManager.currentSO_Dialogues.dialogues.Count)
                {
                    UpdateDialogueLogLat(StorylineManager.currentSO_Dialogues.dialogues[StorylineManager.currentSO_Dialogues.dialogues.Count - 1]);
                }
            }
        }
        else
        {
            Destroy(saved);
            //Destroy();
            // Debug.Log("LOG BOX CLOSE");
        }

    }

    void Create()
    {
        //Create
        if (StorylineManager.currentDialogueIndex > 0)
        {
            for (int i = 0; i < StorylineManager.loggedWords.Count; i++)
            {
                UpdateDialogueLog(StorylineManager.loggedWords[i]);

            }
        }
        if (StorylineManager.currentSO_Dialogues != null)
        {
            if (StorylineManager.currentDialogueIndex  < StorylineManager.currentSO_Dialogues.dialogues.Count)
            {
                UpdateDialogueLog(StorylineManager.currentSO_Dialogues.dialogues[StorylineManager.currentDialogueIndex]);
            }
            else if (StorylineManager.currentDialogueIndex >= StorylineManager.currentSO_Dialogues.dialogues.Count)
            {
                UpdateDialogueLog(StorylineManager.currentSO_Dialogues.dialogues[StorylineManager.currentSO_Dialogues.dialogues.Count-1]);
            }
        }

    }
    private void DestroyLogs()
    {
        //Destroy
        for (int i = 0; i < containerTransform.childCount; i++)
        {
            Destroy(containerTransform.GetChild(i).gameObject);
        }
    }
    public void UpdateDialogueLog(Dialogue p_dialogue)
    {
        //if(couldUpdate)
        //{
        LogTextUI newPrefab = Instantiate(prefab, containerTransform);
        //Find Speaker
        string speakerFound = "";
        for (int x = 0; x < p_dialogue.characterDatas.Count; x++)
        {
            if (p_dialogue.characterDatas[x].isSpeaking)
            {
                if (string.IsNullOrEmpty(p_dialogue.characterDatas[x].character.stageName))
                {
                    speakerFound = "YOU";
                }
                else
                {
                    speakerFound = p_dialogue.characterDatas[x].character.stageName;
                }

            }
        }
        string words = p_dialogue.words.Replace("<MC>", StorylineManager.instance.mainCharacter.stageName);
        newPrefab.Initialize(speakerFound, words);
        //}
      
    }

    public void UpdateDialogueLogLat(Dialogue p_dialogue)
    {
        //if(couldUpdate)
        //{
        LogTextUI newPrefab = Instantiate(prefab, containerTransform);
        //Find Speaker
        string speakerFound = "";
        for (int x = 0; x < p_dialogue.characterDatas.Count; x++)
        {
            if (p_dialogue.characterDatas[x].isSpeaking)
            {
                if (string.IsNullOrEmpty(p_dialogue.characterDatas[x].character.stageName))
                {
                    speakerFound = "YOU";
                }
                else
                {
                    speakerFound = p_dialogue.characterDatas[x].character.stageName;
                }

            }
        }
        string words = p_dialogue.words.Replace("<MC>", StorylineManager.instance.mainCharacter.stageName);
        newPrefab.Initialize(speakerFound, words);
        saved = newPrefab.gameObject;
        //}

    }

}
