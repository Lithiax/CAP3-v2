using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
[System.Serializable]
public class CueUIPresetData
{
    [SerializeField]
    public CueType cueType;
    [SerializeField]
    public Sprite cueIcon;
    [SerializeField]
    public List<CuePresetPositionData> presetPosition = new List<CuePresetPositionData>();
}
[System.Serializable]
public class CuePresetPositionData
{
    [SerializeField]
    public CharacterPositionType characterPositionType;
    [SerializeField]
    public Vector2 position;
}
public class ActionUIs : MonoBehaviour
{
    [SerializeField]
    private bool canExit = true;

    [SerializeField]
    public static bool isShowing = false;

    [SerializeField] private CueUI currentCueUI;
    [SerializeField] private List<CueUIPresetData> cueUIPresetDatas;

    public static Action<ActionUI> onEnterEvent;
    public static Action onExitEvent;
    public static Action<ActionUI> onPointClickEvent;

    public bool canDoWork = true;

    private void Awake()
    {
        CueUI.onChoiceChosenEvent += ClosedUIButton;
        onEnterEvent += Entered;
        onExitEvent += ClosedUI;
        onPointClickEvent += PointClick;
        CharacterDialogueUI.OnStartChooseChoiceEvent += startchoose;
        CharacterDialogueUI.OnEndChooseChoiceEvent += canDoReset;
    }

    void canDoReset()
    {
        canDoWork = true;
    }
    private void OnDestroy()
    {
        CueUI.onChoiceChosenEvent -= ClosedUIButton;
        onEnterEvent -= Entered;
        onExitEvent -= ClosedUI;
        onPointClickEvent -= PointClick;
        CharacterDialogueUI.OnStartChooseChoiceEvent -= startchoose;
        CharacterDialogueUI.OnEndChooseChoiceEvent -= canDoReset;
    }

    void startchoose()
    {
        canDoWork = false;
        ClosedUIButton();
    }
    CueUIPresetData GetCueUIPresetData(CueType p_cueType)
    {

        for (int i = 0; i < cueUIPresetDatas.Count; i++)
        {
            if (cueUIPresetDatas[i].cueType == p_cueType)
            {
                return cueUIPresetDatas[i];
            }
        }
        return null;
    }
 
    public void Entered(ActionUI test)
    {
        //ToggleContainer();
        Debug.Log(canDoWork + " - " + canExit);
        if (canDoWork)
        {
            if (canExit)
            {


                CueUIPresetData chosenCueUIPresetData = GetCueUIPresetData(test.cueType);
                if (StorylineManager.currentSO_Dialogues != null)
                {
                    Debug.Log("INSIDE 1");
                    if (!StorylineManager.sideDialogue)
                    {
                        Debug.Log("INSIDE 2");
                        //Find Speaker
                        CharacterData speaker = null;
                        if (DialogueSpreadSheetPatternConstants.cueCharacter != null)
                        {
                            Debug.Log("INSIDE 2a");
                            if (StorylineManager.currentDialogueIndex < StorylineManager.currentSO_Dialogues.dialogues.Count)
                            {
                                Debug.Log("INSIDE 2b");
                                if (StorylineManager.currentSO_Dialogues.dialogues[StorylineManager.currentDialogueIndex].characterDatas != null)
                                {
                                    Debug.Log("INSIDE 2c");
                                    for (int i = 0; i < StorylineManager.currentSO_Dialogues.dialogues[StorylineManager.currentDialogueIndex].characterDatas.Count; i++)
                                    {
                                        //if (StorylineManager.currentSO_Dialogues.dialogues[StorylineManager.currentDialogueIndex].characterDatas[i].isSpeaking)
                                        //{
                                          
                                            speaker = StorylineManager.currentSO_Dialogues.dialogues[StorylineManager.currentDialogueIndex].characterDatas[i];
                                            Debug.Log("INSIDE 2d " + speaker);
                                            if (speaker.character != DialogueSpreadSheetPatternConstants.cueCharacter)
                                            {
                                                Debug.Log("INSIDE 2g");
                                                speaker = null;
                                            }
                                            else
                                            {
                                                break;
                                            }


                                          
                                        //}
                                    }
                                }

                            }
                        }


                        Debug.Log("INSIDE 3 " + speaker);
                        if (speaker != null)
                        {
                            CuePresetPositionData cuePresetPositionData = null;
                            for (int i = 0; i < chosenCueUIPresetData.presetPosition.Count; i++)
                            {
                                if (chosenCueUIPresetData.presetPosition[i].characterPositionType == speaker.characterPosition)
                                {
                                    cuePresetPositionData = chosenCueUIPresetData.presetPosition[i];
                                    break;
                                }
                            }

                            currentCueUI.Initialize(test.cueType.ToString(), chosenCueUIPresetData.cueIcon, StorylineManager.currentSO_Dialogues.cueBankData.GetCueValue(test.cueType), cuePresetPositionData.position);
                            StartCoroutine(Ent());
                        }
              
                    }

                }
            }
        }
      
       

       
    }

    public void PointClick(ActionUI test)
    {
        Debug.Log("POINT CLICK " + canExit + " - " + ActionUIs.isShowing);
        if (ActionUIs.isShowing)
        {
            //ToggleContainer();
            canExit = false;
        }
    }

    IEnumerator Ent()
    {
        Debug.Log("START");
        yield return new WaitForSeconds(1f);
        ActionUIs.isShowing = true;
        currentCueUI.gameObject.SetActive(true);
        CharacterDialogueUI.OnInspectingEvent.Invoke();
       //actionsContainer.SetActive(true);
    }

    public void ClosedUI()
    {
        StopAllCoroutines();
        if (canExit)
        {
            ActionUIs.isShowing = false;
           
            CharacterDialogueUI.OnDeinspectingEvent.Invoke();
            currentCueUI.gameObject.SetActive(false);
            currentCueUI.ResetChoiceManager();
        }
 
    }

    public void ClosedUIButton()
    {
        canExit = true;
        ActionUIs.isShowing = false;
        StopAllCoroutines();
        CharacterDialogueUI.OnDeinspectingEvent.Invoke();
        currentCueUI.gameObject.SetActive(false);
        currentCueUI.ResetChoiceManager();
        

    }
}
