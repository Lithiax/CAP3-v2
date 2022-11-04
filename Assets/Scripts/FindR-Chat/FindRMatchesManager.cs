using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class FindRMatchesManager : MonoBehaviour
{
    [SerializeField] GameObject profileMatchPrefab;
    [SerializeField] ChatUserManager userManager;
    [SerializeField] GameObject gridLayout;
    [SerializeField] FindRMatchProfileUI matchProfile;
    List<GameObject> matchesObj; 
    // Start is called before the first frame update
    void Start()
    {
        foreach(ChatUser user in userManager.SpawnedUsers)
        {
            GameObject match = GameObject.Instantiate(profileMatchPrefab, gridLayout.transform);

            MatchProfileButtonUI ui = match.GetComponent<MatchProfileButtonUI>();
            ui.NameText.text = user.ChatUserSO.profileName;
            ui.ProfileImage.sprite = user.ChatUserSO.profileImage;

            ui.Button.onClick.AddListener(() =>
            {
                matchProfile.SetUpProfile(user);
                matchProfile.gameObject.SetActive(true);
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
