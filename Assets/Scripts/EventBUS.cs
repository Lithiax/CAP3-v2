using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class UpdateCurrentRoomIDEvent : UnityEvent<int> { }
public class EventBUS : MonoBehaviour
{
    public static UpdateCurrentRoomIDEvent onUpdateCurrentRoomIDEvent = new UpdateCurrentRoomIDEvent();
}
