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
    public Vector2 position;
}
public class ActionUIs : MonoBehaviour
{
    [SerializeField]
    private bool canExit = true;

    [SerializeField]
    private bool isShowing = false;

    [SerializeField] private CueUI currentCueUI;
    [SerializeField] private List<CueUIPresetData> cueUIPresetDatas;

    public static Action<ActionUI> onEnterEvent;
    public static Action onExitEvent;
    public static Action<ActionUI> onPointClickEvent;
    private void Awake()
    {
        CueUI.onChoiceChosenEvent += ClosedUIButton;
        onEnterEvent += Entered;
        onExitEvent += ClosedUI;
        onPointClickEvent += PointClick;
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
        if (canExit)
        {
            

            CueUIPresetData chosenCueUIPresetData = GetCueUIPresetData(test.cueType);
            if (StorylineManager.currentSO_Dialogues != null)
            {
                if (!StorylineManager.sideDialogue)
                {
                    currentCueUI.Initialize(test.cueType.ToString(), chosenCueUIPresetData.cueIcon, StorylineManager.currentSO_Dialogues.cueBankData.GetCueValue(test.cueType), chosenCueUIPresetData.position);
                    StartCoroutine(Ent());
                }
                 
            }
        }
       

       
    }

    public void PointClick(ActionUI test)
    {
        if (isShowing)
        {
            //ToggleContainer();
            canExit = false;
        }
    }

    IEnumerator Ent()
    {
        yield return new WaitForSeconds(1f);
        isShowing = true;
        currentCueUI.gameObject.SetActive(true);
        CharacterDialogueUI.OnInspectingEvent.Invoke();
       //actionsContainer.SetActive(true);
    }

    public void ClosedUI()
    {
        if (canExit)
        {
            isShowing = false;
            StopAllCoroutines();
            CharacterDialogueUI.OnDeinspectingEvent.Invoke();
            currentCueUI.gameObject.SetActive(false);
            currentCueUI.ResetChoiceManager();
        }
 
    }

    public void ClosedUIButton()
    {
        canExit = true;
        isShowing = false;
        StopAllCoroutines();
        CharacterDialogueUI.OnDeinspectingEvent.Invoke();
        currentCueUI.gameObject.SetActive(false);
        currentCueUI.ResetChoiceManager();
        

    }
}
