using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public string PromptText;
    public List<ChatBubble> ChatBubbles;

    public List<ChatCollectionSO> Prompts;
    public bool isPrompt;

    private void Awake()
    {
        isPrompt = (Prompts.Count >= 2);
    }
}
