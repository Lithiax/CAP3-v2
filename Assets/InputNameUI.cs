using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class InputNameUI : MonoBehaviour
{
    [SerializeField] private GameObject frame;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField]
    private SO_Character so_Character;

    public void ToggleUI()
    {
        frame.SetActive(!frame.activeSelf);
        if (frame.activeSelf == true)
        {
            //CharacterDialogueUI.OnStartChooseChoiceEvent.Invoke();
        }
        else if (frame.activeSelf == false)
        {
          
        }
    }
    public void SubmitInputName()
    {
        so_Character.stageName = inputField.text;
        StorylineManager.currentDialogueIndex++;
        //CharacterDialogueUI.OnEndEvent.Invoke();
        CharacterDialogueUI.OnContinueEvent.Invoke();
        ToggleUI();
    }
}
