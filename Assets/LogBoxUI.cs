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
    bool couldUpdate = false;

    private void Awake()
    {
        CharacterDialogueUI.onNewDialogueEvent.AddListener(UpdateDialogueLog);
    }
    public void ToggleLogBox()
    {
        couldUpdate = !couldUpdate;
        if (logBox.activeSelf == false)
        {
          
            //Create
            if (StorylineManager.currentDialogueIndex > 0)
            {
                for (int i = 0; i <= StorylineManager.loggedWords.Count; i++)
                {
                    UpdateDialogueLog(StorylineManager.loggedWords[i]);

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
        if(couldUpdate)
        {
            LogTextUI newPrefab = Instantiate(prefab, containerTransform);
            //Find Speaker
            string speakerFound = "";
            for (int x = 0; x < p_dialogue.characterDatas.Count; x++)
            {
                if (p_dialogue.characterDatas[x].isSpeaking)
                {
                    speakerFound = p_dialogue.characterDatas[x].character.stageName;
                }
            }
            string words = p_dialogue.words.Replace("<MC>", StorylineManager.instance.mainCharacter.stageName);
            newPrefab.Initialize(speakerFound, words);
        }
      
    }

}
