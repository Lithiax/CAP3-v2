using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class ChoicesUI : MonoBehaviour
{

    [SerializeField] private GameObject choiceUIsContainer;
    private Transform choiceUIsContainerTransform;
    private RectTransform choiceUIsContainerRectTransform;

    [SerializeField] private HealthUI healthUI;

    [SerializeField] private ChoiceUI choiceUIPrefab;


    public static Action<List<ChoiceData>> OnChoosingChoiceEvent;
    public GameObject cue;

    List<ChoiceData> savedchoiceDatas = new List<ChoiceData> ();
   
    private void Awake()
    {
        //ActionUIs.onEnterEvent += ves;
        CharacterDialogueUI.OnInspectingEvent += open;
        CharacterDialogueUI.OnDeinspectingEvent += close;
    
        choiceUIsContainerTransform = choiceUIsContainer.transform;
        choiceUIsContainerRectTransform = choiceUIsContainer.GetComponent<RectTransform>();

        OnChoosingChoiceEvent += Initialize;
        StorylineManager.OnLoadedEvent += ResetChoiceManager;

    }
     
    void open()
    {
        Destroy();
        choiceUIsContainer.SetActive(false);
    }

    void close()
    {
        for (int i = 0; i < savedchoiceDatas.Count; i++)
        {
            Debug.Log("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX: " + savedchoiceDatas[i].words);
        }
        if (savedchoiceDatas.Count > 0)
        {
            CreateChoiceUIs(savedchoiceDatas);
        }

        //choiceUIsContainer.SetActive(true);
    }
    private void OnDestroy()
    {
     
        CharacterDialogueUI.OnInspectingEvent -= open;
        CharacterDialogueUI.OnDeinspectingEvent -= close;
        OnChoosingChoiceEvent -= Initialize;
        StorylineManager.OnLoadedEvent -= ResetChoiceManager;
    }
  
    void Destroy()
    {
        if (choiceUIsContainer.activeSelf)
        {
            for (int i = 0; i < choiceUIsContainerTransform.childCount; i++)
            {
                Destroy(choiceUIsContainerTransform.GetChild(i).gameObject);

            }
        }
    }

    void CreateChoiceUIs(List<ChoiceData> p_choiceDatas)
    {
        Destroy();
        choiceUIsContainer.SetActive(true);
        for (int i = 0; i < p_choiceDatas.Count; i++)
        {
            ChoiceUI newChoiceUI = Instantiate(choiceUIPrefab, choiceUIsContainerTransform);
            newChoiceUI.InitializeValues(p_choiceDatas[i].words);
            ChoiceData currentChoiceData = p_choiceDatas[i];
            if (StorylineManager.currentSO_Dialogues.choiceDatas[i].isHealthConditionInUseColumnPattern)
            {
                if (healthUI.OnIsWithinHealthConditionEvent.Invoke(StorylineManager.currentSO_Dialogues.choiceDatas[i].healthCeilingCondition, StorylineManager.currentSO_Dialogues.choiceDatas[i].healthFloorCondition))
                {
                    //Can be selected
                    newChoiceUI.GetComponent<Button>().onClick.AddListener(delegate { ChooseChoiceUI(currentChoiceData); });
                    LayoutRebuilder.ForceRebuildLayoutImmediate(choiceUIsContainerRectTransform);
                }
                else
                {
                    //Cant be selected
                    newChoiceUI.GetComponent<Button>().interactable = false;
                    newChoiceUI.GetComponent<Image>().color = new Color32(255, 255, 255, 150);
                }

            }
            else if(StorylineManager.currentSO_Dialogues.choiceDatas[i].isEffectIDConditionInUseColumnPattern)
            {
                if (DialogueSpreadSheetPatternConstants.effects.Contains(StorylineManager.currentSO_Dialogues.choiceDatas[i].effectIDCondition.ToLower()))
                {
                    //Can be selected
                    newChoiceUI.GetComponent<Button>().onClick.AddListener(delegate { ChooseChoiceUI(currentChoiceData); });
                    LayoutRebuilder.ForceRebuildLayoutImmediate(choiceUIsContainerRectTransform);
                }
                else
                {
                    //Cant be selected
                    newChoiceUI.GetComponent<Button>().interactable = false;
                    newChoiceUI.GetComponent<Image>().color = new Color32(255, 255, 255, 150);
                }
            }
            else if (!StorylineManager.currentSO_Dialogues.choiceDatas[i].isHealthConditionInUseColumnPattern &&
                !StorylineManager.currentSO_Dialogues.choiceDatas[i].isEffectIDConditionInUseColumnPattern)
            {
                //Can be selected
                newChoiceUI.GetComponent<Button>().onClick.AddListener(delegate { ChooseChoiceUI(currentChoiceData); });
                LayoutRebuilder.ForceRebuildLayoutImmediate(choiceUIsContainerRectTransform);
            }
           

        }
    
    }

    public void ChooseChoiceUI(ChoiceData p_currentChoiceData)
    {
        savedchoiceDatas.Clear();
        StorylineManager.currentDialogueIndex = 0;
        if (p_currentChoiceData.effectID != "")
        {
            Debug.Log(p_currentChoiceData.effectID);
            for (int i=0; i < DialogueSpreadSheetPatternConstants.effects.Count; i++)
            {
                Debug.Log(DialogueSpreadSheetPatternConstants.effects[i]);
            }
           // DialogueSpreadSheetPatternConstants.effects.Add(p_currentChoiceData.effectID.ToLower());
        }

        if (p_currentChoiceData.isImmediateGoPhone)
        {
            if (!string.IsNullOrEmpty(p_currentChoiceData.effectID))
            {
                if (p_currentChoiceData.effectID != "<VN>")
                {
                    Debug.Log("EFFECT ADDED ");
                    string[] sheetDivided = p_currentChoiceData.effectID.Split('&');
                    for (int i=0; i< sheetDivided.Length; i++)
                    {
                        DialogueSpreadSheetPatternConstants.effects.Add(sheetDivided[i].ToLower());
                    }
                    
                    

                }
                StorylineManager.LoadPhone();
            }
        }
        CharacterDialogueUI.OnStartChooseChoiceEvent.Invoke();
        //Reset Choice Manager
        ResetChoiceManager();

        //Set Choice Damage
        healthUI.OnModifyHealthEvent.Invoke(p_currentChoiceData.healthModifier);
      
        //Set Pop Up
        Debug.Log("1 POP UP TEXT " + p_currentChoiceData.popUpContent);
        if (!string.IsNullOrEmpty(p_currentChoiceData.popUpContent))
        {
            Debug.Log("2 POP UP TEXT " + p_currentChoiceData.popUpContent);
            PopUpUI.OnPopUpEvent.Invoke(p_currentChoiceData.popUpTitle, p_currentChoiceData.popUpContent);
            CharacterDialogueUI.OnPopUpEvent.Invoke(p_currentChoiceData.branchDialogue);

        }
        else
        {
            StorylineManager.currentSO_Dialogues = p_currentChoiceData.branchDialogue;
            CharacterDialogueUI.OnEndChooseChoiceEvent.Invoke();
        }

    }

    public void Initialize(List<ChoiceData> p_choiceDatas)
    {
        savedchoiceDatas.Clear();
        for (int i = 0; i < p_choiceDatas.Count; i++)
        {
            savedchoiceDatas.Add(p_choiceDatas[i]);
        }

        StartCoroutine(Delay(savedchoiceDatas));
      
    }
    IEnumerator Delay(List<ChoiceData> p_choiceDatas)
    {
        yield return new WaitForSeconds(2f);
        if (!cue.activeSelf)
        {
            CreateChoiceUIs(p_choiceDatas);
        }
       
    
    }
    void ResetChoiceManager()
    {
        if (choiceUIsContainer.activeSelf)
        {
            for (int i = 0; i < choiceUIsContainerTransform.childCount; i++)
            {
                Destroy(choiceUIsContainerTransform.GetChild(i).gameObject);

            }
        }
        choiceUIsContainer.SetActive(false);
    }
}
