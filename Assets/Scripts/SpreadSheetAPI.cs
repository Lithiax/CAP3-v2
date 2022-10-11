using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class SpreadSheetAPI : MonoBehaviour
{
    //Google sheet >>> File >>> Share >>> Publish to web
    //Share >>> Restricted to Anyone can have access
    //https://docs.google.com/spreadsheets/d/e/2PACX-1vR1_kBC9z3QpoGu5IG_ya1ZodxPQf63vyjtZdWTPadbo-Up8Qj6xyEM4UzQ8fpP-7SadbNU2VlJI-fV/pubhtml

    //spreadSheetID
    //1sXdUXWGEicqD-EotO6DOppzJYMrR_rNhWJszIvgSiC4

    //API Key: AIzaSyC8PPNhhTSLFqpcEUZZrGwTyl1m4e9xU-Q
    //"https://sheets.googleapis.com/v4/spreadsheets/" + spreadSheetID + "/values/" + p_sheetName + "?key=" + apiKey);

    //Google Credentials https://console.cloud.google.com/apis/credentials
    //SimpleJSON JSON Parser Library Package https://drive.google.com/drive/folders/126VXfBnr-K6KCeTS4RqPJsatl_tfReiL
    //Published Google Sheet Format https://sheets.googleapis.com/v4/spreadsheets/<Spreadsheet id>/values/<Sheet name>?key=<API key>

    //New:https://sheets.googleapis.com/v4/spreadsheets/18Z4onzDE8gProsuP7TYJBkRCdYykBw9IstRoFffS_KM/values/Week1?key=AIzaSyBsDkCdfQlc4nieOTYf6eAq3xYafiCiEOM
    //new api key: AIzaSyBsDkCdfQlc4nieOTYf6eAq3xYafiCiEOM

    public static SpreadSheetAPI instance;
    [SerializeField] List<string> spreadSheetID = new List<string>();//"1sXdUXWGEicqD-EotO6DOppzJYMrR_rNhWJszIvgSiC4";

    [SerializeField] string apiKey = "AIzaSyC8PPNhhTSLFqpcEUZZrGwTyl1m4e9xU-Q";

    [SerializeField] private string sheetNameTester = "Settings";

    public bool temp = false;
    //[SerializeField]
    //public int currentIndex;

    //[SerializeField]
    //public List<string> sheets = new List<string>();

    bool isUpdatingEditorValues = true;



    [SerializeField] public static string[] formattedSheetRows;



    public bool firstTime = true;
    public bool allLoaded = false;


    public string currentSheetName;
    List<string> sheetNames = new List<string>();
    public int currentSheetIndex;

    public static string sheetToLoad;

    public void Awake()
    {
        instance = this;
 
        StartCoroutine(Co_LoadValues(sheetNameTester));
    
    }

    public void Reload()
    {
        StartCoroutine(Co_LoadValues(sheetNameTester));
    }
    public IEnumerator Co_LoadValues(string p_sheetName)
    {
        string rawJson = "";
        if (isUpdatingEditorValues)
        {
            UnityWebRequest www = UnityWebRequest.Get("https://sheets.googleapis.com/v4/spreadsheets/18Z4onzDE8gProsuP7TYJBkRCdYykBw9IstRoFffS_KM/values/" + p_sheetName + "?key=AIzaSyBsDkCdfQlc4nieOTYf6eAq3xYafiCiEOM");
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError ||
                www.timeout > 2)  //Computer has no internet access/cannot connect to website, opt to use json file saved
            {

                Debug.Log(p_sheetName + " CURRENTLY OFFLINE DUE TO ERROR: " + www.error);

            }
            else//Computer has internet access/can connect to website, opt to update the previous json file saved with new values from web
            {
                string rawSheet = www.downloadHandler.text;


                foreach (var rawSheetRowsValuesObject in JSON.Parse(rawSheet)["values"]) //Only get values in sheet
                {
                    var rawSheetRowsObject = JSON.Parse(rawSheetRowsValuesObject.ToString());

                    List<string> rawSheetRowsStrings;
                    rawSheetRowsStrings = rawSheetRowsObject[0].AsStringList;

                    foreach (string cell in rawSheetRowsStrings) //Adding every values to a single thread of string
                    {
                        rawJson += cell + ",";
                    }
                    rawJson += "\n";

                }
                string newString = p_sheetName.Replace(" ", string.Empty);
                Debug.Log("SAVED INTO " + (newString + ".json"));
                JSONFileHandler.SaveToJSON(newString + ".json", rawJson);
          
            }
         
        

        }
        string newStrings = p_sheetName.Replace(" ", string.Empty);
        rawJson = JSONFileHandler.ReadFromJSON(newStrings + ".json");
        formattedSheetRows = rawJson.Split(new char[] { '\n' });

        if (firstTime)
        {
            firstTime = false;
            GetAllSheetNames();
        }
        if (!allLoaded && !firstTime)
        {
            LoadAllSheetNames();
            currentSheetIndex++;
        }
    }
    public static void LoadLocalFile(string p_fileName, SO_Dialogues p_sceneToLoad = null)
    {
        string rawJson = JSONFileHandler.ReadFromJSON(p_fileName);
        formattedSheetRows = rawJson.Split(new char[] { '\n' });
        if (p_sceneToLoad != null)
        {
            SpreadSheetReader.instance.ReadLocalFile(p_fileName, p_sceneToLoad);
        }

        
    }

   
    public void GetAllSheetNames()
    {
        currentSheetName = formattedSheetRows[1].Substring(0, formattedSheetRows[1].Length - 1);
        sheetNames.Clear();
        for (int i = 3; i < formattedSheetRows.Length; i++)
        {
            if (formattedSheetRows[i] != "")
            {
                if (!formattedSheetRows[i].ToLower().Contains("end sheets"))
                {

                    string newName = formattedSheetRows[i].Substring(0, formattedSheetRows[i].Length - 1);
                    //Debug.Log(formattedSheetRows[i] + " - " + newName);
                    sheetNames.Add(newName);
                }
                else if (formattedSheetRows[i].ToLower().Contains("end sheets"))
                {
                    Debug.Log("BREAKING");
                    break;
                }
            }

        }
        LoadAllSheetNames();
    }

    public void LoadAllSheetNames()
    {
        if (currentSheetIndex < sheetNames.Count)
        {
            StartCoroutine(Co_LoadValues(sheetNames[currentSheetIndex]));
          
        }
        else
        {
            Debug.Log("ALL SHEETS LOADED");
            allLoaded = true;
            sheetToLoad = sheetNames[0];
            SceneManager.LoadScene("VisualNovel");
            //LoadLocalFile(sheetNames[0]);

        }
        
   
    
        
    }

  


}
