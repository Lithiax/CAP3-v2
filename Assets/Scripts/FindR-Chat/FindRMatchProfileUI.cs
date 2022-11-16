using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FindRMatchProfileUI : MonoBehaviour
{
    [SerializeField] Image profileImage;
    [SerializeField] TextMeshProUGUI profileName;

    public void SetUpProfile(ChatUser user)
    {
        profileName.text = user.ChatUserSO.profileName;
        profileImage.sprite = user.ChatUserSO.profileImage;

        GetComponent<Image>().sprite = user.ChatUserSO.profileMatchImage;
    }
}
