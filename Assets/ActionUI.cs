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
    bool can = true;

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
        if (can)
        {
            can = false;
            StartCoroutine(cd());
            ActionUIs.onPointClickEvent.Invoke(this);
        }
     
    }

    IEnumerator cd()
    {
    
        yield return new WaitForSeconds(1f);
        can = true;
    }
}
