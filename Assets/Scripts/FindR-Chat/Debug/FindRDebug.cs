using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class FindRDebug : MonoBehaviour
{
    [SerializeField] ChatUserSO test;
    private void Start()
    {
        DialogueSpreadSheetPatternConstants.effects.Remove("Drunk");
        StaticUserData.UserSOs.Add(test);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadSceneAsync("FindR", LoadSceneMode.Additive);
        }
    }
}
