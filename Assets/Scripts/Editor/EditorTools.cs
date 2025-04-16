using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

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

    [MenuItem("FastCopy/Copy Scripts")]
    // /E：拷贝所有子目录（包括空目录）

    // /COPYALL：拷贝所有文件属性

    // /R:1：失败时重试 1 次

    // /W:1：重试间隔 1 秒
    static void FastCopyScripts()
    {
        var sourcePath = Application.dataPath + "/Scripts";
        var tarPath = "F:/Knives/Assets/Scripts";

        if (Directory.Exists(tarPath)) Directory.Delete(tarPath, true);
        ProcessStartInfo psi = new()
        {
            FileName = "robocopy",
            Arguments = $"\"{sourcePath}\" \"{tarPath}\" /E /COPYALL /R:1 /W:1",
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using Process process = Process.Start(psi);
        process.WaitForExit();
    }

    static void SetSortingLayer(GameObject go)
    {
        var sps = go.transform.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < sps.Length; i++)
        {
            sps[i].sortingOrder = sps[i].gameObject.name == "Bg" ? 100 : 105;
        }
    }
    [MenuItem("Assets/批量修改地图缩放和层级")]
    static void ChangeAllScale()
    {
        var objs = Selection.gameObjects;

        //object lockObj = new();
        //string nowName = "默认";
        //int count = 0;
        //EditorUtility.DisplayProgressBar("Loading", "修改中", 0);
        //EditorApplication.update = () =>
        //{
        //    if (count < objs.Length) EditorUtility.DisplayProgressBar($"修改中{count}/{objs.Length}", nowName, (float) count / objs.Length);
        //    else
        //    {
        //        EditorApplication.update = null;
        //        EditorUtility.ClearProgressBar();
        //        AssetDatabase.Refresh();
        //        Debug.LogError("完成");
        //    }
        //};

        for (int i = 0; i < objs.Length; i++)
        {
            var go = objs[i];
            //ThreadPool.QueueUserWorkItem(_ =>
            //{
            //    lock (lockObj)
            //    {
            //nowName = go.name;
            Util.GetTransform(go, "map").localScale = Vector3.one * 50;
            SetSortingLayer(go);
            EditorUtility.SetDirty(go);
            //count++;
            //    }
            //});
        }
    }

    static void ChangeSingle(TextAsset ta, string path)
    {
        var fullPath = Path.GetFullPath(path);
        var s1 = ta.text.Split("\r\n").ToList();
        //s1.Insert(2, "int,float,int,int,int,int,int");    // Level
        // RushConfig
        //s1.Insert(1, "ID,rush_Enable,rush_Dmg,skillCD,rushCount,rush_Distance,rush_MoveSpeed,rush_attent_First,rush_interval,rush_attent,retinue_Enable,transLimit,retinueNum,retinueLevel");
        //s1.Insert(2, "int,int,int,int,int,int,int,float,int,int,int,int,int,int");
        // MiasmaConfig
        //s1.Insert(1, "ID,path_Enable,dmg,path_CD,path_ExitTime,range_Enable,range_CD,range_Range,range_Attent,range_Num,range_PerRange,range_ExitTime");
        //s1.Insert(2, "int,int,int,float,int,int,int,int,int,int,int,int");
        // FireConfig
        //s1.Insert(1, "ID,fan_Enable,fan_fireDmg,fan_SkillCD,fan_Angle,fan_FireNum,fan_FireDis,fan_Attent,fall_Enable,fall_Dmg,fall_SkillCD,fall_Attent,fall_Range,followCD");
        //s1.Insert(2, "int,int,int,int,int,int,int,int,float,int,int,int,float,int,int");
        // Role
        //s1.Insert(2, "int,int,int,int,int,int,int,int,int,int[],int,int,int,int,int,int");
        File.WriteAllText(fullPath, string.Join("\r\n", s1));
    }

    [MenuItem("Assets/写入数据类型")]
    static void ChangeAllOutlines()
    {
        var objs = Selection.GetFiltered(typeof(object), SelectionMode.Assets);
        if (objs.Length > 0)
        {
            for (int k = 0; k < objs.Length; ++k)
            {
                var path = AssetDatabase.GetAssetPath(objs[k]);
                if (!AssetDatabase.IsValidFolder(path))
                {
                    Debug.Log("选的不是文件夹，尝试进行单个转换");
                    var tx = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                    if (tx != null)
                    {
                        ChangeSingle(tx, path);
                        EditorUtility.SetDirty(tx);
                    }
                }
                else
                {
                    var ps = Directory.GetFiles(path, "*.csv", SearchOption.AllDirectories);
                    for (int i = 0; i < ps.Length; i++)
                    {
                        var tx = AssetDatabase.LoadAssetAtPath<TextAsset>(ps[i]);
                        ChangeSingle(tx, ps[i]);
                        EditorUtility.SetDirty(tx);
                    }
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
