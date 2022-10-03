using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatBubbleUI : MonoBehaviour
{
    [HideInInspector] public ChatUser ChatUserParent;
    [SerializeField] TextMeshProUGUI chatText;
    [SerializeField] VerticalLayoutGroup chatLayoutGroup;
    [SerializeField] VerticalLayoutGroup anchorLayoutGroup;
    public ContentSizeFitter andchorFitter;
    [SerializeField] Image chatBG;
    [SerializeField] Color userChatColor;

    public void SetUpChat(ChatUser parent, string chat, bool user)
    {
        parent = ChatUserParent;
        chatText.text = chat;

        if (user)
        {
            chatBG.color = userChatColor;
            chatLayoutGroup.childAlignment = TextAnchor.UpperRight;
            anchorLayoutGroup.childAlignment = TextAnchor.UpperRight;
        }
    }
}
