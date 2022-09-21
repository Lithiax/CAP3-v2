using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DialogueNode : Node
{
    public string GUID;

    public ChatCollectionSO chatCollection;

    public bool EntryPoint = false;
}
