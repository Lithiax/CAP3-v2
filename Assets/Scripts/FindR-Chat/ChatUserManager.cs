using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatUserManager : MonoBehaviour
{
    [SerializeField] List<ChatUserSO> UserData;
    [SerializeField] GameObject UserParent;
    [SerializeField] GameObject UserPrefab;
    [SerializeField] ToggleGroup toggleGroup;
    [SerializeField] ChatManagerUI chatManager;

    List<ChatUser> SpawnedUsers = new List<ChatUser>();

    private void Start()
    {
        foreach (ChatUserSO user in UserData)
        {
            GenerateUser(user);
        }

        if (SpawnedUsers.Count > 0)
        {
            SpawnedUsers[0].toggle.Select();
        }
    }

    void GenerateUser(ChatUserSO data)
    {
        GameObject UserObj = Instantiate(UserPrefab, UserParent.transform);
        ChatUser UserComp = UserObj.GetComponent<ChatUser>();
        SpawnedUsers.Add(UserComp);

        UserComp.Init(data.dialogueTree, data.profileName, data.profileImage, chatManager, toggleGroup);
    }
}
