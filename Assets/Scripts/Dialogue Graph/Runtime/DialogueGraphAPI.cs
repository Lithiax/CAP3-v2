using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DialogueTreeNode
{
    public DialogueNodeData BaseNodeData;
    public List<DialogueNodeData> ConnectedNodesData;
}

public class DialogueGraphAPI : MonoBehaviour
{
    public DialogueContainer DialogueTree { get; private set; }
    public DialogueTreeNode CurrentNode { get; private set; }


    List<DialogueTreeNode> Nodes = new List<DialogueTreeNode>();
    public bool IsFirstNode(DialogueTreeNode node) 
    { 
        return node.BaseNodeData.NodeGUID == DialogueTree.NodeLinks.First(x => x.PortName == "Next").TargetNodeGuid; 
    }

    public void SetDialogueTree(DialogueContainer tree)
    {
        if (tree == null) return;
        if (Nodes.Count > 0)
            Nodes.Clear();


        DialogueTree = tree;
        //Create Nodes
        for (int i = 0; i < DialogueTree.DialogueNodeData.Count; i++)
        {
            //Get links where base node is the node's guid
            List<NodeLinkData> connections = DialogueTree.NodeLinks.Where(x => x.BaseNodeGuid == DialogueTree.DialogueNodeData[i].NodeGUID).ToList();
            List<DialogueNodeData> connectedNodes = new List<DialogueNodeData>();
            for (int j = 0; j < connections.Count; j++)
            {
                var targetNodeGuid = connections[j].TargetNodeGuid;
                var targetNode = DialogueTree.DialogueNodeData.First(x => x.NodeGUID == targetNodeGuid);

                //Debug.Log(DialogueTree.DialogueNodeData[i].chatCollection.name +
                //    " is connected to " + targetNode.chatCollection.name);

                connectedNodes.Add(targetNode);
            }

            DialogueTreeNode node = new DialogueTreeNode
            {
                BaseNodeData = DialogueTree.DialogueNodeData[i],
                ConnectedNodesData = connectedNodes
            };

            Nodes.Add(node);
        }

        NodeLinkData nodeLink = DialogueTree.NodeLinks.First(x => x.PortName == "Next");
        CurrentNode = Nodes.First(x => x.BaseNodeData.NodeGUID == nodeLink.TargetNodeGuid);
    }

    public List<ScriptableObject> GetConnectedDialogues()
    {
        List<ScriptableObject> objects = new List<ScriptableObject>();

        foreach (DialogueNodeData node in CurrentNode.ConnectedNodesData)
        {
            objects.Add(node.chatCollection);
        }

        return objects;
    }

    public void ClearTree()
    {
        Nodes.Clear();
    }
    public ScriptableObject GetCurrentDialogueData()
    {
        return CurrentNode.BaseNodeData.chatCollection;
    }

    public void MoveToNode(ScriptableObject chatCollection)
    {
        if (CurrentNode.ConnectedNodesData.Count <= 0) return;

        //Get node chosen
        DialogueNodeData node = CurrentNode.ConnectedNodesData.First(x => x.chatCollection == chatCollection);
        //Move Tree into Node Chosen
        CurrentNode = Nodes.First(x => x.BaseNodeData == node);
    }
     
    //Used For Loading Data ONLY
    public void ForceJumpToNode(DialogueTreeNode node)
    {
        if (!Nodes.Any(x  => x == node))
        {
            Debug.LogError("Node to jump to does not exist!");
            return;
        }

        CurrentNode = Nodes.First(x => x == node);      
    }
}

