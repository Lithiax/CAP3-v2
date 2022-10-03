using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class ChatManagerUI : MonoBehaviour
{
    FindREventsManager EventManager;
    [SerializeField] Scrollbar scrollBar;
    [SerializeField] VerticalLayoutGroup chatParent;
    [SerializeField] RectTransform chatParentTransform;
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

    public void StartSpawningChat(ChatUser parent, DialogueGraphAPI Tree)
    {
        parent.currentChatComplete = false;
        StartCoroutine(SpawnChats(parent, Tree));
    }

    void ResponseClicked(ChatUser parent, DialogueGraphAPI Tree, DialogueNodeData nodeData)
    {
        parent.DialogueTree.MoveToNode(nodeData.chatCollection);
        StartSpawningChat(parent, Tree);
        HideResponse();
    }
    void EventClicked(ChatUser parent, ChatEvent chatCollection)
    {
        chatCollection.RaiseEvent();
    }

    public void ActivateChat(List<GameObject> chats, bool con)
    {
        if (chats.Count <= 0) { Debug.Log("chat collection empty"); return; }
        foreach (GameObject chat in chats)
        {
            chat.SetActive(con);
        }

        ChatBubbleUI chatText = chats[chats.Count - 1].GetComponent<ChatBubbleUI>();
        StartCoroutine(RebuildUI());

        StartCoroutine(ScrollDown());
    }

    IEnumerator SpawnChats(ChatUser parent, DialogueGraphAPI Tree)
    {
        ChatCollectionSO ChatCollection = Tree.CurrentNode.BaseNodeData.chatCollection as ChatCollectionSO;

        foreach (ChatBubble chat in ChatCollection.ChatData)
        {
            GameObject chatObj = SpawnChatBubble(chat, parent);
            parent.OnChatSpawned(chatObj, chat.chatText);

            chatObj.SetActive(parent.isToggled);
            
            if (parent.isToggled)
            {
                parent.ResetNotif();
                ChatBubbleUI chatText = chatObj.GetComponent<ChatBubbleUI>();
                StartCoroutine(RebuildUI());

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
            HandleResponse(parent, Tree);
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

    IEnumerator RebuildUI()
    {
        yield return new WaitForSeconds(0.01f);
        LayoutRebuilder.ForceRebuildLayoutImmediate(chatParentTransform);
        //scrollAreaSizeFitter.enabled = true;
        //chatText.andchorFitter.enabled = false;
        StartCoroutine(ScrollDown());
    }

    public void HandleResponse(ChatUser parent, DialogueGraphAPI Tree)
    {
        if (!parent.currentChatComplete)
        {
            HideResponse();
            return;
        }

        ChatCollectionSO ChatCollection = Tree.CurrentNode.BaseNodeData.chatCollection as ChatCollectionSO;

        if (Tree.CurrentNode.ConnectedNodesData.Count > 0 || ChatCollection.isEvent())
        {
            //Clear button on click events
            foreach (ReplyButton bData in replyButtonData)
            {
                bData.replyButtonComp.onClick.RemoveAllListeners();
                bData.replyButtonText.text = "";
                bData.buttonObj.SetActive(false);
            }

            //Set button on click functions
            for (int i = 0; i < Tree.CurrentNode.ConnectedNodesData.Count; i++)
            {
                //WTF ?????, today I learned about closure problems : )
                int copy = i;

                replyButtonData[copy].replyButtonComp.onClick.
                    AddListener(delegate { ResponseClicked(parent, Tree, Tree.CurrentNode.ConnectedNodesData[copy]); });

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

            for (int i = 0; i < Tree.CurrentNode.ConnectedNodesData.Count; i++)
            {
                ChatCollectionSO collectionSO = Tree.CurrentNode.ConnectedNodesData[i].chatCollection as ChatCollectionSO;
                replyButtonData[i].replyButtonText.text = collectionSO.PromptText;
                replyButtonData[i].buttonObj.SetActive(true);
            }

            for (int i = 0; i < ChatCollection.ChatEvents.Count; i++)
            {
                replyButtonData[i].replyButtonText.text += ChatCollection.ChatEvents[i].GetResponse();
                replyButtonData[i].buttonObj.SetActive(true);
            }

 
            ShowResponseBox();
        }
        else
        {
            HideResponse();
        }
    }

    void ShowResponseBox()
    {
        /*Left rectTransform.offsetMin.x;
        /*Right rectTransform.offsetMax.x;
        /*Top rectTransform.offsetMax.y;
        /*Bottom rectTransform.offsetMin.y;*/

        //replyBoxTransform.offsetMax = new Vector2(oldreplyBoxTransform.x, 60.255f);

        DOVirtual.Float(oldreplyBoxTransform.y, 60.255f, 0.1f, x =>
        {
            replyBoxTransform.offsetMax = new Vector2(oldreplyBoxTransform.x, x);
        });

        //-0.1619987
        //chatElements.offsetMin = new Vector2(oldchatElementsTransform.x, 96.326f);

        DOVirtual.Float(oldchatElementsTransform.y, 96.326f, 0.1f, x =>
        {
            chatElements.offsetMin = new Vector2(oldchatElementsTransform.x, x);
        })
            .OnComplete(() =>
        {
            replyButtonsParent.SetActive(true);
            StartCoroutine(ScrollDown());
        });


        //replyButtonsParent.SetActive(true);
    }

    void HideResponse()
    {
        replyButtonsParent.SetActive(false);

        if (replyBoxTransform.offsetMax != oldreplyBoxTransform)
        {
            DOVirtual.Float(60.255f, oldreplyBoxTransform.y, 0.1f, x =>
            {
                replyBoxTransform.offsetMax = new Vector2(oldreplyBoxTransform.x, x);
            });
        }
        else
            replyBoxTransform.offsetMax = oldreplyBoxTransform;

        if (chatElements.offsetMin != oldchatElementsTransform)
        {
            DOVirtual.Float(96.326f, oldchatElementsTransform.y, 0.1f, x =>
            {
                chatElements.offsetMin = new Vector2(oldchatElementsTransform.x, x);
            })
                .OnComplete(() =>
            {
                StartCoroutine(ScrollDown());
            });
            return;
        }
        else
            chatElements.offsetMin = oldchatElementsTransform;

        StartCoroutine(ScrollDown());
    }
}
