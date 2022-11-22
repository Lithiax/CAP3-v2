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
    public List<ChatUser> ChatUsers;
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
                ChatUser u = ChatUsers.First(x => x.ChatUserSO == userData);
                BeginDateYesButton.onClick.AddListener(() => {
                    LoadVisualNovel(data);
                    u.OnChatComplete();
                });
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
                GaugeEvent(userData, data);
                break;
            case ChatEventTypes.ChangeSceneEvent:
                StartCoroutine(ChangeSceneEvent(data));
                break;
            default:
                Debug.LogError("Invalid Event");
                break;
        }
    }

    IEnumerator InstantDateEvent(string data)
    {
        yield return new WaitForSeconds(5f);
        if (StaticUserData.ProgressionData == null)
        {
            Debug.LogError("Progression Data is Empty!");
            yield return null;
        }

        LoadVisualNovel(StaticUserData.ProgressionData.CurrentDateScene);
    }

    void GaugeEvent(ChatUserSO userData, string num)
    {
        ChatUser user = ChatUsers.First(x => x.ChatUserSO == userData);

        int n = int.Parse(num);
        user.ModifyHealth(n);

        Debug.Log("Date Gauge: " + n);
    }

    void LoadVisualNovel(string scene)
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

            LoadingUI.instance.InitializeLoadingScreen("VisualNovel");
        });
    }

    IEnumerator ChangeSceneEvent(string data)
    {
        yield return new WaitForSeconds(5f);
        LoadScene(data);
    }

    void LoadScene(string scene)
    {
        Debug.Log("Change Scene To " + scene);
        FadeImage.DOFade(1, 1).OnComplete(() =>
        {
            LoadingUI.instance.InitializeLoadingScreen(scene);
        });
    }

    public void ClearButtons()
    {
        BeginDateYesButton.onClick.RemoveAllListeners();
    }
}
