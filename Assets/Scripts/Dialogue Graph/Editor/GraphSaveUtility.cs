using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System;

public class GraphSaveUtility
{
    DialogueGraphView targetGraphView;
    DialogueContainer containerCache;
    List<Edge> Edges => targetGraphView.edges.ToList();
    List<DialogueNode> Nodes => targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList();

    public static GraphSaveUtility GetInstance(DialogueGraphView _targetGraphView)
    {
        return new GraphSaveUtility
        {
            targetGraphView = _targetGraphView
        };
    }

    public void SaveGraph(string fileName)
    {
        if (!Edges.Any())
            return;

        var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();

        var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
        for (int i = 0; i < connectedPorts.Length; i++)
        {
            var outputNode = connectedPorts[i].output.node as DialogueNode;
            var inputNode = connectedPorts[i].input.node as DialogueNode;

            dialogueContainer.NodeLinks.Add(new NodeLinkData
            {
                BaseNodeGuid = outputNode.GUID,
                PortName = connectedPorts[i].output.portName,
                TargetNodeGuid = inputNode.GUID
            });
        }

        foreach (var dialogueNode in Nodes.Where(node=>!node.EntryPoint))
        {
            dialogueContainer.DialogueNodeData.Add(new DialogueNodeData
            {
                NodeGUID = dialogueNode.GUID,
                chatCollection = dialogueNode.chatCollection,
                Position = dialogueNode.GetPosition().position
            });
        }

        AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Scripts/Dialogue Graph/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();
    }

    public void LoadGraph(string fileName)
    {
        containerCache = Resources.Load<DialogueContainer>(fileName);

        if (containerCache == null)
        {
            EditorUtility.DisplayDialog("File Not Found!", "Target dialogue graph file does not exist!", "OK");
            return;
        }

        ClearGraph();
        CreateNodes();
        ConnectNodes();
    }
    private void ClearGraph()
    {
        //Set entry points guid back from the save. Discard existing guid.
        Nodes.Find(x => x.EntryPoint).GUID = containerCache.NodeLinks[0].BaseNodeGuid;

        foreach (var node in Nodes)
        {
            if (node.EntryPoint) return;

            //Remove edges connected to node
            Edges.Where(x => x.input.node == node).ToList().ForEach(edge => targetGraphView.RemoveElement(edge));
            
            //Remove node
            targetGraphView.RemoveElement(node);
        }
    }
    private void CreateNodes()
    {
        foreach (var nodeData in containerCache.DialogueNodeData)
        {
            //TO CHANGE ***************************************************************
            var tempNode = targetGraphView.CreateDialogueNode(nodeData.NodeGUID);

            tempNode.GUID = nodeData.NodeGUID;
            targetGraphView.AddElement(tempNode);

            var nodePorts = containerCache.NodeLinks.Where(x => x.BaseNodeGuid == nodeData.NodeGUID).ToList();
            nodePorts.ForEach(x => targetGraphView.AddChoicePort(tempNode, x.PortName));
        }
    }

    private void ConnectNodes()
    {
        throw new NotImplementedException();
    }


}
