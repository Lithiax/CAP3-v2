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
    [SerializeField] GameObject dividerPrefab;
    [SerializeField] GameObject newMatchPanelPrefab;
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
    [SerializeField] GameObject ReplyButton3;
    [SerializeField] GameObject ReplyText;
    TextMeshProUGUI replyButton1Text;
    TextMeshProUGUI replyButton2Text;
    Button replyButton1Comp;
    Button replyButton2Comp;

    List<ChatBubbleUI> allChatBubbles = new List<ChatBubbleUI>();
    [HideInInspector] public List<GameObject> AllChats = new List<GameObject>();
    Vector2 oldreplyBoxTransform;
    Vector2 oldchatElementsTransform;

    public Action OnCurrentNodeEnded;

    private void Awake()
    {
        InitializeTransforms();

        EventManager = GameObject.FindObjectOfType<FindREventsManager>();
        replyButtonObjs.Add(ReplyButton1);
        replyButtonObjs.Add(ReplyButton2);
        replyButtonObjs.Add(ReplyButton3);

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

    public void InitializeTransforms()
    {
        oldreplyBoxTransform = replyBoxTransform.offsetMax;
        oldchatElementsTransform = chatElements.offsetMin;
    }

    public void ReplyClicked(int num)
    {
        HideResponse();
    }

    public GameObject SpawnNewMatchPanel(ChatUserSO userData)
    {
        GameObject g =  GameObject.Instantiate(newMatchPanelPrefab.gameObject, chatParentTransform);
        g.GetComponent<NewMatchPanelUI>().SetUp(userData);
        return g;
    }

    public GameObject SpawnDivider()
    {
        return GameObject.Instantiate(dividerPrefab, chatParentTransform);
    }

    public void StartSpawningChat(ChatUser parent, DialogueGraphAPI Tree)
    {
        parent.currentChatComplete = false;

        Debug.Log("StartSpawning" + this);
        StartCoroutine(SpawnChats(parent, Tree));
    }

    void ResponseClicked(ChatUser parent, DialogueGraphAPI Tree, DialogueNodeData nodeData)
    {
        parent.DialogueTree.MoveToNode(nodeData.chatCollection);

        Debug.Log("Clicked" + this);

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

        StartCoroutine(ScrollDown(true));
    }

    //Dont use for now
    void OneResponseButton(ChatBubble currChat, ChatUser parent)
    {
        if (!parent.isToggled)
            return;

        foreach (ReplyButton bData in replyButtonData)
        {
            bData.replyButtonComp.onClick.RemoveAllListeners();
            bData.replyButtonText.text = "";
            bData.buttonObj.SetActive(false);
        }


        replyButtonData[0].replyButtonText.text = currChat.chatText;
        replyButtonData[0].buttonObj.SetActive(true);
        replyButtonData[0].replyButtonComp.onClick.AddListener(() => { 
            HideResponse(); 
            parent.SingleResponseChat = null;
        });

        ShowResponseBox();
    }

    float SetWaitTime(float length)
    {
        int rand = UnityEngine.Random.Range(-30, 30);
        float CPM = 1050f + rand;

        return ((length / CPM) * 60);
    }

    //SO BAD BUT ITS 5AM IDK
    bool CheckIsUser(bool isFirstChat, bool isFirstNode)
    {
        if (isFirstNode && isFirstChat) return true;
        if (!isFirstChat) return true;

        return false;
    }

    //NOTE: This is only used for NEW chats
    IEnumerator SpawnChats(ChatUser parent, DialogueGraphAPI Tree)
    {
        ChatCollectionSO ChatCollection = Tree.CurrentNode.BaseNodeData.chatCollection as ChatCollectionSO;

        foreach (ChatBubble chat in ChatCollection.ChatData)
        {
            //chat != ChatCollection.ChatData[0] so it doesnt trigger twice. 
            //Check if tree is not the first node

            Debug.Log("Check User: " + CheckIsUser(chat == ChatCollection.ChatData[0], Tree.IsFirstNode(Tree.CurrentNode)));
            Debug.Log("Is First Node: " + Tree.IsFirstNode(Tree.CurrentNode));

            if (chat.isUser &&
                CheckIsUser(chat == ChatCollection.ChatData[0], Tree.IsFirstNode(Tree.CurrentNode)))
            {
                parent.SingleResponseChat = chat;
                OneResponseButton(chat, parent);
                while (parent.SingleResponseChat != null)
                    yield return null;
            }
            else
            {
                parent.SingleResponseChat = null;
            }

            float waitTime = SetWaitTime(chat.chatText.Length);
            
            GameObject chatObj = SpawnChatBubble(chat, parent, !chat.isUser);
            chatObj.SetActive(parent.isToggled);
            parent.OnChatSpawned(chat, chatObj);

            if (parent.isToggled)
            {
                parent.ResetNotif();
                StartCoroutine(RebuildUI());

                StartCoroutine(ScrollDown());
            }


            if (chat.isUser)
            {
                waitTime = 1f;
                yield return new WaitForSeconds(waitTime);
                continue;
            }
            Debug.Log(waitTime);
            yield return new WaitForSeconds(waitTime);

            parent.SetLastMessageText(chat.chatText);

            if (parent.isToggled)
            {
                parent.ResetNotif();
                StartCoroutine(RebuildUI());

                StartCoroutine(ScrollDown());
            }
            else if (!chat.isUser)
                parent.SetNotif();

            ChatBubbleUI chatText = chatObj.GetComponent<ChatBubbleUI>();
            chatText.ShowText();
        }

        parent.currentChatComplete = true;

        //if event, register
        foreach (ChatEvent ev in ChatCollection.ChatEvents)
        {
            EventManager.RegisterEvent(ev);
            if (ev.EventType != ChatEventTypes.DateEvent)
            {
                ev.RaiseEvent();
            }
        }

        if (parent.isToggled)
        {
            HandleResponse(parent, Tree);
        }
    }   

    public void RebuildAfterSpawning()
    {
        StartCoroutine(RebuildUI());

        StartCoroutine(ScrollDown(true));
    }

    public GameObject SpawnChatBubble(ChatBubble data, ChatUser parent, bool isNew = false)
    {
        GameObject chatBubble = GameObject.Instantiate(chatPrefab, chatParent.transform);
        chatBubble.SetActive(false);
        ChatBubbleUI chatText = chatBubble.GetComponent<ChatBubbleUI>();
        allChatBubbles.Add(chatText);

        chatText.SetUpChat(parent, data, isNew);

        return chatBubble;
    }

    public GameObject SpawnChatObjects(GameObject chat, bool toggle)
    {
        GameObject chatBubble = GameObject.Instantiate(chat, chatParent.transform);
        chatBubble.SetActive(toggle);

        return chatBubble;
    }

    IEnumerator ScrollDown(bool instant = false)
    {
        yield return new WaitForSeconds(0.1f);

        Debug.Log("Scroll Instant: " + instant);

        if (instant)
        {
            scrollBar.value = 0;
            yield break;
        }

        DOVirtual.Float(scrollBar.value, 0, 0.1f, x =>
        {
            scrollBar.value = x;
        });
    }

    IEnumerator RebuildUI()
    {
        yield return new WaitForSeconds(0.01f);
        LayoutRebuilder.ForceRebuildLayoutImmediate(chatParentTransform);
        //scrollAreaSizeFitter.enabled = true;
        //chatText.andchorFitter.enabled = false;
    }

    public void HandleResponse(ChatUser parent, DialogueGraphAPI Tree)
    {
        if (Tree.DialogueTree == null)
        {
            HideResponse(true);
            return;
        }
        if (parent.SingleResponseChat != null)
        {
            Debug.Log("Single Response");
            OneResponseButton(parent.SingleResponseChat, parent);
            ShowResponseBox();
            return;
        }
        if (!parent.currentChatComplete)
        {
            Debug.Log("Chat Not Complete");
            HideResponse();
            return;
        }

        ChatCollectionSO ChatCollection = Tree.CurrentNode.BaseNodeData.chatCollection as ChatCollectionSO;



        //If dialogue tree is over
        //NOTE: There is a minor bug where it still shows up but it works rn so whatever pin it for next time.
        if (Tree.CurrentNode.ConnectedNodesData.Count <= 0 && !ChatCollection.isEvent())
        {
            HideResponse();
            OnCurrentNodeEnded?.Invoke();
            Debug.Log("Choice END");
            return;
        }

        if( Tree.CurrentNode.ConnectedNodesData.Count == 1)
        {
            ChatCollectionSO coll = Tree.CurrentNode.ConnectedNodesData[0].chatCollection as ChatCollectionSO;
            if (String.IsNullOrEmpty(coll.PromptText))
            {
                Debug.Log("Skip");
                ResponseClicked(parent, Tree, Tree.CurrentNode.ConnectedNodesData[0]);
                return;
            }
        }

        if (Tree.CurrentNode.ConnectedNodesData.Count > 0 || ChatCollection.isEvent())
        {

            if (ChatCollection.isEvent())
            {
                if (ChatCollection.ChatEvents[0].EventType != ChatEventTypes.DateEvent)
                {
                    HideResponse();
                    return;
                }
            }
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

        ReplyText.SetActive(false);

        DOVirtual.Float(oldreplyBoxTransform.y, 60.255f, 0.1f, x =>
        {
            replyBoxTransform.offsetMax = new Vector2(oldreplyBoxTransform.x, x);
        });

        //-0.1619987
        //chatElements.offsetMin = new Vector2(oldchatElementsTransform.x, 96.326f);

        DOVirtual.Float(oldchatElementsTransform.y, 138.357f, 0.1f, x =>
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

    void HideResponse(bool instant = false)
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
            DOVirtual.Float(138.357f, oldchatElementsTransform.y, 0.1f, x =>
            {
                chatElements.offsetMin = new Vector2(oldchatElementsTransform.x, x);
            })
                .OnComplete(() =>
            {
                //StartCoroutine(ScrollDown());
                ReplyText.SetActive(true);
            });
            return;
        }
        else
            chatElements.offsetMin = oldchatElementsTransform;

        StartCoroutine(ScrollDown(true));
    }
}
