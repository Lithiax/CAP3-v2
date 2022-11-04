using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorylineManager : MonoBehaviour
{
    public static StorylineManager instance;
    public SO_Character mainCharacter;
    public static SO_Dialogues currentSO_Dialogues;
    public static SO_Dialogues currentSideDialogue;
    public static int currentDialogueIndex;
    public static List<Dialogue> loggedWords = new List<Dialogue>();
    public static string currentBackgroundMusic ="";
    private void Awake()
    {
        instance = this;
    }


}
