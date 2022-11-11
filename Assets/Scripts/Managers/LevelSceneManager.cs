using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LevelSceneManager : MonoBehaviour
{
    List<string> activeSceneNames = new List<string>();
    void Awake()
    {
        //Ensure theres only one pause menu
        for (int i = 0; i > SceneManager.sceneCount; i++)
        {
            activeSceneNames.Add(SceneManager.GetSceneAt(i).name);
        }

        if (activeSceneNames.Contains("PauseMenu"))
        {
            SceneManager.LoadSceneAsync("PauseMenu", LoadSceneMode.Additive);
        }

        //Ensure theres only one event system

        EventSystem[] es = FindObjectsOfType<EventSystem>();
        if (es.Length > 1)
        {
            for (int i = 1; i < es.Length; i++)
            {
                Destroy(es[i].gameObject);
            }
        }
    }

}
