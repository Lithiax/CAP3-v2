using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ChatEventTypes
{
    DateEvent
};

[System.Serializable]
public class ChatEvent
{
    public string EventData;
    public Action<string, ChatEventTypes> onEvent;
    public ChatEventTypes EventType;

    public void RaiseEvent()
    {
        onEvent?.Invoke(EventData, EventType);
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
