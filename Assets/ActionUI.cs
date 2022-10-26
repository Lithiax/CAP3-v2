using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
    [SerializeField]
    public string cueValue;
    [SerializeField] List<SO_Choice> choice;


    void Awake()
    {

    }

    public void Selected()
    {
        
    }

    public void Close()
    {
      
    }

}
