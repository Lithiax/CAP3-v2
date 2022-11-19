using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SaveManagerUI : MonoBehaviour
{
    [SerializeField] GameObject savePrefab;
    [SerializeField] GameObject gridPanel;
    [SerializeField] Image header;

    [Header("Header Properties")]
    [SerializeField] Sprite saveSprite;
    [SerializeField] Sprite loadSprite;

    [SerializeField] int saveFilesCount;

    List<SaveFileUI> fileComponents = new List<SaveFileUI>();

    private void Start()
    {
        for (int i = 0; i < saveFilesCount; i++)
        {
            GameObject g = GameObject.Instantiate(savePrefab, gridPanel.transform);

            SaveFileUI ui = g.GetComponent<SaveFileUI>();
            ui.ID = i + 1;
            ui.Init();

            fileComponents.Add(ui);
        }
    }

    public void SetUpSave()
    {
        header.sprite = saveSprite;

        foreach (SaveFileUI file in fileComponents)
        {
            file.ButtonComp.onClick.RemoveAllListeners();
            file.ButtonComp.onClick.AddListener(file.Save);
        }
    }

    public void SetUpLoad()
    {
        header.sprite = loadSprite;

        foreach (SaveFileUI file in fileComponents)
        {
            file.ButtonComp.onClick.RemoveAllListeners();
            file.ButtonComp.onClick.AddListener(file.Load);
        }
    }
}
