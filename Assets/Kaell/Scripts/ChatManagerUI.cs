using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class ChatManagerUI : MonoBehaviour
{
    [SerializeField] List<ChatBubbleSO> chatData = new List<ChatBubbleSO>();
    [SerializeField] Scrollbar scrollBar;
    [SerializeField] GameObject scrollArea;
    [SerializeField] VerticalLayoutGroup scrollLayout;
    [SerializeField] GameObject chatPrefab;

    private void Start()
    {
        StartCoroutine(Demo());
    }

    IEnumerator Demo()
    {
        yield return new WaitForSeconds(2f);
        SpawnChatBubble(chatData[0]);

        yield return new WaitForSeconds(2f);
        SpawnChatBubble(chatData[1]);

        yield return new WaitForSeconds(2f);
        SpawnChatBubble(chatData[2]);
    }

    void SpawnChatBubble(ChatBubbleSO data)
    {
        GameObject chatBubble = GameObject.Instantiate(chatPrefab, scrollArea.transform);

        ChatBubbleUI chatText = chatBubble.GetComponent<ChatBubbleUI>();

        chatText.SetUpChat(data.chatText, data.isUser);
        StartCoroutine(Buffer(chatBubble));
    }

    IEnumerator Buffer(GameObject chatBubble)
    {
        yield return new WaitForSeconds(0.1f);
        scrollBar.value = 0;
    }
}
