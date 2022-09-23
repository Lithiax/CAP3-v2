using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[System.Serializable]
public class ChatBubble
{
    public bool isUser;

    [TextArea(5, 15)]
    public string chatText;
}

[CreateAssetMenu(fileName = "New Chat Collection", menuName = "Chat Collection")]
public class ChatCollectionSO : ScriptableObject
{

    [Header("Chat Bubbles")]
    public List<ChatBubble> ChatBubbles;

    [Header("Prompts")]
    public string PromptText;
    public List<ChatCollectionSO> Prompts;

    [Header("Event Parameters")]

    public List<ChatEvent> ChatEvents;

    public bool isPrompt()
    {
        return Prompts.Count >= 1;
    }
    public bool isEvent()
    {
        return ChatEvents.Count >= 1;
    }
}
