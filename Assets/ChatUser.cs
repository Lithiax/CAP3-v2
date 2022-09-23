using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class ChatUser : MonoBehaviour
{
    [SerializeField] ChatCollectionSO initialChatCollection;

     public ChatCollectionSO currentCollection;
    List<GameObject> chats = new List<GameObject>();
    [HideInInspector] public Toggle toggle;

    ChatManagerUI chatManager;
    [SerializeField] TextMeshProUGUI profileName;
    [SerializeField] TextMeshProUGUI lastMessageText;
    [SerializeField] Image profileImage;

    [HideInInspector] public bool currentChatComplete = false;

    [Header("Notification")]
    [SerializeField] GameObject notifObj;
    [SerializeField] TextMeshProUGUI notifText;
    public bool isToggled { get; private set; }

    int notifNum = 0;
    private void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    public void Init(ChatCollectionSO initCol, string name, Sprite img, ChatManagerUI manager, ToggleGroup toggleGroup)
    {
        chatManager = manager;
        chatManager.chatUsers.Add(this);
        toggle.group = toggleGroup;

        isToggled = toggle.isOn;

        initialChatCollection = initCol;
        currentCollection = initCol;
        profileName.text = name;
        profileImage.sprite = img;
        chatManager.StartSpawningChat(this, initialChatCollection);
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

        ResetNotif();
        chatManager.ActivateChat(chats, toggle);

        if (!toggle) return;
        Debug.Log(profileName.text);
        chatManager.HandleResponse(this, currentCollection);
    }
}
