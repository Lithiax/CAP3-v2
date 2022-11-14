using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SODialogueLoader : MonoBehaviour
{
    [SerializeField]
    private GameObject frame;
    [SerializeField] private TMP_InputField folderField;
    [SerializeField] private TMP_InputField sheetField;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            Debug.Log("CHEAT PRESSED");
            frame.SetActive(!frame.activeSelf);
        }
    }
    public void LoadSODialogueButtonUI()
    {
        StorylineManager.LoadVisualNovel(folderField.text, sheetField.text);
        //SO_Character mainCharacter = Resources.Load<SO_Character>("Scriptable Objects/Characters/You");
        //mainCharacter.stageName = "You";
        //StorylineManager.currentSO_Dialogues = Resources.Load<SO_Dialogues>("Scriptable Objects/Dialogues/Visual Novel/" + folderField.text + "/" + sheetField.text);
        //StorylineManager.currentInteractibleChoices = Resources.Load<SO_InteractibleChoices>("Scriptable Objects/Dialogues/Visual Novel/" + folderField.text + "/" + "Interactible Choices");
        if (StorylineManager.currentSO_Dialogues != null)
        {
            Debug.Log("DIALOGUE LOADED");
            CharacterDialogueUI.onCharacterSpokenTo.Invoke("");
        
            frame.SetActive(false);
        }
        else
        {
            Debug.Log("IT DOESNT WORK");
        }
    }


}
