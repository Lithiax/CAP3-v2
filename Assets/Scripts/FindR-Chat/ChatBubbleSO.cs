using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New ChatBubble", menuName = "ChatBubble")]
public class ChatBubbleSO : ScriptableObject
{
    public bool isUser;

    [TextArea(5, 15)]
    public string chatText;
}
