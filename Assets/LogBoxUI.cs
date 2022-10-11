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
    [SerializeField]
    private CharacterDialogueUI ui;
    private void Awake()
    {
        CharacterDialogueUI.onNewDialogueEvent.AddListener(UpdateDialogueLog);
    }
    public void ToggleLogBox()
    {
        if (logBox.activeSelf == false)
        {
            //Create
            if (StorylineManager.instance.temp.currentDialogueIndex > 0)
            {
                for (int i = 0; i <= StorylineManager.instance.temp.currentDialogueIndex - 1; i++)
                {
                    UpdateDialogueLog(ui.currentSO_Dialogues.dialogues[i]);

                }
            }
           
        }
        else
        {
            //Destroy
            for (int i = 0; i < containerTransform.childCount; i++)
            {
                Destroy(containerTransform.GetChild(i).gameObject);
            }
        }
        logBox.SetActive(!logBox.activeSelf);

    }

    public void UpdateDialogueLog(Dialogue p_dialogue)
    {
        LogTextUI newPrefab = Instantiate(prefab, containerTransform);
        //Find Speaker
        string speakerFound = "";
        for (int x = 0; x < p_dialogue.characterDatas.Count; x++)
        {
            if (p_dialogue.characterDatas[x].isSpeaking)
            {
                speakerFound = p_dialogue.characterDatas[x].character.name;
            }
        }
        newPrefab.Initialize(speakerFound, p_dialogue.words);
    }

}
