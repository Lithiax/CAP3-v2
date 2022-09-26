using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ChoiceData
{
    public string words;
    public SO_Dialogues so_branchDialogue;

}
[CreateAssetMenu(fileName = "New Dialogues Scriptable Object", menuName = "Scriptable Objects/Dialogues")]
public class SO_Dialogues : ScriptableObject
{
    [NonReorderable] public List<Dialogue> dialogues = new List<Dialogue>();
    public List<ChoiceData> choiceDatas = new List<ChoiceData>();
}
