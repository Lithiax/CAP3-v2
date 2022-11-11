using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ChatEventTypes
{
    DateEvent,
    BranchEvent,
    InstantDateEvent,
    RGaugeEvent
};

[System.Serializable]
public class ChatEvent
{
    public string EventData;
    public Action<ChatUserSO, string, ChatEventTypes> onEvent;
    public ChatEventTypes EventType;
    public ChatUserSO userOwner;

    public void RaiseEvent()
    {
        onEvent?.Invoke(userOwner, EventData, EventType);
    }

    public string GetResponse()
    {
        switch (EventType)
        {
            case ChatEventTypes.DateEvent:
                return "Begin Date";
            default:
                return "";
        }
    }
}
