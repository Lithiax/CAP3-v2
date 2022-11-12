using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatBubbleUI : MonoBehaviour
{
    [HideInInspector] public ChatUser ChatUserParent;
    [SerializeField] TextMeshProUGUI chatText;
    [SerializeField] GameObject chatTextObj;
    [SerializeField] GameObject waitChatObj;
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


    float waitTime = 0.5f;
    Vector2 OriginalImageSize;
    private void Awake()
    {
        OriginalImageSize = ProfileImage.rectTransform.sizeDelta;
    }
    public void SetUpChat(ChatUser parent, ChatBubble data, bool isNew)
    {
        SetUI(parent, data);

        if (isNew)
        {
            chatTextObj.SetActive(false);
            waitChatObj.SetActive(true);
        }

        chatText.text = data.chatText;
    }

    public void ShowText()
    {
        chatTextObj.SetActive(true);
        waitChatObj.SetActive(false);
    }

    void SetTyping()
    {
        StartCoroutine(IsTypingWait());
    }
    IEnumerator IsTypingWait()
    {
        yield return new WaitForSeconds(waitTime);

        chatTextObj.SetActive(true);
        waitChatObj.SetActive(false);
    }

    void SetWaitTime(int length)
    {
        int rand = Random.Range(-30, 30);
        int CPM = 350 + rand;

        waitTime = (length / CPM) * 60;
    }

    void SetUI(ChatUser parent, ChatBubble data)
    {
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

        if (!parent.PreviousChat.isUser && parent.PreviousChat.chatText != "")
        {
            ProfileImage.sprite = null;
            //Actual width/height = 68.028
            ProfileImage.rectTransform.sizeDelta = new Vector2(OriginalImageSize.x, 5);
            ProfileNameObj.SetActive(false);
        }
    }
}
