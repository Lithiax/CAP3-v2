using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorylineManager : MonoBehaviour
{
    public static StorylineManager instance;
    [NonReorderable][SerializeField] public SO_Dialogues startSO_Dialogues;
    [NonReorderable][SerializeField] public List<SO_Dialogues> so_Dialogues;
    [SerializeField] public CharacterDialogueUI temp;
    private void Awake()
    {
        instance = this;
    }
    public void Start()
    {
        //temp.OnCharacterSpokenTo("",startSO_Dialogues);
        //CharacterDialogueUI.onCharacterSpokenToEvent.Invoke("None", startSO_Dialogues);
        //AudioManager.instance.PlayOnRoomEnterString("QuestComplete");
    }

}
