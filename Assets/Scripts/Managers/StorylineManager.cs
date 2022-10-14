using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorylineManager : MonoBehaviour
{
    public static StorylineManager instance;
    public SO_Character mainCharacter;
    public static SO_Dialogues currentSO_Dialogues;
    public static int currentDialogueIndex;
    private void Awake()
    {
        instance = this;
    }


}
