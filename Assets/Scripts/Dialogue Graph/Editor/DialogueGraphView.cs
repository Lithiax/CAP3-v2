using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor.UIElements;

public class DialogueGraphView : GraphView
{
    public Action<DialogueNode> OnNodeSelected;
    public readonly Vector2 defaultNodeSize = new Vector2(150, 200);
    public DialogueGraphView()
    {
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();
        AddElement(GenerateEntryPointNode());
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();

        ports.ForEach(port => 
        {
            if (startPort != port && startPort.node != port.node)
            {
                compatiblePorts.Add(port);
            }
        });

        return compatiblePorts;
    }

    Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }

    public void CreateNode(string nodeName, ChatCollectionSO chatCollection)
    {
        AddElement(CreateDialogueNode(nodeName, chatCollection));
    }

    public DialogueNode CreateDialogueNode(string nodeName, ChatCollectionSO chatCollectionSO)
    {
        string tempName = chatCollectionSO == null ? nodeName : chatCollectionSO.name;

        var dialogueNode = new DialogueNode
        {
            title = nodeName,
            GUID = Guid.NewGuid().ToString(),
            Name = tempName
        };

        //Create input Port
        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);

        //Create new choice buttons
        var button = new Button(() => AddChoicePort(dialogueNode));
        button.text = "New Choice";
        dialogueNode.titleContainer.Add(button);

        //Create scriptable object housing
        var objField_ScriptableObject = new ObjectField
        {
            objectType = typeof(ChatCollectionSO),
            allowSceneObjects = false,
            value = chatCollectionSO,
        };

        if (chatCollectionSO != null)
        {
            objField_ScriptableObject.value = chatCollectionSO;
            dialogueNode.chatCollection = chatCollectionSO;
        }

        //On DialogueObject Changed
        objField_ScriptableObject.RegisterValueChangedCallback(v =>
        {
            //dialogueNode.chatCollection = objField_ScriptableObject.value as ChatCollectionSO;
            dialogueNode.chatCollection = v.newValue as ChatCollectionSO;
            if (dialogueNode.chatCollection == null)
            {
                dialogueNode.title = "Dialogue Node";
            }
            else
            {
                //dialogueNode.title = objField_ScriptableObject.value.name;
                dialogueNode.title = v.newValue.name;
            }
        });

        dialogueNode.mainContainer.Add(objField_ScriptableObject);

        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();

        dialogueNode.SetPosition(new Rect(Vector2.zero, defaultNodeSize));

        return dialogueNode;
    }

    public void AddChoicePort(DialogueNode node, string overriddenPortName = "")
    {
        var generatedPort = GeneratePort(node, Direction.Output);

        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);

        var outputPortCount = node.outputContainer.Query("connector").ToList().Count;
        var choicePortName = string.IsNullOrEmpty(overriddenPortName) ? $"Choice {outputPortCount + 1}" : overriddenPortName;

        var textField = new TextField
        {
            name = string.Empty,
            value = choicePortName
        };
        textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
        generatedPort.contentContainer.Add(new Label(" "));
        generatedPort.contentContainer.Add(textField);

        var deleteButton = new Button(() => RemovePort(node, generatedPort))
        {
            text = "X"
        };
        generatedPort.contentContainer.Add(deleteButton);

        generatedPort.portName = choicePortName;

        node.outputContainer.Add(generatedPort);
        node.RefreshPorts();
        node.RefreshExpandedState();
    }

    void RemovePort(DialogueNode node, Port generatedPort)
    {
        var targetEdge = edges.ToList().Where(x => x.output.portName == generatedPort.portName
        && x.output.node == generatedPort.node);

        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }

        node.outputContainer.Remove(generatedPort);
        node.RefreshPorts();
        node.RefreshExpandedState();
    }

    DialogueNode GenerateEntryPointNode()
    {
        var node = new DialogueNode
        {
            title = "START",
            GUID = Guid.NewGuid().ToString(),
            EntryPoint = true
        };

        var generatePort = GeneratePort(node, Direction.Output);
        generatePort.portName = "Next";
        node.outputContainer.Add(generatePort);

        node.capabilities &= ~Capabilities.Movable;
        node.capabilities &= Capabilities.Deletable;

        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(100, 200, 100, 150));

        return node;
    }
}
