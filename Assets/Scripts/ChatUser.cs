using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ChatUserData
{
    public List<ChatBubble> ChatBubbles = new List<ChatBubble>();
    public string name;
    public DialogueContainer CurrentTree;
    public ChatUserSO UserSO;
    public DialogueTreeNode CurrentNode;
    public ChatUserData(ChatUserSO userSO)
    {
        UserSO = userSO;
        name = userSO.profileName;
        CurrentTree = userSO.dialogueTree;
    }
}

public class ChatUser : MonoBehaviour, IDataPersistence
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
    public ChatUserData ChatData { get; private set; }
    GameObject Divider;
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

        foreach (ChatUserData d in StaticUserData.ChatUserData)
        {
            Debug.Log(d.UserSO.profileName);
        }

        //If ChatUser is new
        if (!StaticUserData.ChatUserData.Any(x => x.name == data.profileName))
        {
            Debug.Log("Chat User is New");
            //ADD SAVING CHAT OBJECTS INTO THE CHAT USER DATA
            ChatData = new ChatUserData(data);

            if (data.initialPreviousChat)
            {
                foreach (ChatBubble chat in data.initialPreviousChat.ChatData)
                {
                    GameObject obj = chatManager.SpawnChatBubble(chat, this);
                    chatsObj.Add(obj);
                    chatManager.RebuildAfterSpawning();
                    ChatData.ChatBubbles.Add(chat);
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
            ChatData = StaticUserData.ChatUserData.First(x => x.UserSO == data);
            Debug.Log("Load Chat Data: " + ChatData.name);

            LoadChatData(ChatData);
        }


        //initialChatCollection = DialogueTree.CurrentNode.BaseNodeData.chatCollection as ChatCollectionSO;
        //currentCollection = initialChatCollection;

        profileName.text = data.profileName;
        profileImage.sprite = data.profileImage;


        if (DialogueTree.DialogueTree == null)
        {
            chatManager.ReplyClicked(0);
            return;
        }

        Divider = chatManager.SpawnDivider();
        chatManager.StartSpawningChat(this, DialogueTree);
    }

    void LoadChatData(ChatUserData data)
    {
        //Spawn in Previous Chat Objects
        foreach (ChatBubble chat in data.ChatBubbles)
        {
            GameObject obj = chatManager.SpawnChatBubble(chat, this);
            chatsObj.Add(obj);

            PreviousChat = chat;
        }
        chatManager.RebuildAfterSpawning();


        //TODO: Set dialogue containers based on LEVEL not just effects. :^) 

        //Get appropriate dialogue tree
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
            return;
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

        //DEBUG Purposes
        //DialogueSpreadSheetPatternConstants.effects.Add("Drunk");

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

    private void OnDisable()
    {
        chatManager.OnCurrentNodeEnded -= SetNextTree;
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

    public void OnChatSpawned(ChatBubble chat, GameObject spawnedObj)
    {
        //chatsObj.Add(spawnedObj);
        chatsObj.Add(spawnedObj);
        ChatData.ChatBubbles.Add(chat);
        PreviousChat = chat;
    }

    public void SetLastMessageText(string text)
    {
        lastMessageText.text = text;
    }

    public void SwitchChat(bool tog)
    {
        isToggled = tog;

        ResetNotif();
        chatManager.ActivateChat(chatsObj, tog);

        Divider?.SetActive(tog);

        toggle.image.color = tog ? toggleActivatedColor : toggleUnactiveColor;


        if (!tog) return;
        if (DialogueTree.DialogueTree == null) return;
        chatManager.HandleResponse(this, DialogueTree);
    }

    public void LoadData(GameData data)
    {
        foreach (string effect in DialogueSpreadSheetPatternConstants.effects)
        {
            Debug.Log(effect);
        }

        return;

        ChatUserData chatData = data.ChatUserData.First(x => x.UserSO == ChatUserSO);
        //ChatData = data.ChatUserData.First(x => x.UserSO == ChatUserSO);

        if (chatData == null)
        {
            Debug.LogError("ChatData Save File Not Found!");
            return;
        }    
        
        LoadChatData(chatData);

        //DialogueTree.ForceJumpToNode(chatData.CurrentNode);
    }

    public void SaveData(ref GameData data)
    {
        return;

        ChatData.CurrentNode = DialogueTree.CurrentNode;
        //Note: Not sure if ChatData updating will automatically update this ?
        //It does, make new object to save.
        if (!data.ChatUserData.Any(x => x.UserSO == ChatUserSO))
        {
            ChatUserData newData = new ChatUserData(ChatUserSO);
            data.ChatUserData.Add(newData);
        }

        else
        {
            ChatUserData d = data.ChatUserData.First(x => x.UserSO == ChatUserSO);

            //I hope c#'s garbage collector collects this 
            data.ChatUserData.Remove(d);
            
            ChatUserData newData = new ChatUserData(ChatUserSO);
            data.ChatUserData.Add(newData);
        }
    }
}
