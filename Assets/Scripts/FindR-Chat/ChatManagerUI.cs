using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
public class ChatManagerUI : MonoBehaviour
{
    [SerializeField] Scrollbar scrollBar;
    [SerializeField] VerticalLayoutGroup chatParent;
    [SerializeField] GameObject chatPrefab;
    [HideInInspector] public List<ChatUser> chatUsers;

    [Header("Parent UI Transforms")]
    [SerializeField] RectTransform chatElements;
    [SerializeField] RectTransform replyBoxTransform;
    [SerializeField] GameObject replyButtonsParent;
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

    bool chatHasPrompt = false;

    private void Awake()
    {
        replyButton1Text = ReplyButton1.GetComponentInChildren<TextMeshProUGUI>();
        replyButton2Text = ReplyButton2.GetComponentInChildren<TextMeshProUGUI>();

        replyButton1Comp = ReplyButton1.GetComponent<Button>();
        replyButton2Comp = ReplyButton2.GetComponent<Button>();
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
        StartCoroutine(SpawnChats(parent, SelectedChats));
    }

    public void ResponseClicked(ChatUser parent, ChatCollectionSO SelectedChats)
    {
        StartSpawningChat(parent, SelectedChats);
        HideResponse();
    }
    IEnumerator PlayChat(List<GameObject> SelectedChats)
    {
        foreach (GameObject chat in SelectedChats)
        {
            yield return new WaitForSeconds(1f);
            chat.SetActive(true);

            if (chat.activeSelf)
            {
                ChatBubbleUI chatText = chat.GetComponent<ChatBubbleUI>();
                StartCoroutine(reset(chatText));

                StartCoroutine(ScrollDown());
            }
        }
    }

    IEnumerator SpawnChats(ChatUser parent, ChatCollectionSO ChatCollection)
    {
        foreach (ChatBubble chat in ChatCollection.ChatBubbles)
        {
            yield return new WaitForSeconds(1f);
            GameObject chatObj = SpawnChatBubble(chat, parent);
            parent.OnChatSpawned(chatObj, chat.chatText);

            chatObj.SetActive(parent.isToggled);

            if (parent.isToggled)
            {
                ChatBubbleUI chatText = chatObj.GetComponent<ChatBubbleUI>();
                StartCoroutine(reset(chatText));

                StartCoroutine(ScrollDown());
            }
        }

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
        if (ChatCollection.isPrompt && parent.isToggled)
        {
            replyButton1Comp.onClick.RemoveAllListeners();
            replyButton2Comp.onClick.RemoveAllListeners();

            replyButton1Comp.onClick.AddListener(delegate { ResponseClicked(parent, ChatCollection.Prompts[0]); });
            replyButton2Comp.onClick.AddListener(delegate { ResponseClicked(parent, ChatCollection.Prompts[1]); });

            ShowResponse(ChatCollection.Prompts[0].PromptText, ChatCollection.Prompts[1].PromptText);
        }
        else
        {
            HideResponse();
        }
    }

    void ShowResponse(string firstReply, string secondReply)
    {
        /*Left rectTransform.offsetMin.x;
        /*Right rectTransform.offsetMax.x;
        /*Top rectTransform.offsetMax.y;
        /*Bottom rectTransform.offsetMin.y;*/

        replyButton1Text.text = firstReply;
        replyButton2Text.text = secondReply;

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
