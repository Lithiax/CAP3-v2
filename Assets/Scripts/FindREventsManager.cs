using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Linq;

public class FindREventsManager : MonoBehaviour
{
    [SerializeField] Image FadeImage;
    [SerializeField] GameObject BeginDatePanel;
    [SerializeField] Button BeginDateYesButton;

    List<ChatEvent> events = new List<ChatEvent>();
    [HideInInspector] public List<ChatUser> ChatUsers;
    public void RegisterEvent(ChatEvent chatEvents)
    {
        Debug.Log("Register");
        chatEvents.onEvent += HandleEvent;
        events.Add(chatEvents);
    }

    private void OnDisable()
    {
        foreach(ChatEvent e in events)
        {
            e.onEvent -= HandleEvent;
        }    
    }

    void HandleEvent(ChatUserSO userData, string data, ChatEventTypes eventType)
    {
        Debug.Log("Call Event");
        ClearButtons();
        switch (eventType)
        {
            case ChatEventTypes.DateEvent:
                Debug.Log("Date Event");
                BeginDatePanel.SetActive(true);
                BeginDateYesButton.onClick.AddListener(() => LoadScene(data));
                break;
            case ChatEventTypes.BranchEvent:
                Debug.Log("Branch Event");
                ChatUser user = ChatUsers.First(x => x.ChatUserSO == userData);
                user.SetNewEventTree();
                break;
            case ChatEventTypes.InstantDateEvent:
                Debug.Log("Instant Date Event");
                StartCoroutine(InstantDateEvent(data));
                break;
            case ChatEventTypes.RGaugeEvent:
                Debug.Log("RGaugeEvent");
                break;
            default:
                Debug.LogError("Invalid Event");
                break;
        }
    }

    IEnumerator InstantDateEvent(string data)
    {
        yield return new WaitForSeconds(5f);
        LoadScene(data);
    }

    void GaugeEvent(string num)
    {
        int n = int.Parse(num);
        Debug.Log("Date Gauge: " + num);
    }

    void LoadScene(string scene)
    {
        string[] subs = scene.Split(',');

        if (subs.Length > 2)
        {
            Debug.LogError("Visual Novel Parameters: '" + scene + "' is in an invalid format. Split with ,");
            return;
        }

        Debug.Log("Change Scene To " + scene);
        FadeImage.DOFade(1, 1).OnComplete(() => 
        {
            StorylineManager.LoadVisualNovel(subs[0], subs[1]);
            SceneManager.LoadScene("VisualNovel"); 
        });
    }

    public void ClearButtons()
    {
        BeginDateYesButton.onClick.RemoveAllListeners();
    }
}
