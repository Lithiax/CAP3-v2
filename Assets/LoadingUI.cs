using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LoadingUI : MonoBehaviour
{
    public static LoadingUI instance;

    [SerializeField] GameObject loadingScreen;
    [SerializeField] Image loadingBar;
    [SerializeField] CalendarUI calendar;

    AsyncOperation asyncOperation;

    float progress = 0f;

    bool calendarDone = false;
    bool loadingDone = false;

    string sceneToLoad;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        calendar.OnAnimationDone += CalendarDone;
    }

    void CalendarDone()
    {
        calendarDone = true;
        StartLoading();
        LoadNextScene();
    }

    private void OnDisable()
    {
        calendar.OnAnimationDone -= CalendarDone;
    }


    public void InitializeLoadingScreen(string sceneName)
    {
        calendar.gameObject.SetActive(true);
        loadingScreen.SetActive(true);
        calendar.Init();

        calendar.StartAnimation();

        sceneToLoad = sceneName;
        //asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

    }

    void StartLoading()
    {
        if (sceneToLoad == "")
        {
            Debug.Log("No Scene To Load!");
            return;
        }
        asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);
        StartCoroutine(LoadProgress());
    }

    IEnumerator LoadProgress()
    {
        while (!asyncOperation.isDone)
        {
            loadingBar.fillAmount = asyncOperation.progress;
            yield return null;
        }
        loadingBar.fillAmount = asyncOperation.progress;

        loadingDone = true;
        LoadNextScene();
    }

    void LoadNextScene()
    {
        if (calendarDone && loadingDone)
        {
            loadingScreen.SetActive(false);
        }
    }
}
