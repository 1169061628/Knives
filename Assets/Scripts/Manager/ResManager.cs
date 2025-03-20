using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ResManager
{
    static readonly Dictionary<string, string> resName2resPath = new();
    public static string resTxtPath = Application.dataPath + "/ManagedResources/Configs/ResRecord.txt";
    public static T LoadRes<T>(string name) where T : Object
    {
        if (resName2resPath.TryGetValue(name, out var path))
        {
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
        else
        {
            Debug.LogError($"资源名{name}没找到对应资源");
            return null;
        }
    }

    public static void InitALlResPath()
    {
        resName2resPath.Clear();
        string str = File.ReadAllText(resTxtPath);
        string[] arr1 = str.Split("\n");
        for (int i = 0;i < arr1.Length; i++)
        {
            if (!string.IsNullOrEmpty(arr1[i]))
            {
                var arr2 = arr1[i].Split("\t");
                resName2resPath[arr2[0]] = arr2[1];
            }
        }
    }
}
