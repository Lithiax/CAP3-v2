using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
public class FileDataHandler
{
    string dataDirPath = "";

    public FileDataHandler(string dataDirPath)
    {
        this.dataDirPath = dataDirPath;
    }

    public GameData Load(string fileName)
    {
        string fullPath = Path.Combine(dataDirPath, fileName);
        GameData loadedData = null;

        if (File.Exists(fullPath))
        {
            string dataToLoad = "";
            try
            {
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                //deserialize from json to C#

                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
            }
        }

        return loadedData;
    }

    public void Save(GameData data, string fileName)
    {
        string fullPath = Path.Combine(dataDirPath, fileName);
        try
        {
            //create directory if does not exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //serialize game data to json
            string dataToStore = JsonUtility.ToJson(data, true);

            //write to file
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file:" + fullPath + "\n" + e);
        }
    }
}
