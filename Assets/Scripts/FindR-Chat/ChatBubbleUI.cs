using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatBubbleUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI chatText;
    [SerializeField] VerticalLayoutGroup chatLayoutGroup;
    [SerializeField] VerticalLayoutGroup anchorLayoutGroup;
    public ContentSizeFitter andchorFitter;
    [SerializeField] Image chatBG;

    public Action OnPrompt;
    bool prompt = false;

    public void SetUpChat(string chat, bool user, bool isPrompt)
    {
        chatText.text = chat;
        prompt = isPrompt;

        if (user)
        {
            chatBG.color = new Color32(115, 245, 130, 255);
            chatLayoutGroup.childAlignment = TextAnchor.UpperRight;
            anchorLayoutGroup.childAlignment = TextAnchor.UpperRight;
        }

        if (prompt && this.gameObject.activeSelf)
        {
            OnPrompt?.Invoke();
        }
    }

    public void OnEnable()
    {
        if (prompt)
        {
            OnPrompt?.Invoke();
        }
    }
}
