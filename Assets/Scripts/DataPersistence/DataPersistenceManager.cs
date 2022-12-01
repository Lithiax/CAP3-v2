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
        dataHandler = new FileDataHandler(Application.persistentDataPath);
        dataPersistenceObjets = FindAllDataPersistenceObj();
        NewGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame(string pfileName)
    {
        this.gameData = dataHandler.Load(pfileName);

        if (this.gameData == null)
        {
            Debug.Log("No data found!");
            return;
        }

        DialogueSpreadSheetPatternConstants.effects = gameData.GameEffects.ToList();

        if (gameData.ProgressionData != null)
        {
            StaticUserData.ProgressionData = gameData.ProgressionData;
        }

        if (gameData.EffectsUsed != null)
        {
            StaticUserData.UsedEffects = gameData.EffectsUsed;
        }

        if (gameData.CurrentSceneName == "VisualNovel")
        {
            StorylineManager.LoadVisualNovel(gameData);
        }

        if (gameData.CurrentSceneName != "")
        {
            //SceneManager.LoadSceneAsync(gameData.CurrentSceneName);
            LoadingUI.instance.InitializeLoadingScreen(gameData.CurrentSceneName);
        }

        SceneManager.sceneLoaded += GetAndLoadData;

        //Commented because it doesnt load data in the loaded scene lol
        //foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjets)
        //{
        //    dataPersistenceObject.LoadData(gameData);
        //}
    }

    void GetAndLoadData(Scene scene, LoadSceneMode mode)
    {
        dataPersistenceObjets = FindAllDataPersistenceObj();

        foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjets)
        {
            dataPersistenceObject.LoadData(gameData);
        }

        SceneManager.sceneLoaded -= GetAndLoadData;
    }

    public void SaveGame(string pfileName)
    {
        gameData.GameEffects = DialogueSpreadSheetPatternConstants.effects.ToArray();
        gameData.ProgressionData = StaticUserData.ProgressionData;
        gameData.EffectsUsed = StaticUserData.UsedEffects;

        foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjets)
        {
            dataPersistenceObject.SaveData(ref gameData);
        }

        dataHandler.Save(gameData, pfileName);
    }

    List<IDataPersistence> FindAllDataPersistenceObj()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjs = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjs);
    }
}
