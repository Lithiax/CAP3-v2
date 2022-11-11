using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSceneManager : MonoBehaviour
{
    List<string> activeSceneNames = new List<string>();
    void Awake()
    {
        for (int i = 0; i > SceneManager.sceneCount; i++)
        {
            activeSceneNames.Add(SceneManager.GetSceneAt(i).name);
        }

        if (activeSceneNames.Contains("PauseMenu"))
        {
            SceneManager.LoadSceneAsync("PauseMenu", LoadSceneMode.Additive);
        }
    }

}
