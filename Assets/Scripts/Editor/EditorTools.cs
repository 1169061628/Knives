using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class EditorTools
{
    static void ClearChild(GameObject go)
    {
        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
        EditorUtility.SetDirty(go);
        for (int i = 0; i < go.transform.childCount; i++)
        {
            ClearChild(go.transform.GetChild(i).gameObject);
        }
    }
    [MenuItem("Tools/移除失效脚本")]
    static void RemoveInProject()
    {
        var assetPaths = AssetDatabase.GetAllAssetPaths();
        foreach (string assetPath in assetPaths)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefab != null)
            {
                ClearChild(prefab);
            }
        }
        AssetDatabase.Refresh();
    }
    [MenuItem("GameFrame/记录资源路径")]
    static void RecordAssetsPath()
    {
        var files = Directory.GetFiles(Application.dataPath, "*", SearchOption.AllDirectories);
        HashSet<string> repeatPath = new();
        System.Text.StringBuilder sb = new();
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".prefab"))
            {
                var path = files[i];
                int idx = path.IndexOf("Assets");
                path = path.Remove(0, idx);
                string name = Path.GetFileNameWithoutExtension(path);
                if (repeatPath.Contains(name))
                {
                    Debug.LogError($"{name}资源重复", AssetDatabase.LoadAssetAtPath<GameObject>(path));
                    continue;
                }
                repeatPath.Add(name);
                sb.Append(Path.GetFileNameWithoutExtension(path) + "\t" + path + "\n");
                File.WriteAllText(ResManager.resTxtPath, sb.ToString());
            }
        }
        AssetDatabase.Refresh();
    }
}
