using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
public class ChatManagerUI : MonoBehaviour
{
    FindREventsManager EventManager;
    [SerializeField] Scrollbar scrollBar;
    [SerializeField] VerticalLayoutGroup chatParent;
    [SerializeField] GameObject chatPrefab;
    [HideInInspector] public List<ChatUser> chatUsers;

    [Header("Parent UI Transforms")]
    [SerializeField] RectTransform chatElements;
    [SerializeField] RectTransform replyBoxTransform;
    [SerializeField] GameObject replyButtonsParent;

    struct ReplyButton
    {
        public GameObject buttonObj;
        public TextMeshProUGUI replyButtonText;
        public Button replyButtonComp;
    }
    List<GameObject> replyButtonObjs = new List<GameObject>();
    List<ReplyButton> replyButtonData = new List<ReplyButton>();
    [SerializeField] GameObject ReplyButton1;
    [SerializeField] GameObject ReplyButton2;
    TextMeshProUGUI replyButton1Text;
    TextMeshProUGUI replyButton2Text;
    Button replyButton1Comp;
    Button replyButton2Comp;

    List<ChatBubbleUI> allChatBubbles = new List<ChatBubbleUI>();
    [HideInInspector] public List<GameObject> AllChats = new List<GameObject>();
    Vector2 oldreplyBoxTransform;
    Vector2 oldchatElementsTransform;

    private void Awake()
    {
        EventManager = GameObject.FindObjectOfType<FindREventsManager>();
        replyButtonObjs.Add(ReplyButton1);
        replyButtonObjs.Add(ReplyButton2);

        foreach (GameObject button in replyButtonObjs)
        {
            ReplyButton data = new ReplyButton
            {
                buttonObj = button,
                replyButtonText = button.GetComponentInChildren<TextMeshProUGUI>(),
                replyButtonComp = button.GetComponent<Button>()
            };

            replyButtonData.Add(data);
        }

        //replyButton1Text = ReplyButton1.GetComponentInChildren<TextMeshProUGUI>();
        //replyButton2Text = ReplyButton2.GetComponentInChildren<TextMeshProUGUI>();

        //replyButton1Comp = ReplyButton1.GetComponent<Button>();
        //replyButton2Comp = ReplyButton2.GetComponent<Button>();
    }

    public void ReplyClicked(int num)
    {
        HideResponse();
    }

    private void Start()
    {
        oldreplyBoxTransform = replyBoxTransform.offsetMax;
        oldchatElementsTransform = chatElements.offsetMin;
    }

    public void StartSpawningChat(ChatUser parent, ChatCollectionSO SelectedChats)
    {
        parent.currentChatComplete = false;
        StartCoroutine(SpawnChats(parent, SelectedChats));
    }

    void ResponseClicked(ChatUser parent, ChatCollectionSO SelectedChats)
    {
        StartSpawningChat(parent, SelectedChats);
        parent.currentCollection = SelectedChats;
        HideResponse();
    }
    void EventClicked(ChatUser parent, ChatEvent chatCollection)
    {
        chatCollection.RaiseEvent();
    }

    public void ActivateChat(List<GameObject> chats, bool con)
    { 
        foreach (GameObject chat in chats)
        {
            chat.SetActive(con);
        }

        ChatBubbleUI chatText = chats[chats.Count - 1].GetComponent<ChatBubbleUI>();
        StartCoroutine(reset(chatText));

        StartCoroutine(ScrollDown());
    }

    IEnumerator SpawnChats(ChatUser parent, ChatCollectionSO ChatCollection)
    {
        foreach (ChatBubble chat in ChatCollection.ChatBubbles)
        {
            GameObject chatObj = SpawnChatBubble(chat, parent);
            parent.OnChatSpawned(chatObj, chat.chatText);

            chatObj.SetActive(parent.isToggled);
            
            if (parent.isToggled)
            {
                parent.ResetNotif();
                ChatBubbleUI chatText = chatObj.GetComponent<ChatBubbleUI>();
                StartCoroutine(reset(chatText));

                StartCoroutine(ScrollDown());
            }
            else if (!chat.isUser)
                parent.SetNotif();

            yield return new WaitForSeconds(1f);
        }

        parent.currentChatComplete = true;

        //if event, register
        foreach (ChatEvent ev in ChatCollection.ChatEvents)
        {
            EventManager.RegisterEvent(ev);
        }
        
        if (parent.isToggled)
            HandleResponse(parent, ChatCollection);
    }   

    public GameObject SpawnChatBubble(ChatBubble data, ChatUser parent)
    {
        GameObject chatBubble = GameObject.Instantiate(chatPrefab, chatParent.transform);
        chatBubble.SetActive(false);
        ChatBubbleUI chatText = chatBubble.GetComponent<ChatBubbleUI>();
        allChatBubbles.Add(chatText);

        chatText.SetUpChat(parent, data.chatText, data.isUser);

        return chatBubble;
    }

    IEnumerator ScrollDown()
    {
        yield return new WaitForSeconds(0.1f);
        scrollBar.value = 0;
    }

    IEnumerator reset(ChatBubbleUI chatText)
    {
        yield return new WaitForSeconds(0.1f);
        chatText.andchorFitter.enabled = false;
        StartCoroutine(ScrollDown());
    }

    public void HandleResponse(ChatUser parent, ChatCollectionSO ChatCollection)
    {
        if (!parent.currentChatComplete)
        {
            HideResponse();
            return;
        }

        if (ChatCollection.isPrompt() || ChatCollection.isEvent())
        {
            //Clear button on click events
            foreach (ReplyButton bData in replyButtonData)
            {
                bData.replyButtonComp.onClick.RemoveAllListeners();
                bData.replyButtonText.text = "";
                bData.buttonObj.SetActive(false);
            }

            //Set button on click functions
            for (int i = 0; i < ChatCollection.Prompts.Count; i++)
            {
                //WTF ?????, today I learned about closure problems : )
                int copy = i;

                replyButtonData[copy].replyButtonComp.onClick.
                    AddListener(delegate { ResponseClicked(parent, ChatCollection.Prompts[copy]); });

                //replyButtonData[i].replyButtonComp.onClick.
                //    AddListener(delegate { ResponseClicked(parent, ChatCollection.Prompts[i]); });
            }

            //Add events
            for (int i = 0; i < ChatCollection.ChatEvents.Count; i++)
            {
                int copy = i;

                replyButtonData[copy].replyButtonComp.onClick.
                    AddListener(delegate { EventClicked(parent, ChatCollection.ChatEvents[copy]); });
            }

            for (int i = 0; i < ChatCollection.Prompts.Count; i++)
            {
                replyButtonData[i].replyButtonText.text = ChatCollection.Prompts[i].PromptText;
                replyButtonData[i].buttonObj.SetActive(true);
            }

            for (int i = 0; i < ChatCollection.ChatEvents.Count; i++)
            {
                replyButtonData[i].replyButtonText.text += ChatCollection.ChatEvents[i].GetResponse();
                replyButtonData[i].buttonObj.SetActive(true);
            }

            ShowResponseBox(ChatCollection.Prompts);
        }
        else
        {
            HideResponse();
        }
    }

    void ShowResponseBox(List<ChatCollectionSO> prompts)
    {
        /*Left rectTransform.offsetMin.x;
        /*Right rectTransform.offsetMax.x;
        /*Top rectTransform.offsetMax.y;
        /*Bottom rectTransform.offsetMin.y;*/

        replyBoxTransform.offsetMax = new Vector2(oldreplyBoxTransform.x, 60.255f);

        //-0.1619987
        chatElements.offsetMin = new Vector2(oldchatElementsTransform.x, 96.326f);

        StartCoroutine(ScrollDown());

        replyButtonsParent.SetActive(true);
    }

    void HideResponse()
    {
        replyButtonsParent.SetActive(false);

        replyBoxTransform.offsetMax = oldreplyBoxTransform;

        chatElements.offsetMin = oldchatElementsTransform;

        StartCoroutine(ScrollDown());
    }
}
