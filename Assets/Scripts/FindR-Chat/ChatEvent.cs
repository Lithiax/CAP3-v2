using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ChatEventTypes
{
    DateEvent,
    BranchEvent,
    InstantDateEvent,
    RGaugeEvent,
    ChangeSceneEvent,
    AddEffectEvent
};

[System.Serializable]
public class ChatEvent
{
    public string EventData;
    public Action<ChatUserSO, string, ChatEventTypes> onEvent;
    public ChatEventTypes EventType;
    public ChatUserSO userOwner;

    public void RaiseEvent(ChatUserSO owner)
    {
        onEvent?.Invoke(owner, EventData, EventType);
    }

    public string GetResponse()
    {
        switch (EventType)
        {
            case ChatEventTypes.DateEvent:
                return "Begin Date";
            case ChatEventTypes.InstantDateEvent:
                return "Proceed to Next Week.";
            default:
                return "";
        }
    }
}
