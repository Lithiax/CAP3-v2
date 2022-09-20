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
    public Toggle toggle;

    ChatManagerUI chatManager;
    [SerializeField] TextMeshProUGUI profileName;
    [SerializeField] TextMeshProUGUI lastMessageText;
    [SerializeField] Image profileImage;

    [HideInInspector] public bool inResponse = false;

    [Header("Notification")]
    [SerializeField] GameObject notifObj;
    [SerializeField] TextMeshProUGUI notifText;
    public bool isToggled { get; private set; }
    public bool OnPrompt = false;
    int notifNum = 0;
    private void Awake()
    {
        toggle = GetComponent<Toggle>();

        currentCollection = initialChatCollection;
    }

    public void Init(ChatCollectionSO initCol, string name, Sprite img, ChatManagerUI manager, ToggleGroup toggleGroup)
    {
        chatManager = manager;
        chatManager.chatUsers.Add(this);
        toggle.group = toggleGroup;

        isToggled = toggle.isOn;

        initialChatCollection = initCol;
        profileName.text = name;
        profileImage.sprite = img;
        chatManager.StartSpawningChat(this, initialChatCollection);
    }

    private void OnEnable()
    {
        if (OnPrompt)
            chatManager.HandleResponse(this, currentCollection);
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

        chatManager.ActivateChat(chats, toggle);
    }
}
