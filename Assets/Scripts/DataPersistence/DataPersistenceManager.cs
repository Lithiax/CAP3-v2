using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    GameData gameData;
    public static DataPersistenceManager instance { get; private set; }
    List<IDataPersistence> dataPersistenceObjets;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one Data Persistence Manager Found!");
            return;
        }
        instance = this;
    }

    private void Start()
    {
        dataPersistenceObjets = FindAllDataPersistenceObj();
        NewGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        if (this.gameData == null)
        {
            Debug.Log("No data found!");
            return;
        }

        if (gameData.CurrentSceneName != "")
        {
            SceneManager.LoadScene(gameData.CurrentSceneName);
        }

        foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjets)
        {
            dataPersistenceObject.LoadData(gameData);
        }

        gameData.DebugLogData();
    }

    public void SaveGame()
    {
        foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjets)
        {
            dataPersistenceObject.SaveData(ref gameData);
        }

        Debug.Log("Saved");

        gameData.DebugLogData();
    }

    List<IDataPersistence> FindAllDataPersistenceObj()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjs = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjs);
    }
}
