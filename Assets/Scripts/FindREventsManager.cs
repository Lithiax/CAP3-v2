using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FindREventsManager : MonoBehaviour
{
    [SerializeField] GameObject BeginDatePanel;
    [SerializeField] Button BeginDateYesButton;
   
    public void RegisterEvent(ChatEvent chatEvents)
    {
        Debug.Log("Register");
        chatEvents.onEvent += HandleEvent;
    }

    void HandleEvent(string data, ChatEventTypes eventType)
    {
        ClearButtons();
        switch (eventType)
        {
            case ChatEventTypes.DateEvent:
                BeginDatePanel.SetActive(true);
                BeginDateYesButton.onClick.AddListener(() => BeginDate(data));
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
