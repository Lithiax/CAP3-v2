using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject PausePanel;
    [SerializeField] GameObject logSheet;

    [SerializeField] List<GameObject> panels;
    public static Action isPausingEvent;
    bool paused = false;

    private void Awake()
    {
        isPausingEvent += IsPausing;
    }

    private void OnDestroy()
    {
        isPausingEvent -= IsPausing;
    }
    private void SetInitialPanels()
    {
        foreach (GameObject p in panels)
        {
            p.SetActive(false);
        }

        logSheet.SetActive(true);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPausingEvent.Invoke();

            //paused = !paused;
            //PausePanel.SetActive(paused);
        }
    }

    void IsPausing()
    {

        paused = !paused;
        SetInitialPanels();
        Debug.Log("PAUSE MENU IS " + paused);
        PausePanel.SetActive(paused);
    }
    public void ExitButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ActivatePanel(GameObject panel)
    {
        logSheet.SetActive(false);
        foreach (GameObject p in panels)
        {
            p.SetActive(false);

            if (p == panel)
                p.SetActive(true);
        }
    }

    public void ResumeButton()
    {
        paused = false;
        PausePanel.SetActive(false);
    }
}
