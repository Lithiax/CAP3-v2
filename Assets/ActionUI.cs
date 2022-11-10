using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
[System.Serializable]
public enum CueType
{
    Voice,
    Body_Posture,
    Proxemic,
    Eye_Contact,
    Gesture,
    None
}

public class ActionUI : MonoBehaviour
{
    [SerializeField]
    public CueType cueType;


    public void EnterFunction()
    {
        ActionUIs.onEnterEvent.Invoke(this);
    }

    public void ExitFunction()
    {
        ActionUIs.onExitEvent.Invoke();
    }

    public void PointClickFunction()
    {
        ActionUIs.onPointClickEvent.Invoke(this);
    }
}
