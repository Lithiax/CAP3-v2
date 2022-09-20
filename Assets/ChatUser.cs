using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class ChatUser : MonoBehaviour
{
    [SerializeField] ChatCollectionSO initialChatCollection;
    [HideInInspector] public ChatCollectionSO currentCollection;
    List<GameObject> chats = new List<GameObject>();
    Toggle toggle;
    [SerializeField] ChatManagerUI chatManager;
    [SerializeField] TextMeshProUGUI lastMessageText;

    public Action OnPrompt;
    [HideInInspector] public bool inResponse = false;

    [Header("Notification")]
    [SerializeField] GameObject notifObj;
    [SerializeField] TextMeshProUGUI notifText;
    public bool isToggled { get; private set; }
    int notifNum = 0;
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

    public void SetNotif()
    {
        notifNum++;

        notifText.text = notifNum.ToString();
        notifObj.SetActive(true);
    }

    public void ResetNotif()
    {
        notifObj.SetActive(false);
        notifNum = 0;
    }

    public void OnChatSpawned(GameObject spawnedObj, string text)
    {
        chats.Add(spawnedObj);
        lastMessageText.text = text;
    }

    public void SwitchChat(bool toggle)
    {
        isToggled = toggle;
        chatManager.HandleResponse(this, currentCollection);
        ResetNotif();
        foreach (GameObject chat in chats)
        {
            chat.SetActive(toggle);
        }
    }
}
