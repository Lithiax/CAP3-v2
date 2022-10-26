using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class CueUI : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] TMP_Text cueTypeValueText;
    [SerializeField] Image iconImage;
    [SerializeField] TMP_Text cueValueText;

    [SerializeField] private GameObject choiceUIsContainer;
    private RectTransform choiceUIsContainerRectTransform;
    private ChoiceUI choiceUIPrefab;
    private void Awake()
    {
        choiceUIsContainerRectTransform = choiceUIsContainer.GetComponent<RectTransform>();

    }
    public void Initialize(string p_cueTypeValue, Sprite p_iconImage, string p_cueValueText, Vector2 p_position)
    {
        cueTypeValueText.text = p_cueTypeValue;
        iconImage.sprite = p_iconImage;
        cueValueText.text = p_cueValueText;
        rectTransform.anchoredPosition = p_position;
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
    void ResetChoiceManager()
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
            PopUpUI.OnPopUpEvent.Invoke(p_currentChoiceData.popUpTitle, p_currentChoiceData.popUpContent);
            CharacterDialogueUI.OnPopUpEvent.Invoke(p_currentChoiceData.branchDialogue);

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
