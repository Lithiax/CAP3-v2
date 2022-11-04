using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] string fileName;

    FileDataHandler dataHandler;

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
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        dataPersistenceObjets = FindAllDataPersistenceObj();
        NewGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        this.gameData = dataHandler.Load();

        if (this.gameData == null)
        {
            Debug.Log("No data found!");
            return;
        }

        DialogueSpreadSheetPatternConstants.effects = gameData.GameEffects.ToList();

        if (gameData.CurrentSceneName != "")
        {
            SceneManager.LoadScene(gameData.CurrentSceneName);
        }

        foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjets)
        {
            dataPersistenceObject.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        gameData.GameEffects = DialogueSpreadSheetPatternConstants.effects.ToArray();

        foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjets)
        {
            dataPersistenceObject.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);
    }

    List<IDataPersistence> FindAllDataPersistenceObj()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjs = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjs);
    }
}
