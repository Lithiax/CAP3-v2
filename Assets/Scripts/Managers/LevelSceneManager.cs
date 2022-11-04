using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSceneManager : MonoBehaviour
{
    void Awake()
    {
        SceneManager.LoadSceneAsync("PauseMenu", LoadSceneMode.Additive);
    }

}
