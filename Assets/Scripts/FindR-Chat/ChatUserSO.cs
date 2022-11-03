using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Chat User", menuName = "Chat User")]
public class ChatUserSO : ScriptableObject
{
    public Sprite profileImage;
    public int ID;
    public string profileName;
    //public ChatCollectionSO initialChatCollection;
    public DialogueContainer dialogueTree;

    public DialogueBranchesSO dialogueBranches;
    public ChatCollectionSO initialPreviousChat;
}
