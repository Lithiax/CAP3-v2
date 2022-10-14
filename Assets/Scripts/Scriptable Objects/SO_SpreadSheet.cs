using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SpreadSheet Scriptable Object", menuName = "Scriptable Objects/SpreadSheet")]
public class SO_SpreadSheet : ScriptableObject
{
    [SerializeField] public string spreadSheetID;
    [SerializeField] public List<string> sheetNames = new List<string>();
}
