using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using System;
using UnityEngine;

public class DialogueNode : Node
{
    public string GUID;
    public string Name;

    public ScriptableObject chatCollection;

    public bool EntryPoint = false;
}
