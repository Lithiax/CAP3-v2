using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class FindREventsManager : MonoBehaviour
{
    [SerializeField] GameObject BeginDatePanel;
    [SerializeField] Button BeginDateYesButton;

    [HideInInspector] public List<ChatUser> ChatUsers;
    public void RegisterEvent(ChatEvent chatEvents)
    {
        Debug.Log("Register");
        chatEvents.onEvent += HandleEvent;
    }

    void HandleEvent(ChatUserSO userData, string data, ChatEventTypes eventType)
    {
        ClearButtons();
        switch (eventType)
        {
            case ChatEventTypes.DateEvent:
                BeginDatePanel.SetActive(true);
                BeginDateYesButton.onClick.AddListener(() => BeginDate(data));
                break;
            case ChatEventTypes.BranchEvent:
                Debug.Log("Branch Event");
                ChatUser user = ChatUsers.First(x => x.ChatUserSO == userData);
                user.SetNewEventTree();
                break;
            default:
                Debug.LogError("Invalid Event");
                break;
        }
    }

    void BeginDate(string scene)
    {
        Debug.Log("Change Scene To " + scene);
    }

    public void ClearButtons()
    {
        BeginDateYesButton.onClick.RemoveAllListeners();
    }
}
