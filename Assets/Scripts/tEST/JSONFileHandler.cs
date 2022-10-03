using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;


public static class JSONFileHandler
{
    public static void SaveToJSON(string fileName, string toSave)
    {
        //Debug.Log("[S1] JSON FILE NAMED: " + fileName + "\n\n" +
        //" CAN BE FOUND IN: " + GetPath(fileName) + "\n\n" +
        //" WITH THE FOLLOWING CONTENTS: " + toSave);
        string content = JsonHelper.ToJson(toSave);
        WriteFile(GetPath(fileName), content);
    }
    public static void SaveToJSON<T>(string fileName,T toSave)
    {
        Debug.Log("[S2] JSON FILE NAMED: " + fileName + "\n\n" +
        " CAN BE FOUND IN: " + GetPath(fileName) + "\n\n" +
        " WITH THE FOLLOWING CONTENTS: " + toSave);
        string content = JsonUtility.ToJson(toSave);
        WriteFile(GetPath(fileName), content);
    }

    public static void SaveToJSON<T>(string fileName, T[] toSave)
    {
        Debug.Log("[S3] JSON FILE NAMED: " + fileName + "\n\n" +
        " CAN BE FOUND IN: " + GetPath(fileName) + "\n\n" +
        " WITH THE FOLLOWING CONTENTS: " + toSave);
        string content = JsonHelper.ToJson<T>(toSave.ToArray());

        WriteFile(GetPath(fileName), content);
    }
    public static void SaveToJSON<T>(string fileName, List<T> toSave)
    {
        Debug.Log("[S4] JSON FILE NAMED: " + fileName + "\n\n" +
        " CAN BE FOUND IN: " + GetPath(fileName) + "\n\n" +
        " WITH THE FOLLOWING CONTENTS: " + toSave);
        string content = JsonHelper.ToJson<T>(toSave.ToArray());
        WriteFile(GetPath(fileName), content);
    }
    public static string ReadFromJSON(string fileName)
    {
        string content = ReadFile(GetPath(fileName));

        if (string.IsNullOrEmpty(content) || content == "{}")
        {
            return "";
        }

        VariableValuesJson res = JsonUtility.FromJson<VariableValuesJson>(content);
        string resString = res.contents;
        return resString;
    }

    public static T ReadFromJSON<T>(string fileName)
    {
        string content = ReadFile(GetPath(fileName));

        if (string.IsNullOrEmpty(content) || content == "{}")
        {
            return default(T);
        }

        T res = JsonUtility.FromJson<T>(content);
        return res;
    }

    public static T[] ReadArrayFromJSON<T>(string fileName)
    {
        string content = ReadFile(GetPath(fileName));

        //if (string.IsNullOrEmpty(content) || content == "{}")
        //{
        //    return new T[9];
        //}

        T[] res = JsonHelper.FromJson<T>(content);
        return res;
    }

    public static List<T> ReadListFromJSON<T>(string fileName)
    {
        string content = ReadFile(GetPath(fileName));

        if (string.IsNullOrEmpty(content) || content == "{}")
        {
            return new List<T>();
        }

        List<T> res = JsonHelper.FromJson<T>(content).ToList();
        return res;
    }

    private static string GetPath(string fileName)
    {

        return Application.persistentDataPath + "/" + fileName;
    }

    private static void WriteFile(string path, string content)
    {
        
        FileStream fileStream = new FileStream(path, FileMode.Create);

        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(content);
        }
    }

    private static string ReadFile(string path)
    {
        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string content = reader.ReadToEnd();
                return content;
            }
        }
        return "";
    }
}
