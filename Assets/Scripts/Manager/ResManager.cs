using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ResManager
{
    static readonly Dictionary<string, string> resName2resPath = new();
    public static string resTxtPath = Application.dataPath + "/ManagedResources/Configs/ResRecord.txt";
    static GameObject poolNode;
    static readonly Dictionary<string, Stack<GameObject>> loadedPrefabs = new();
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

    // 加载预设
    public static GameObject LoadPrefab(string name, Transform parent, Vector3 scale, Vector3 pos)
    {
        GameObject go;
        if (loadedPrefabs.ContainsKey(name) && loadedPrefabs[name].Count > 0) go = loadedPrefabs[name].Pop();
        else go = Object.Instantiate(LoadRes<GameObject>(name));
        go.SetActive(true);
        go.transform.SetParent(parent);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        return go;
    }
    // 回收预设
    public static void UnloadPrefab(string name, GameObject obj)
    {
        if (obj == null) return;
        if (!loadedPrefabs.TryGetValue(name, out var data))
        {
            data = new();
            loadedPrefabs[name] = data;
        }
        obj.transform.SetParent(poolNode.transform);
        obj.SetActive(false);
        data.Push(obj);

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

        poolNode = new GameObject("poolNode");
        Object.DontDestroyOnLoad(poolNode);
        poolNode.SetActive(false);
    }
}
