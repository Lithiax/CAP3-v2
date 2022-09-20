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

    private void Start()
    {
        foreach (ChatUserSO user in UserData)
        {
            GenerateUser(user);
        }
    }

    void GenerateUser(ChatUserSO data)
    {
        GameObject UserObj = Instantiate(UserPrefab, UserParent.transform);
        ChatUser UserComp = UserObj.GetComponent<ChatUser>();

        UserComp.Init(data.initialChatCollection, data.profileName, data.profileImage, chatManager, toggleGroup);
    }
}
