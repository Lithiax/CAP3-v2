using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogues Scriptable Object", menuName = "Scriptable Objects/Dialogues")]
public class SO_Dialogues : ScriptableObject
{
    [NonReorderable] public List<Dialogue> dialogues = new List<Dialogue>();
    public List<SO_Choice> so_Choices = new List<SO_Choice>();
    [NonReorderable] public Dialogue nextChapterDialogue;
}
