using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatBubbleUI : MonoBehaviour
{
    [HideInInspector] public ChatUser ChatUserParent;
    [SerializeField] TextMeshProUGUI chatText;
    [SerializeField] HorizontalLayoutGroup chatLayoutGroup;
    [SerializeField] VerticalLayoutGroup anchorLayoutGroup;
    public ContentSizeFitter andchorFitter;
    [SerializeField] Image chatBG;
    [SerializeField] Color userChatColor;
    [SerializeField] Sprite userChatBG;

    [Header("Non-User Chat Elements")]
    [SerializeField] GameObject NotUserElements;
    [SerializeField] Image ProfileImage;
    [SerializeField] GameObject ProfileNameObj;
    [SerializeField] TextMeshProUGUI ProfileName;
    public void SetUpChat(ChatUser parent, ChatBubble data)
    {
        chatText.text = data.chatText;

        if (!data.isUser)
        {
            ProfileImage.sprite = parent.ChatUserSO.profileImage;
            ProfileName.text = parent.ChatUserSO.profileName;
        }
        else
        {
            NotUserElements.SetActive(false);
            ProfileNameObj.SetActive(false);
            chatBG.sprite = userChatBG;
            chatText.color = userChatColor;
            chatLayoutGroup.childAlignment = TextAnchor.LowerRight;
            anchorLayoutGroup.childAlignment = TextAnchor.UpperRight;
        }

        if (parent.PreviousChat == null) return;
        if (!parent.PreviousChat.isUser)
        {
            ProfileImage.sprite = null;
            ProfileNameObj.SetActive(false);
        }
    }
}
