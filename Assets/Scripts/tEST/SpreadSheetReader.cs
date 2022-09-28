using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System;


public class SpreadSheetReader : MonoBehaviour
{
    //Old:https://sheets.googleapis.com/v4/spreadsheets/1ecfrlT5Rol7vyjEcBhwbuZypl0Z8pp3LFMnK6eFXvUg/values/Sheet1?key=AIzaSyC8PPNhhTSLFqpcEUZZrGwTyl1m4e9xU-Q
    //New:https://sheets.googleapis.com/v4/spreadsheets/18Z4onzDE8gProsuP7TYJBkRCdYykBw9IstRoFffS_KM/values/Sheet1?key=AIzaSyBsDkCdfQlc4nieOTYf6eAq3xYafiCiEOM
    //new api key: AIzaSyBsDkCdfQlc4nieOTYf6eAq3xYafiCiEOM
    //Google sheet >>> File >>> Share >>> Publish to web
    //Share >>> Restricted to Anyone can have access
    //Google Credentials https://console.cloud.google.com/apis/credentials
    //SimpleJSON JSON Parser Library Package https://drive.google.com/drive/folders/126VXfBnr-K6KCeTS4RqPJsatl_tfReiL
    //Published Google Sheet Format https://sheets.googleapis.com/v4/spreadsheets/<Spreadsheet id>/values/<Sheet name>?key=<API key>
    [SerializeField] private string fileName;


    bool isUpdatingEditorValues = true;
    public static Action OnFinishedLoadingValues;

    [SerializeField] private static string[] formattedSheetRows;

  
    public void Awake()
    {
        StartCoroutine(Co_LoadValues());
    }

    public static string[] GetCurrentSheetRow(int p_desiredSheetRowCell)
    {
        return formattedSheetRows[p_desiredSheetRowCell].Split(new char[] { ',' });
    }

    public static string GetCellString(int p_desiredSheetRowCell, int p_desiredSheetCollumnCell)
    {
        string[] currentSheetRow = GetCurrentSheetRow(p_desiredSheetRowCell);

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


    public IEnumerator Co_LoadValues()
    {
        if (isUpdatingEditorValues)
        {
            string rawJson = "";
            UnityWebRequest www = UnityWebRequest.Get("https://sheets.googleapis.com/v4/spreadsheets/18Z4onzDE8gProsuP7TYJBkRCdYykBw9IstRoFffS_KM/values/Sheet1?key=AIzaSyBsDkCdfQlc4nieOTYf6eAq3xYafiCiEOM");
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

        }
        else
        {
            
        }
        OnFinishedLoadingValues.Invoke();
       
    }


}


