using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ChoiceManager : MonoBehaviour
{

    [SerializeField] private GameObject choiceUIsContainer;
    private Transform choiceUIsContainerTransform;
    private RectTransform choiceUIsContainerRectTransform;

    [SerializeField] private ChoiceUI choiceUIPrefab;


    public static Action<HealthUI, List<ChoiceData>> OnChoosingChoiceEvent;
    
    private void Awake()
    {
        choiceUIsContainerTransform = choiceUIsContainer.transform;
        choiceUIsContainerRectTransform = choiceUIsContainer.GetComponent<RectTransform>();

        OnChoosingChoiceEvent += CreateChoiceUIs;

    }

    void CreateChoiceUIs(HealthUI healthUI, List<ChoiceData> p_choiceDatas)
    {
        choiceUIsContainer.SetActive(true);
        for (int i = 0; i < p_choiceDatas.Count; i++)
        {
            ChoiceUI newChoiceUI = Instantiate(choiceUIPrefab, choiceUIsContainerTransform);
            newChoiceUI.InitializeValues(p_choiceDatas[i].words);
            ChoiceData currentChoiceData = p_choiceDatas[i];
            //if (healthUI.currentHealth <= p_choiceDatas[i].healthCeilingCondition &&
            //    healthUI.currentHealth > p_choiceDatas[i].healthFloorCondition)
            //{
                //Can be selected
                newChoiceUI.GetComponent<Button>().onClick.AddListener(delegate { ChooseChoiceUI(currentChoiceData); });
                LayoutRebuilder.ForceRebuildLayoutImmediate(choiceUIsContainerRectTransform);
            //}
            //else
            //{
            //    //Cant be selected
            //    newChoiceUI.GetComponent<Button>().interactable = false;
            //    newChoiceUI.GetComponent<Image>().color = new Color32(255,255,255,150);
            //}
       
        }
    }

    public void ChooseChoiceUI(ChoiceData p_currentChoiceData)
    {

        CharacterDialogueUI.OnStartChooseChoiceEvent.Invoke();
        //Reset Choice Manager
        ResetChoiceManager();

        //Set Choice Damage
        //if (p_currentChoiceData.damage)
        //{

        //}
        HealthUI.ModifyHealthEvent.Invoke(p_currentChoiceData.healthModifier);
        if (p_currentChoiceData.effectID != "")
        {
            DialogueSpreadSheetPatternConstants.effects.Add(p_currentChoiceData.effectID);
        }
        //Set Pop Up
        Debug.Log("1 POP UP TEXT " + p_currentChoiceData.popUpContent);
        if (!string.IsNullOrEmpty(p_currentChoiceData.popUpContent))
        {
            Debug.Log("2 POP UP TEXT " + p_currentChoiceData.popUpContent);
            PopUpUI.OnPopUpEvent.Invoke(p_currentChoiceData.popUpTitle,p_currentChoiceData.popUpContent);
            CharacterDialogueUI.OnPopUpEvent.Invoke(p_currentChoiceData.branchDialogue);

        }
        else
        {
            StorylineManager.currentSO_Dialogues = p_currentChoiceData.branchDialogue;
            CharacterDialogueUI.OnEndChooseChoiceEvent.Invoke();
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
