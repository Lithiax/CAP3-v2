using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class ChatUser : MonoBehaviour
{
    [SerializeField] ChatCollectionSO initialChatCollection;
    ChatCollectionSO currentCollection;
    List<GameObject> chats = new List<GameObject>();
    Toggle toggle;
    [SerializeField] ChatManagerUI chatManager;
    [SerializeField] TextMeshProUGUI lastMessageText;

    public Action OnPrompt;
    public bool inResponse = false;
    public bool isToggled { get; private set; }
    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        isToggled = toggle.isOn;
        chatManager.chatUsers.Add(this);

        currentCollection = initialChatCollection;
    }

    private void Start()
    {
        chatManager.StartSpawningChat(this, initialChatCollection);

        //SwitchChat(toggle.isOn);
    }

    public void OnChatSpawned(GameObject spawnedObj, string text)
    {
        chats.Add(spawnedObj);
        lastMessageText.text = text;
    }

    void ReplyClicked(int num)
    {
        currentCollection = currentCollection.Prompts[num];
        chatManager.StartSpawningChat(this, currentCollection);
    }

    public void SwitchChat(bool toggle)
    {
        isToggled = toggle;
        chatManager.HandleResponse(this, currentCollection);

        foreach (GameObject chat in chats)
        {
            chat.SetActive(toggle);
        }
    }
}
