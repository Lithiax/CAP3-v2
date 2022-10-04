using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ChatUserData
{
    public List<GameObject> ChatObjects = new List<GameObject>();
    public string name;
    public DialogueContainer CurrentTree;
}

public class ChatUser : MonoBehaviour
{
    //[SerializeField] ChatCollectionSO initialChatCollection;
    public DialogueGraphAPI DialogueTree { get; private set; }

    //public ChatCollectionSO currentCollection;
    List<GameObject> chatsObj = new List<GameObject>();
    [HideInInspector] public Toggle toggle;
    [SerializeField] Color toggleActivatedColor;
    [SerializeField] Color toggleUnactiveColor;

    ChatManagerUI chatManager;
    [SerializeField] TextMeshProUGUI profileName;
    [SerializeField] TextMeshProUGUI lastMessageText;
    [SerializeField] Image profileImage;

    [HideInInspector] public bool currentChatComplete = false;
    [HideInInspector] public ChatBubble SingleResponseChat = null;
    [HideInInspector] public ChatBubble PreviousChat = null;

    [Header("Notification")]
    [SerializeField] GameObject notifObj;
    [SerializeField] TextMeshProUGUI notifText;

    [HideInInspector] public ChatUserSO ChatUserSO { get; private set; }
    ChatUserData ChatData;
    public bool isToggled { get; private set; }

    int notifNum = 0;
    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        DialogueTree = GetComponent<DialogueGraphAPI>();
    }

    public void Init(ChatUserSO data, FindREventsManager eventManager, ChatManagerUI manager, ToggleGroup toggleGroup)
    {
        eventManager.ChatUsers.Add(this);
        ChatUserSO = data;
        chatManager = manager;
        chatManager.chatUsers.Add(this);
        toggle.group = toggleGroup;

        isToggled = toggle.isOn;

        //If ChatUser is new
        if (!StaticUserData.ChatUserData.Any(x => x.name == data.profileName))
        {
            //ADD SAVING CHAT OBJECTS INTO THE CHAT USER DATA
            ChatData = new ChatUserData
            {
                name = data.profileName,
                CurrentTree = data.dialogueTree
            };

            if (data.initialPreviousChat)
            {
                foreach (ChatBubble chat in data.initialPreviousChat.ChatData)
                {
                    GameObject obj = chatManager.SpawnChatBubble(chat, this);
                    chatsObj.Add(obj);
                    PreviousChat = chat;
                }

                chatManager.RebuildAfterSpawning();
            }

            DialogueTree.SetDialogueTree(data.dialogueTree);

            StaticUserData.ChatUserData.Add(ChatData);
        }
        //If ChatUser is not new (FOR TESTING)
        else
        {
            ChatData = StaticUserData.ChatUserData.First(x => x.name == name);

            foreach (GameObject chat in ChatData.ChatObjects)
            {
                chatManager.SpawnChatObjects(chat, isToggled);
            }
            chatManager.RebuildAfterSpawning();

            //Get next container
            DialogueContainer nextContainer = null;
            foreach(string s in DialogueSpreadSheetPatternConstants.effects)
            {
                DialogueContainer d = data.dialogueBranches.GetBranch(s);
                if (d)
                {
                    nextContainer = d;
                    DialogueTree.SetDialogueTree(nextContainer);
                    break;
                }
            }

            if (nextContainer == null)
                DialogueTree.SetDialogueTree(ChatData.CurrentTree);
        }


        //initialChatCollection = DialogueTree.CurrentNode.BaseNodeData.chatCollection as ChatCollectionSO;
        //currentCollection = initialChatCollection;

        profileName.text = data.profileName;
        profileImage.sprite = data.profileImage;
        chatManager.StartSpawningChat(this, DialogueTree);
    }

    public void SetNextTree()
    {
        DialogueContainer nextContainer = null;
        foreach (string s in DialogueSpreadSheetPatternConstants.effects)
        {
            DialogueContainer d = ChatUserSO.dialogueBranches.GetPostBranch(s);
            if (d)
            {
                nextContainer = d;
                DialogueTree.SetDialogueTree(nextContainer);
                //TEMP TEMP CHANGE IF NECESSARY
                DialogueSpreadSheetPatternConstants.effects.Remove(s);
                break;
            }
        }

        if (nextContainer == null)
        {
            Debug.Log("NO NEXT BRANCH FOUND!");
            return;
        }

        chatManager.StartSpawningChat(this, DialogueTree);
    }

    public void SetNewEventTree()
    {
        DialogueContainer nextContainer = null;
        foreach (string s in DialogueSpreadSheetPatternConstants.effects)
        {
            DialogueContainer d = ChatUserSO.dialogueBranches.GetBranch(s);
            if (d)
            {
                nextContainer = d;
                DialogueTree.SetDialogueTree(nextContainer);
                break;
            }
        }

        if (nextContainer == null)
        {
            Debug.LogError("NO EVENT FOUND!");
            return;
        }
        chatManager.OnCurrentNodeEnded += SetNextTree;
        chatManager.StartSpawningChat(this, DialogueTree);
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

    public void OnChatSpawned(ChatBubble chat, GameObject spawnedObj, string text)
    {
        chatsObj.Add(spawnedObj);
        PreviousChat = chat;
        lastMessageText.text = text;
    }

    public void SwitchChat(bool tog)
    {
        isToggled = tog;

        ResetNotif();
        chatManager.ActivateChat(chatsObj, tog);

        toggle.image.color = tog ? toggleActivatedColor : toggleUnactiveColor;


        if (!tog) return;
        Debug.Log(profileName.text);
        chatManager.HandleResponse(this, DialogueTree);
    }
}
