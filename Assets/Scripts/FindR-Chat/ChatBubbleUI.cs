using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatBubbleUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI chatText;
    [SerializeField] VerticalLayoutGroup chatLayoutGroup;
    [SerializeField] VerticalLayoutGroup anchorLayoutGroup;
    [SerializeField] Image chatBG;

    public void SetUpChat(string chat, bool user)
    {
        chatText.text = chat;

        if (user)
        {
            chatBG.color = new Color32(115, 245, 130, 255);
            chatLayoutGroup.childAlignment = TextAnchor.UpperRight;
            anchorLayoutGroup.childAlignment = TextAnchor.UpperRight;
        }
        
    }
}
