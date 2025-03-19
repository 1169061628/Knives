using UnityEditor;
using UnityEngine;

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

    }
}
