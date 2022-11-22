using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using System.Linq;

[System.Serializable]
public class ChatUserData
{
    public List<ChatBubble> ChatBubbles = new List<ChatBubble>();
    public string name;
    public DialogueContainer CurrentTree;
    public ChatUserSO UserSO;
    public string CurrentNodeGUID;
    public int CurrentDialogueIndex;
    public bool CurrentDialogueComplete;
    public float RGMeter;
    public bool WasBranchEffect;
    public bool ActiveDialogue;
    public int CurrentRGIndex;
    public bool CanRGText;
    public ChatUserData(ChatUserSO userSO)
    {
        UserSO = userSO;
        name = userSO.profileName;
        CurrentTree = userSO.dialogueTree;
        CurrentDialogueIndex = 0;
        CurrentDialogueComplete = false;
        CurrentRGIndex = 0;
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

    [Header("Online Indicator")]
    [SerializeField] Image onlineIndicator;
    [SerializeField] Color offlineColor;
    [SerializeField] Color onlineColor;

    [HideInInspector] public ChatUserSO ChatUserSO { get; private set; }
    [SerializeField] HealthUI healthUI;
    public ChatUserData ChatData { get; private set; }
    GameObject Divider;
    GameObject newMatchPanel;
    RectTransform panelRectTransform;
    public bool isToggled { get; private set; }

    int notifNum = 0;

    GameData gameData;

    public Action<string> OnRemoveEffect;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        DialogueTree = GetComponent<DialogueGraphAPI>();
        panelRectTransform = GetComponent<RectTransform>();

        DialogueTree.OnNodeChanged += OnNodeChange;
    }

    public void ModifyHealth(int health)
    {
        healthUI.OnModifyHealthEvent?.Invoke(health);
    }

    public void Init(ChatUserSO data, FindREventsManager eventManager, ChatManagerUI manager, ToggleGroup toggleGroup)
    {
        Debug.Log("Init");
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

        Debug.Log("Load Data: " + gameData);
        if (gameData != null)
        {
            //We clear StaticUserData since its temporary, we reset it to this current save state.
            StaticUserData.ChatUserData.Clear();
            LoadGameData(data, gameData);
        }
        //If ChatUser is new
        else if (!StaticUserData.ChatUserData.Any(x => x.name == data.profileName))
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
            
            if (DialogueTree.DialogueTree == null)
                LoadChatData(ChatData);

            LoadRGDataFromStatic(ChatData);

            StaticUserData.ChatUserData.Add(ChatData);

            if (DialogueTree.DialogueTree != null)
            {
                Divider = chatManager.SpawnDivider();
            }
        }
        //If ChatUser is not new (FOR TESTING)
        else
        {
            ChatData = StaticUserData.ChatUserData.First(x => x.UserSO == data);
            Debug.Log("Load Chat Data: " + ChatData.name);

            LoadChatData(ChatData);
            LoadRGDataFromStatic(ChatData);

            if (ChatData.CurrentDialogueIndex != 0)
            {
                DialogueTree.ForceJumpToNode(ChatData.CurrentNodeGUID, ChatData.CurrentDialogueIndex);
            }

            if (DialogueTree.DialogueTree != null)
            {
                Divider = chatManager.SpawnDivider();
            }
        }

        //initialChatCollection = DialogueTree.CurrentNode.BaseNodeData.chatCollection as ChatCollectionSO;
        //currentCollection = initialChatCollection;

        profileName.text = data.profileName;
        profileImage.sprite = data.profileImage;


        if (DialogueTree.DialogueTree == null)
        {
            onlineIndicator.color = offlineColor;
            panelRectTransform.SetAsLastSibling();
            chatManager.ReplyClicked(0);
            ChatData.ActiveDialogue = false;

            if (chatsObj.Count <= 0)
            {
                newMatchPanel = chatManager.SpawnNewMatchPanel(data);
            }

            return;
        }
        ChatData.ActiveDialogue = true;
        onlineIndicator.color = onlineColor;
        panelRectTransform.SetAsFirstSibling();
        chatManager.StartSpawningChat(this, DialogueTree);
    }

    void LoadRGDataFromStatic(ChatUserData data)
    {
        switch (data.UserSO.profileName)
        {
            case "Maeve":
                data.RGMeter = DialogueSpreadSheetPatternConstants.maeveHealth;
                break;

            case "Liam":
                data.RGMeter = DialogueSpreadSheetPatternConstants.liamHealth;
                break;

            case "Brad":
                data.RGMeter = DialogueSpreadSheetPatternConstants.bradHealth;
                break;

            case "Penelope":
                data.RGMeter = DialogueSpreadSheetPatternConstants.penelopeHealth;
                break;

            default:
                Debug.Log("User does not have a health bar");
                break;
        }

        Debug.Log("Load RGMeter: " + data.RGMeter);
        healthUI.currentHealth = data.RGMeter;
        UpdateHealthBar(data.RGMeter);

    }

    void UpdateHealthBar(float meter)
    {
        float fill = meter / 100f;
        healthUI.InstantUpdateBar(fill);
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


        //Get appropriate dialogue tree
        if (ChatUserSO.dialogueBranches == null) return;

        DialogueContainer c = SetDialogueContainer();

        if (c == null && data.CanRGText)
        {
            SetRGScript(data);
        }
    }

    void SetRGScript(ChatUserData data)
    {
        if (data.CurrentRGIndex >= data.UserSO.RGScripts.Count)
        {
            Debug.Log("Max RGs Reached!");
            return;
        }

        DialogueTree.SetDialogueTree(data.UserSO.RGScripts[data.CurrentRGIndex]);
        ChatData.CurrentTree = DialogueTree.DialogueTree;
        data.CurrentRGIndex++;
    }

    DialogueContainer SetDialogueContainer()
    {
        DialogueContainer nextContainer = null;
        foreach (string s in DialogueSpreadSheetPatternConstants.effects)
        {
            DialogueContainer d = ChatUserSO.dialogueBranches.GetBranch(s);
            if (d)
            {
                nextContainer = d;
                DialogueTree.SetDialogueTree(nextContainer);
                ChatData.CurrentTree = DialogueTree.DialogueTree;

                Debug.Log("Removing Effect: " + s);
                OnRemoveEffect?.Invoke(s);

                ChatData.CanRGText = true;

                break;
            }
        }
        return nextContainer;
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
                ChatData.CurrentTree = DialogueTree.DialogueTree;

                Debug.Log("Removing Effect: " + s);
                OnRemoveEffect?.Invoke(s);

                break;
            }
        }

        if (nextContainer == null)
        {
            Debug.Log("NO NEXT BRANCH FOUND!");
            return;
        }

        ChatData.CurrentDialogueComplete = false;
        chatManager.StartSpawningChat(this, DialogueTree);
    }

    public void SetNewEventTree()
    {
        if (SetDialogueContainer() == null)
        {
            Debug.Log("NO EVENT FOUND!");
            return;
        }
        chatManager.OnSetNextTree += SetNextTree;
        ChatData.WasBranchEffect = true;

        ChatData.CurrentDialogueComplete = false;
        chatManager.StartSpawningChat(this, DialogueTree);
    }

    void OnNodeChange()
    {
        ChatData.CurrentDialogueComplete = true;
    }

    private void OnDisable()
    {
        chatManager.OnSetNextTree -= SetNextTree;
        ChatData.WasBranchEffect = false;
        DialogueTree.OnNodeChanged -= OnNodeChange;
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
        PreviousChat = chat;
    }

    public void SetLastMessageText(string text)
    {
        lastMessageText.text = text;
        panelRectTransform.SetAsFirstSibling();
    }

    public void SwitchChat(bool tog)
    {
        isToggled = tog;

        ResetNotif();
        chatManager.ActivateChat(chatsObj, tog);

        Divider?.SetActive(tog);
        newMatchPanel?.SetActive(tog);

        toggle.image.color = tog ? toggleActivatedColor : toggleUnactiveColor;


        if (!tog) return;

        chatManager.HandleResponse(this, DialogueTree);
    }

    //use this instead to ensure script execution
    public void LoadGameData(ChatUserSO userData, GameData data)
    {
        ChatData = new ChatUserData(userData);

        //Set to intial tree if it has one.

        if (ChatData.ActiveDialogue)
            DialogueTree.SetDialogueTree(userData.dialogueTree);
        else
            DialogueTree.SetDialogueTree(null);

        StaticUserData.ChatUserData.Add(ChatData);

        //Spawn in chats that were already done before.

        if (data.ChatUserData.Any(x => x.name == ChatUserSO.profileName))
        {
            ChatData = data.ChatUserData.First(x => x.name == ChatUserSO.profileName);
            healthUI.currentHealth = ChatData.RGMeter;
            UpdateHealthBar(ChatData.RGMeter);

            if (userData.dialogueTree != null)
            {
                DialogueTree.SetDialogueTree(ChatData.CurrentTree);
                if (ChatData.WasBranchEffect == true)
                {
                    chatManager.OnSetNextTree += SetNextTree;
                }
                Divider = chatManager.SpawnDivider();
            }
           
            LoadChatData(ChatData);
            DialogueTree.ForceJumpToNode(ChatData.CurrentNodeGUID, ChatData.CurrentDialogueIndex);
        }

    }
    public void AllowLoadingData(GameData data)
    {
        this.gameData = data;
    }

    public void LoadData(GameData data)
    {
        return;
    }

    public void SaveData(ref GameData data)
    {

        if (DialogueTree.DialogueTree != null)
        {
            ChatData.CurrentNodeGUID = DialogueTree.CurrentNode.BaseNodeData.NodeGUID;
            ChatData.CurrentDialogueIndex = DialogueTree.CurrentNode.CurrentIndex;
        }

        //Note: Not sure if ChatData updating will automatically update this ?
        //It does, make new object to save.

        //if data exists
        if (data.ChatUserData.Any(x => x.UserSO == ChatUserSO))
        {
            ChatUserData d = data.ChatUserData.First(x => x.UserSO == ChatUserSO);

            //I hope c#'s garbage collector collects this 
            data.ChatUserData.Remove(d);
            
            ChatUserData newData = new ChatUserData(ChatUserSO);
            data.ChatUserData.Add(ChatData);
        }
        //if data does not exist
        else
        {
            Debug.Log("Creating new user data");
            ChatUserData newData = new ChatUserData(ChatUserSO);
            data.ChatUserData.Add(ChatData);
        }
    }
}


/* TODO
 * 1. Load User Data - Done
 * 1.5 Load Initial Chat Objects - Done
 * 2. Load Previous Chat Objects - Done
 * 3. Load Proper Dialogue Tree - Done
 * 4. Jump to previous Node - Done
 * 5. Fix Weird bug where UI is not loaded for the first chat - Not Done
 * 6. Fix bug where if you save while chat is loading on the last element. it resets it due to index going back to 0.
*/
