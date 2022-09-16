using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorylineManager : MonoBehaviour
{
    [NonReorderable][SerializeField] public SO_Dialogues startSO_Dialogues;
    [NonReorderable][SerializeField] public List<SO_Dialogues> so_Dialogues;

    public void Start()
    {
        CharacterDialogueUI.onCharacterSpokenToEvent.Invoke("None", startSO_Dialogues);
        //AudioManager.instance.PlayOnRoomEnterString("QuestComplete");
    }

}
