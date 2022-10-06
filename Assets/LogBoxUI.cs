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
    public void ToggleLogBox()
    {
        if (logBox.activeSelf == false)
        {
            //Create
            for (int i = 0; i <= StorylineManager.instance.temp.currentDialogueIndex; i++)
            {
                LogTextUI newPrefab = Instantiate(prefab,containerTransform);
                //Find Speaker
                string speakerFound = "";
                for (int x = 0; x < StorylineManager.instance.temp.currentSO_Dialogues.dialogues[i].characterDatas.Count; x++)
                {
                    if (StorylineManager.instance.temp.currentSO_Dialogues.dialogues[i].characterDatas[x].isSpeaking)
                    {
                        speakerFound = StorylineManager.instance.temp.currentSO_Dialogues.dialogues[i].characterDatas[x].character.name;
                    }
                }
                newPrefab.Initialize(speakerFound,StorylineManager.instance.temp.currentSO_Dialogues.dialogues[i].words);

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

}
