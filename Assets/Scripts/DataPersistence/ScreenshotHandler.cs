using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ScreenshotHandler : MonoBehaviour
{
    public static ScreenshotHandler instance { get; private set; }
    [SerializeField] GameObject PausePanel;

    public Action<Texture2D> OnTextureRendered;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one Data Persistence Manager Found!");
            return;
        }
        instance = this;
    }

    IEnumerator TakeScreenshot(int ID)
    {
        PausePanel.SetActive(false);

        yield return new WaitForEndOfFrame();
        string fileName = "FileImg" + ID.ToString() + ".png";

        string fullPath = Path.Combine(Application.persistentDataPath, fileName);

        ScreenCapture.CaptureScreenshot(fullPath);
        Texture2D screenshotTexture = ScreenCapture.CaptureScreenshotAsTexture();
        
        PausePanel.SetActive(true);
        OnTextureRendered?.Invoke(screenshotTexture);
        Debug.Log("Screenshot Taken: " + fullPath);
    }

    public void TakeScreenshot_Static(int ID)
    {
        instance.StartCoroutine(instance.TakeScreenshot(ID));
    }
}
