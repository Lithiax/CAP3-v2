using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using TMPro;

public class SaveFileUI : MonoBehaviour
{
    public Image FileImage;
    public TextMeshProUGUI FileText;
    public Button ButtonComp;
    public int ID;

    string saveFileName = "HOHSaveData.save";
    string imageFileName = "FileIMG";

    public void Init()
    {
        saveFileName = "HOHSaveData" + ID.ToString() + ".save";
        imageFileName = "FileImg" + ID.ToString() + ".png";

        //Load image

        string fullPath = Path.Combine(Application.persistentDataPath, imageFileName);

        if (File.Exists(fullPath))
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(fullPath);
                Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
                texture.LoadImage(bytes);

                SetUpImage(texture);
                FileText.text = "File " + ID.ToString();
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
            }
        }
    }

    void SetUpImage(Texture2D texture)
    {
        Sprite spr = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                                    new Vector2(0.5f, 0.5f));

        FileImage.color = Color.white;
        FileImage.sprite = spr;

        ScreenshotHandler.instance.OnTextureRendered -= SetUpImage;
    }

    public void Save()
    {
        ScreenshotHandler.instance.TakeScreenshot_Static(ID);
        ScreenshotHandler.instance.OnTextureRendered += SetUpImage;

        FileText.text = "File " + ID.ToString();
        DataPersistenceManager.instance.SaveGame(saveFileName);
    }

    public void Load()
    {
        DataPersistenceManager.instance.LoadGame(saveFileName);
    }
}
