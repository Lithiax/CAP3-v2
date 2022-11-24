using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class NewMatchPanelUI : MonoBehaviour
{
    [SerializeField] Image profileImage;
    [SerializeField] TextMeshProUGUI matchText;
    
    public void SetUp(ChatUserSO userData)
    {
        profileImage.sprite = userData.profileImage;
        matchText.text = "You have matched with " + userData.profileName + "!";
    }
    
    public void SetBlock(ChatUserSO userData)
    {
        profileImage.sprite = userData.profileImage;
        matchText.text = userData.profileName + " has unmatched with you.";
    }
}
