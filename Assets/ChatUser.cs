using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatUser : MonoBehaviour
{
    [SerializeField] List<ChatBubbleSO> chatData = new List<ChatBubbleSO>();
    [SerializeField] List<GameObject> chats = new List<GameObject>();
    Toggle toggle;
    [SerializeField] ChatManagerUI chatManager;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    private void Start()
    {
        SpawnChatBubbles();

        if (toggle.isOn)
        {
            chatManager.StartPlayChat(chats);
        }

        //SwitchChat(toggle.isOn);
    }

    void SpawnChatBubbles()
    {
        if (chats.Count == 0)
        {
            foreach (ChatBubbleSO data in chatData)
            {
                chats.Add(chatManager.SpawnChatBubble(data));
            }
        }
    }

    public void SwitchChat(bool toggle)
    {
        chatManager.HideResponse();
        foreach (GameObject chat in chats)
        {
            chat.SetActive(toggle);
        }
    }
}
