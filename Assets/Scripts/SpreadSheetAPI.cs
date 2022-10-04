using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System;
public class SpreadSheetAPI : MonoBehaviour
{
    //Google sheet >>> File >>> Share >>> Publish to web
    //Share >>> Restricted to Anyone can have access

    //Google Credentials https://console.cloud.google.com/apis/credentials
    //SimpleJSON JSON Parser Library Package https://drive.google.com/drive/folders/126VXfBnr-K6KCeTS4RqPJsatl_tfReiL
    //Published Google Sheet Format https://sheets.googleapis.com/v4/spreadsheets/<Spreadsheet id>/values/<Sheet name>?key=<API key>

    //New:https://sheets.googleapis.com/v4/spreadsheets/18Z4onzDE8gProsuP7TYJBkRCdYykBw9IstRoFffS_KM/values/Week1?key=AIzaSyBsDkCdfQlc4nieOTYf6eAq3xYafiCiEOM
    //new api key: AIzaSyBsDkCdfQlc4nieOTYf6eAq3xYafiCiEOM

    public static SpreadSheetAPI instance;

    [SerializeField] private string fileName;

    [SerializeField] private string sheetNameTester = "1 - Maeve - Start";
    //[SerializeField]
    //public int currentIndex;

    //[SerializeField]
    //public List<string> sheets = new List<string>();

    bool isUpdatingEditorValues = true;
    public static Action OnFinishedLoadingValues;

    [SerializeField] public string[] testerFormattedSheetRows;
    [SerializeField] private static string[] formattedSheetRows;

    public void Awake()
    {
        instance = this;
        StartCoroutine(Co_LoadValues(sheetNameTester));
    
    }

    public void Reload()
    {
        StartCoroutine(Co_LoadValues(sheetNameTester));
    }

    public static void SetCurrentIndexToSheet(string p_sheetName)
    {
        instance.StartCoroutine(instance.Co_LoadValues(p_sheetName));

    }

    public static string[] GetCurrentSheetRow(int p_desiredSheetRowCell)
    {
        return formattedSheetRows[p_desiredSheetRowCell].Split(new char[] { ',' });
    }

    public static string GetCellString(int p_desiredSheetRowCell, int p_desiredSheetCollumnCell)
    {
        string[] currentSheetRow = GetCurrentSheetRow(p_desiredSheetRowCell);
        //Debug.Log(p_desiredSheetRowCell + " GETTING " + p_desiredSheetCollumnCell);
        if (currentSheetRow.Length <= p_desiredSheetCollumnCell)
        {
            return "error";
        }
        return currentSheetRow[p_desiredSheetCollumnCell];
    }

    public static string GetRowString(int p_desiredSheetRowCell)
    {
        string[] currentSheetRow = GetCurrentSheetRow(p_desiredSheetRowCell);
        string resultText = "";

        for (int sheetCollumnIndex = 0; sheetCollumnIndex < currentSheetRow.Length - 1;)
        {
            resultText += currentSheetRow[sheetCollumnIndex];
            sheetCollumnIndex++;
            if (sheetCollumnIndex < currentSheetRow.Length - 1)
            {
                resultText += ",";
            }
        }
        return resultText;
    }


    public IEnumerator Co_LoadValues(string p_sheetName)
    {
        if (isUpdatingEditorValues)
        {
            string rawJson = "";
            UnityWebRequest www = UnityWebRequest.Get("https://sheets.googleapis.com/v4/spreadsheets/18Z4onzDE8gProsuP7TYJBkRCdYykBw9IstRoFffS_KM/values/" + p_sheetName + "?key=AIzaSyBsDkCdfQlc4nieOTYf6eAq3xYafiCiEOM");
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError ||
                www.timeout > 2)  //Computer has no internet access/cannot connect to website, opt to use json file saved
            {

                Debug.Log("CURRENTLY OFFLINE DUE TO ERROR: " + www.error);
                rawJson = JSONFileHandler.ReadFromJSON(fileName);

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
                JSONFileHandler.SaveToJSON(fileName, rawJson);
            }
            formattedSheetRows = rawJson.Split(new char[] { '\n' });
            testerFormattedSheetRows = formattedSheetRows;//tester;

        }
        else
        {

        }
        OnFinishedLoadingValues?.Invoke();

    }
}
