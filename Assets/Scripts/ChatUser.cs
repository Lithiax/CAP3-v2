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
    public string BranchEffect;
    public bool ActiveDialogue;
    public int CurrentRGIndex;
    public bool CanRGText;
    public bool StayInTree;
    public int DateProgress;
    public DialogueContainer NotSkippableDialogue;
    public string CurrentEffect;
    public ChatUserData(ChatUserSO userSO)
    {
        UserSO = userSO;
        name = userSO.profileName;
        CurrentTree = userSO.dialogueTree;
        CurrentDialogueIndex = 0;
        DateProgress = 0;
        CurrentDialogueComplete = false;
        CurrentRGIndex = 0;
        StayInTree = true;
        NotSkippableDialogue = null;
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
    [SerializeField] GameObject replyNotifObj;

    [Header("Online Indicator")]
    [SerializeField] Image onlineIndicator;
    [SerializeField] Color offlineColor;
    [SerializeField] Color onlineColor;

    [HideInInspector] public ChatUserSO ChatUserSO { get; private set; }
    [SerializeField] HealthUI healthUI;
    DateProgressUI dateProgressUI;
    public ChatUserData ChatData { get; private set; }
    GameObject Divider;
    GameObject newMatchPanel;
    RectTransform panelRectTransform;
    public bool isToggled { get; private set; }

    int notifNum = 0;

    GameData gameData;

    public Action<string> OnRemoveEffect;

    float healthRef;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        DialogueTree = GetComponent<DialogueGraphAPI>();
        panelRectTransform = GetComponent<RectTransform>();
        dateProgressUI = GetComponentInChildren<DateProgressUI>();

        DialogueTree.OnNodeChanged += OnNodeChange;

    }

    public void ModifyHealth(int health)
    {
        healthUI.OnModifyHealthEvent?.Invoke(health);

        SaveRGData(ChatData);
    }

    public void ModifyDateProgress(string data)
    {
        dateProgressUI.AddHearts(ref ChatData.DateProgress, data);
    }

    public void Init(ChatUserSO data, FindREventsManager eventManager, ChatManagerUI manager, ToggleGroup toggleGroup)
    {
        Debug.Log("Init");
        eventManager.ChatUsers.Add(this);
        ChatUserSO = data;
        chatManager = manager;
        chatManager.chatUsers.Add(this);
        toggle.group = toggleGroup;

        chatManager.OnSetNextTree += SetNextTree;

        profileName.text = data.profileName;
        profileImage.sprite = data.profileImage;

        isToggled = toggle.isOn;

        if (checkForBlockedEffect())
        {
            newMatchPanel = chatManager.SpawnNewBlockedPanel(data);

            onlineIndicator.color = Color.red;
            panelRectTransform.SetAsLastSibling();
            chatManager.ReplyClicked(0);
            ChatData.ActiveDialogue = false;

            return;
        }

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
                ChatData.StayInTree = true;
                ChatData.NotSkippableDialogue = DialogueTree.DialogueTree;
                Divider = chatManager.SpawnDivider();
            }
        }
        //If ChatUser is not new
        else
        {
            ChatData = StaticUserData.ChatUserData.First(x => x.UserSO == data);
            Debug.Log("Load Chat Data: " + ChatData.name);

            LoadRGDataFromStatic(ChatData);
            if (SetBlocked(ChatData)) return;

            LoadChatData(ChatData);

            if (ChatData.CurrentDialogueIndex != 0)
            {
                DialogueTree.ForceJumpToNode(ChatData.CurrentNodeGUID, ChatData.CurrentDialogueIndex);
            }

            if (DialogueTree.DialogueTree != null)
            {
                Divider = chatManager.SpawnDivider();
            }
        }

        if (SetBlocked(ChatData)) return;

        //initialChatCollection = DialogueTree.CurrentNode.BaseNodeData.chatCollection as ChatCollectionSO;
        //currentCollection = initialChatCollection;
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

    bool checkForBlockedEffect()
    {
        string e = ("<" + ChatUserSO.profileName + "blocked>");
        return DialogueSpreadSheetPatternConstants.effects.Contains(e);
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

    void SaveRGData(ChatUserData data)
    {
        switch (data.UserSO.profileName)
        {
            case "Maeve":
                DialogueSpreadSheetPatternConstants.maeveHealth = data.RGMeter;
                break;

            case "Liam":
                DialogueSpreadSheetPatternConstants.liamHealth = data.RGMeter;
                break;

            case "Brad":
                DialogueSpreadSheetPatternConstants.bradHealth = data.RGMeter;
                break;

            case "Penelope":
                DialogueSpreadSheetPatternConstants.penelopeHealth = data.RGMeter;
                break;

            default:
                Debug.Log("User does not have a health bar");
                break;
        }
        Debug.Log("Save RGMeter: " + data.RGMeter);
    }

    void UpdateHealthBar(float meter)
    {
        float fill = meter / 100f;
        healthUI.InstantUpdateBar(fill);
    }

    void LoadChatData(ChatUserData data)
    {
        profileName.text = data.UserSO.profileName;
        profileImage.sprite = data.UserSO.profileImage;

        //Check if user is blocked
        if (SetBlocked(data)) return;

        //Set Initial Hearts
        dateProgressUI.SetHearts(data.DateProgress);

        //Spawn in Previous Chat Objects
        foreach (ChatBubble chat in data.ChatBubbles)
        {
            GameObject obj = chatManager.SpawnChatBubble(chat, this);
            chatsObj.Add(obj);

        }

        if (data.ChatBubbles.Count > 0)
            lastMessageText.text = data.ChatBubbles[data.ChatBubbles.Count - 1].chatText;

        chatManager.RebuildAfterSpawning();

        //ADD MONTH AND WEEK CHECKER TOMORROW
        //if (data.DateProgress == 2 && data.RGMeter >= 80)
        //{
        //    DialogueSpreadSheetPatternConstants.AddEffect("<ending" + data.UserSO.profileName + ">");
        //    SetDialogueContainer();
        //    return;
        //}

        //Get appropriate dialogue tree
        if (ChatUserSO.dialogueBranches == null) return;

        //Check if previous tree is skippable (only RGChats are skippable)
        if (data.StayInTree && data.NotSkippableDialogue != null /*&& data.CurrentDialogueComplete == false*/)
        {
            //DialogueSpreadSheetPatternConstants.effects.Add(data.CurrentEffect);

            DialogueTree.SetDialogueTree(data.NotSkippableDialogue);
            DialogueTree.ForceJumpToNode(data.CurrentNodeGUID, data.CurrentDialogueIndex);

            ChatData.CurrentTree = DialogueTree.DialogueTree;
            ChatData.CurrentNodeGUID = DialogueTree.CurrentNode.BaseNodeData.NodeGUID;

            ChatData.CanRGText = true;

            data.StayInTree = true;

            Debug.Log(profileName.text + " stay in tree");
            return;
        }

        //Set dialogue based on effect if not.
        DialogueContainer c = SetDialogueContainer(out data.CurrentEffect);

        if (c != null)
        {
            data.StayInTree = true;
            data.NotSkippableDialogue = c;
            return;
        }
        else
        {
            data.StayInTree = false;
            data.NotSkippableDialogue = null;
        }
         

        //if (StaticUserData.ProgressionData.CurrentWeek)
        if (data.CanRGText && data.RGMeter < 80 && data.DateProgress == 2)
        {
            OnChatComplete();
            SetRGScript(data);
            data.StayInTree = false;
            return;
        }

        return;
        //Go back to dating if RG meter is good, NOT NEEDED
        if (data.RGMeter > 49 && data.DateProgress < 2)
        {
            int date = 0;

            //Just for the effect naming convention lol
            /*
             * 0 - Before Date 1
             * 2 - Before Date 2
             */
            if (data.DateProgress == 1) date = 2;

            string effect = "<" + data.UserSO.profileName + date.ToString() + ">";
            DialogueSpreadSheetPatternConstants.AddEffect(effect);

            SetDialogueContainer(out data.CurrentEffect);
            return;
        }
    }

    void SetRGScript(ChatUserData data)
    {
        if (data.CurrentRGIndex >= data.UserSO.RGScripts.Count)
        {
            Debug.Log("Max RGs Reached!");
            return;
        }

        Debug.Log("Set RG " + data.UserSO.profileName);

        DialogueTree.SetDialogueTree(data.UserSO.RGScripts[data.CurrentRGIndex]);
        ChatData.CurrentTree = DialogueTree.DialogueTree;
        data.CurrentRGIndex++;
    }

    DialogueContainer SetDialogueContainer(out string effect)
    {
        DialogueContainer nextContainer = null;
        effect = "";
        foreach (string s in DialogueSpreadSheetPatternConstants.effects)
        {
            DialogueContainer d = ChatUserSO.dialogueBranches.GetBranch(s);
            if (d)
            {
                nextContainer = d;
                DialogueTree.SetDialogueTree(nextContainer);

                ChatData.CurrentTree = DialogueTree.DialogueTree;
                ChatData.CurrentNodeGUID = DialogueTree.CurrentNode.BaseNodeData.NodeGUID;

                Debug.Log("Removing Effect: " + s);

                effect = s;

                OnRemoveEffect?.Invoke(s);

                ChatData.CanRGText = true;

                break;
            }
        }

        return nextContainer;
    }

    public void SetCanRGText(bool b)
    {
        ChatData.CanRGText = b;
    }

    public void SetNextTree()
    {
        DialogueContainer nextContainer = null;
        foreach (string s in DialogueSpreadSheetPatternConstants.effects)
        {
            Debug.Log("NEXT TREE: " + s);
            DialogueContainer d = ChatUserSO.dialogueBranches.GetPostBranch(s);
            if (d)
            {
                nextContainer = d;

                DialogueTree.SetDialogueTree(nextContainer);
                ChatData.CurrentTree = DialogueTree.DialogueTree;
                ChatData.CurrentNodeGUID = DialogueTree.CurrentNode.BaseNodeData.NodeGUID;

                Debug.Log("Removing Effect: " + s);
                OnRemoveEffect?.Invoke(s);

                break;
            }
        }

        if (nextContainer == null)
        {
            DialogueContainer d = ChatUserSO.dialogueBranches.GetPostBranch(ChatData.BranchEffect);
            if (d)
            {
                nextContainer = d;

                DialogueTree.SetDialogueTree(nextContainer);
                ChatData.CurrentTree = DialogueTree.DialogueTree;
                ChatData.CurrentNodeGUID = DialogueTree.CurrentNode.BaseNodeData.NodeGUID;
            }
            else
            {
                Debug.Log("NO NEXT BRANCH FOUND!");
                return;
            }

        }

        profileName.text = ChatUserSO.profileName;
        profileImage.sprite = ChatUserSO.profileImage;

        ChatData.CurrentDialogueComplete = false;
        chatManager.StartSpawningChat(this, DialogueTree);
    }

    public void SetNewEventTree()
    {
        if (SetDialogueContainer(out ChatData.BranchEffect) == null)
        {
            Debug.Log("NO EVENT FOUND!");
            chatManager.ReplyClicked(0);
            return;
        }

        ChatData.WasBranchEffect = true;

        profileName.text = ChatUserSO.profileName;
        profileImage.sprite = ChatUserSO.profileImage;

        ChatData.CurrentDialogueComplete = false;
        chatManager.StartSpawningChat(this, DialogueTree);
    }

    public void DontStayOnTree()
    {
        Debug.Log("DontStayOnTree CALLED IN " + profileName.text);
        ChatData.StayInTree = false;
    }

    void OnNodeChange()
    {
        Debug.Log("OnNodeChange CALLED IN " + profileName.text);
        ChatData.CurrentDialogueIndex = 0;
        ChatData.CurrentNodeGUID = DialogueTree.CurrentNode.BaseNodeData.NodeGUID;
        ChatData.CurrentDialogueComplete = true;
    }

    private void OnDisable()
    {
        chatManager.OnSetNextTree -= SetNextTree;
        ChatData.WasBranchEffect = false;

        DialogueTree.OnNodeChanged -= OnNodeChange;

    }

    public void OnChatComplete()
    {
        Debug.Log("CHAT COMPLETE CALLED IN " + profileName.text);
        //ChatData.CurrentDialogueIndex = 0;

        //set to false since it needs to reset loading
        ChatData.CurrentDialogueComplete = false;
        ChatData.CurrentTree = null;
    }

    public void SetNotif()
    {
        notifNum++;

        notifText.text = notifNum.ToString();
        notifObj.SetActive(true);
    }

    public void SetReplyNotif()
    {
        replyNotifObj.SetActive(true);
        AudioManager.instance.AdditivePlayAudio("reply notif", true);
    }

    public void ResetReplyNotif()
    {
        replyNotifObj.SetActive(false);
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


        StaticUserData.ChatUserData.Add(ChatData);

        //Spawn in chats that were already done before.

        if (data.ChatUserData.Any(x => x.name == ChatUserSO.profileName))
        {
            ChatData = data.ChatUserData.First(x => x.name == ChatUserSO.profileName);

            ChatData.StayInTree = false;

            profileName.text = userData.profileName;
            profileImage.sprite = userData.profileImage;

            if (ChatData.ActiveDialogue)
                DialogueTree.SetDialogueTree(ChatData.CurrentTree);
            else
                DialogueTree.SetDialogueTree(null);


            healthUI.currentHealth = ChatData.RGMeter;
            UpdateHealthBar(ChatData.RGMeter);

            if (SetBlocked(ChatData)) return;

            if (ChatData.CurrentTree != null)
            {
                DialogueTree.SetDialogueTree(ChatData.CurrentTree);


                Divider = chatManager.SpawnDivider();
            }
           
            LoadChatData(ChatData);
            DialogueTree.ForceJumpToNode(ChatData.CurrentNodeGUID, ChatData.CurrentDialogueIndex);
        }

    }

    bool SetBlocked(ChatUserData data)
    {
        Debug.Log("Check block: " + data.RGMeter);
        if (data.RGMeter > 0) return false;

        newMatchPanel = chatManager.SpawnNewBlockedPanel(ChatUserSO);

        onlineIndicator.color = Color.red;
        panelRectTransform.SetAsLastSibling();
        chatManager.ReplyClicked(0);
        ChatData.ActiveDialogue = false;

        return true;
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
