using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class ChatManagerUI : MonoBehaviour
{
    [SerializeField] List<ChatBubbleSO> chatData = new List<ChatBubbleSO>();
    [SerializeField] Scrollbar scrollBar;
    [SerializeField] VerticalLayoutGroup chatParent;
    [SerializeField] GameObject chatPrefab;

    [Header("Parent UI Transforms")]
    [SerializeField] RectTransform chatElements;
    [SerializeField] RectTransform replyBoxTransform;
    [SerializeField] GameObject replyButtons;

    List<ChatBubbleUI> allChatBubbles = new List<ChatBubbleUI>();
    [HideInInspector] public List<GameObject> AllChats = new List<GameObject>();
    Vector2 oldreplyBoxTransform;
    Vector2 oldchatElementsTransform;

    bool chatHasPrompt = false;

    private void Start()
    {
        oldreplyBoxTransform = replyBoxTransform.offsetMax;
        oldchatElementsTransform = chatElements.offsetMin;
    }

    public void StartPlayChat(List<GameObject> SelectedChats)
    {
        if (SelectedChats.Count == 0)
            return;

        StartCoroutine(PlayChat(SelectedChats));
    }
    IEnumerator PlayChat(List<GameObject> SelectedChats)
    {
        foreach (GameObject chat in SelectedChats)
        {
            yield return new WaitForSeconds(1f);
            chat.SetActive(true);

            ChatBubbleUI chatText = chat.GetComponent<ChatBubbleUI>();
            StartCoroutine(reset(chatText));

            StartCoroutine(ScrollDown());
        }

        //ChatBubbleUI chatText = SelectedChats[SelectedChats.Count - 1].GetComponent<ChatBubbleUI>();

        //StartCoroutine(reset(chatText));


        //yield return new WaitForSeconds(1f);
        //SpawnChatBubble(chatData[0]);

        //yield return new WaitForSeconds(1f);
        //SpawnChatBubble(chatData[1]);

        //yield return new WaitForSeconds(1f);
        //GameObject chatBubble = SpawnChatBubble(chatData[2]);
    }

    public GameObject SpawnChatBubble(ChatBubbleSO data)
    {
        GameObject chatBubble = GameObject.Instantiate(chatPrefab, chatParent.transform);
        chatBubble.SetActive(false);
        ChatBubbleUI chatText = chatBubble.GetComponent<ChatBubbleUI>();
        allChatBubbles.Add(chatText);

        chatText.OnPrompt += ShowResponse;
        chatText.SetUpChat(data.chatText, data.isUser, data.isPrompt);

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

    private void OnDisable()
    {
        foreach (ChatBubbleUI chat in allChatBubbles)
        {
            chat.OnPrompt -= ShowResponse;
        }
    }

    void ShowResponse()
    {
        /*Left rectTransform.offsetMin.x;
        /*Right rectTransform.offsetMax.x;
        /*Top rectTransform.offsetMax.y;
        /*Bottom rectTransform.offsetMin.y;*/

        replyBoxTransform.offsetMax = new Vector2(oldreplyBoxTransform.x, 60.255f);

        //-0.1619987
        chatElements.offsetMin = new Vector2(oldchatElementsTransform.x, 96.326f);

        StartCoroutine(ScrollDown());

        replyButtons.SetActive(true);
    }

    public void HideResponse()
    {
        replyButtons.SetActive(false);

        replyBoxTransform.offsetMax = oldreplyBoxTransform;

        chatElements.offsetMin = oldchatElementsTransform;

        StartCoroutine(ScrollDown());
    }
}
