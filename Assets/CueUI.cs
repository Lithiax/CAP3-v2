using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
public class CueUI : MonoBehaviour
{
  
    [SerializeField] RectTransform rectTransform;
    [SerializeField] TMP_Text cueTypeValueText;
    [SerializeField] Image iconImage;
    [SerializeField] TMP_Text cueValueText;

    [SerializeField] private GameObject choiceUIsContainer;
    [SerializeField] private RectTransform choiceUIsContainerRectTransform;
    [SerializeField] private ChoiceUI choiceUIPrefab;

    public static Action onChoiceChosenEvent;
    private void Awake()
    {
        choiceUIsContainerRectTransform = choiceUIsContainer.GetComponent<RectTransform>();
        gameObject.SetActive(false);

    }
    public void Initialize(string p_cueTypeValue, Sprite p_iconImage, string p_cueValueText, Vector2 p_position)
    {
        cueTypeValueText.text = p_cueTypeValue;
        iconImage.sprite = p_iconImage;
        cueValueText.text = p_cueValueText;
        rectTransform.anchoredPosition = p_position;
        Debug.Log("SEARCHING CUE TYPE : " + p_cueTypeValue);
        if (StorylineManager.cuesChoices != null)
        {
            List<LocalCueChoice> choice = StorylineManager.GetCueChoiceDatas(p_cueTypeValue);
            CreateChoiceUIs(choice);
        }
    }

    void CreateChoiceUIs(List<LocalCueChoice> p_choiceDatas)
    {
        choiceUIsContainer.SetActive(true);
        for (int i = 0; i < p_choiceDatas.Count; i++)
        {
            if (p_choiceDatas[i].wasChosen == false)
            {
                ChoiceUI newChoiceUI = Instantiate(choiceUIPrefab, choiceUIsContainerRectTransform);
                newChoiceUI.InitializeValues(p_choiceDatas[i].choiceData.words);
                ChoiceData currentChoiceData = p_choiceDatas[i].choiceData;
                if (HealthUI.myDelegate.Invoke(StorylineManager.currentSO_Dialogues.choiceDatas[i].healthCeilingCondition, StorylineManager.currentSO_Dialogues.choiceDatas[i].healthFloorCondition))
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
           

        }
    }
    void CreateChoiceUIs(List<ChoiceData> p_choiceDatas)
    {
        choiceUIsContainer.SetActive(true);
        for (int i = 0; i < p_choiceDatas.Count; i++)
        {
            ChoiceUI newChoiceUI = Instantiate(choiceUIPrefab, choiceUIsContainerRectTransform);
            newChoiceUI.InitializeValues(p_choiceDatas[i].words);
            ChoiceData currentChoiceData = p_choiceDatas[i];
            if (HealthUI.myDelegate.Invoke(StorylineManager.currentSO_Dialogues.choiceDatas[i].healthCeilingCondition, StorylineManager.currentSO_Dialogues.choiceDatas[i].healthFloorCondition))
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
    }
    public void ResetChoiceManager()
    {
        if (choiceUIsContainer.activeSelf)
        {
            for (int i = 0; i < choiceUIsContainerRectTransform.childCount; i++)
            {
                Destroy(choiceUIsContainerRectTransform.GetChild(i).gameObject);

            }
        }
        choiceUIsContainer.SetActive(false);
    }
    public void ChooseChoiceUI(ChoiceData p_currentChoiceData)
    {
        Debug.Log("CUE CHOSEN");
        LocalCueChoice t = StorylineManager.GetLocalCueChoice(p_currentChoiceData);
        t.wasChosen = true;
        StorylineManager.savedDialogueIndex = StorylineManager.currentDialogueIndex;
        StorylineManager.savedSO_Dialogues = StorylineManager.currentSO_Dialogues;
        StorylineManager.sideDialogue = true;
        CharacterDialogueUI.OnStartChooseChoiceEvent.Invoke();
        onChoiceChosenEvent.Invoke();
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
            PopUpUI.OnPopUpEvent.Invoke(p_currentChoiceData.popUpTitle, p_currentChoiceData.popUpContent);
            CharacterDialogueUI.OnPopUpEvent.Invoke(p_currentChoiceData.branchDialogue);
            //CharacterDialogueUI.OnContinueEvent.Invoke();
            //StorylineManager.currentSO_Dialogues = p_currentChoiceData.branchDialogue;
            //StorylineManager.currentDialogueIndex = 0;

        }
        else
        {
            StorylineManager.currentSO_Dialogues = p_currentChoiceData.branchDialogue;
            CharacterDialogueUI.OnEndChooseChoiceEvent.Invoke();
        
        }

    }

    public void ExitUI(Collision collision)
    {
        
    }
}
