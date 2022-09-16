using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Choice Scriptable Object", menuName = "Scriptable Objects/Choice")]
public class SO_Choice : ScriptableObject
{
    public string choiceName;
    public SO_Dialogues so_choiceBranchDialogue;
    
}
