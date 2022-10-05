using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class DialogueBranch
{
    public string prompt;
    public DialogueContainer DialogueTree;
    public DialogueContainer PostBranchTree;
}

[CreateAssetMenu(fileName = "New Dialogue Branches", menuName = "Dialogue Branches")]
public class DialogueBranchesSO : ScriptableObject
{
    [SerializeField] List<DialogueBranch> DialogueBranches;

    public DialogueContainer GetBranch(string prompt)
    {
        Debug.Log(prompt);
        if (!DialogueBranches.Any(x => x.prompt == prompt))
            return null;

        DialogueBranch selectedBranch = DialogueBranches.First(x => x.prompt == prompt);

        if (selectedBranch == null)
            Debug.LogError("EVENT_PROMPT " + prompt + " DOES NOT EXIST!");
       
        return selectedBranch.DialogueTree;
    }

    public DialogueContainer GetPostBranch(string prompt)
    {
        if (!DialogueBranches.Any(x => x.prompt == prompt))
            return null;

        DialogueBranch selectedBranch = DialogueBranches.First(x => x.prompt == prompt);

        if (selectedBranch == null)
            Debug.LogError("EVENT_PROMPT " + prompt + " DOES NOT EXIST!");

        return selectedBranch.PostBranchTree;
    }
}
