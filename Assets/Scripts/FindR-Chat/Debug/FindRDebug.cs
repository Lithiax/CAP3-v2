using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class FindRDebug : MonoBehaviour
{
    private void Start()
    {
        DialogueSpreadSheetPatternConstants.effects.Remove("Drunk");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadSceneAsync("FindR", LoadSceneMode.Additive);
        }
    }
}
