using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class VariableValuesJson
{
    public string contents;
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        WrapperTArray<T> wrapper = JsonUtility.FromJson<WrapperTArray<T>>(json);
        return wrapper.contents;
    }


    public static string ToJson(string test)
    {
        VariableValuesJson wrapper = new VariableValuesJson();
        wrapper.contents = test;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array)
    {
        WrapperTArray<T> wrapper = new WrapperTArray<T>();
        wrapper.contents = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        WrapperTArray<T> wrapper = new WrapperTArray<T>();
        wrapper.contents = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class WrapperTArray<T>
    {
        public T[] contents;
    }

}
