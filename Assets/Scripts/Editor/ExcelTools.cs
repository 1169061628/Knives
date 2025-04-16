using OfficeOpenXml;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Threading;

public class ExcelTools : EditorWindow
{
    static readonly string cfgPath = Application.dataPath + "/ManagedResources/Configs";
    static readonly string definePath = Application.dataPath + "/Scripts/Config/ConfigDefine.cs";
    static readonly string bytePath = Application.dataPath + "/ManagedResources/ConfigData";
    static readonly StringBuilder sb = new();

    [MenuItem("打表/关进度", false)]
    static void CloseJinDu()
    {
        EditorApplication.update = null;
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("打表/全部表", false)]
    public static void ImportAllConfigs()
    {
        if (!File.Exists(definePath)) File.Create(definePath);
        var cfgFiles = Directory.GetFiles(cfgPath, "*.csv", SearchOption.AllDirectories);
        //string[] cfgFiles = new string[] { cfgPath + "/Level/level1.csv" };
        int totalNum = cfgFiles.Length;
        int count = 0;
        object lockObj = new();
        string nowFile = null;
        EditorUtility.DisplayProgressBar("Loading", "正在导表：", 0f);
        EditorApplication.update = () =>
        {
            if (count < totalNum) EditorUtility.DisplayProgressBar($"导表{count}/{totalNum}", nowFile, (float)count / totalNum);
            else
            {
                EditorApplication.update = null;
                EditorUtility.ClearProgressBar();
                File.WriteAllText(definePath, sb.ToString(), Encoding.UTF8);
                sb.Clear();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.LogError("完成");
                //OpenConfigJsonPath();
            }
        };

        for (int i = 0; i < totalNum; i++)
        {
            var str = cfgFiles[i];
            ThreadPool.QueueUserWorkItem(_ =>
            {
                lock (lockObj)
                {
                    nowFile = str;
                    ReadSingleExcel(str);
                    count++;
                }
            });
        }
    }


    static void ReadSingleExcel(string filePath)
    {
        var csvContent = File.ReadAllText(filePath);
        //var file = new FileInfo(filePath);
        //using ExcelPackage package = new(file);
        using var package = new ExcelPackage();
        var name = Path.GetFileNameWithoutExtension(filePath);
        var sheet = package.Workbook.Worksheets.Add(name);
        sheet.Cells["A1"].LoadFromText(csvContent, new ExcelTextFormat()
        {
            Delimiter = ',',
            EOL = "\r\n",
            DataTypes = new[] { eDataTypes.String }
        });
        //if (sheet == null) Debug.LogError(name);
        var dimension = sheet.Dimension;
        int column = dimension.Columns, row = dimension.Rows;

        sb.AppendFormat("public readonly struct {0} : IConfig\n", name);
        sb.Append("{\n");

        for (int c = 1; c <= column; c++)
        {
            var value = sheet.Cells[1, c].Value;
            if (value == null) break;
            //var readType = sheet.Cells[3, c].Value.ToString();
            //if (ConfigUtil.NeedExport(readType))
            //{
                sb.AppendFormat("\tpublic readonly {0} {1};\n", sheet.Cells[3, c].Value.ToString(), sheet.Cells[2, c].Value.ToString());
            //}
        }
        sb.Append("}\n");

        var sheetIni = new ExcelSheet(sheet);
        if (!Directory.Exists(bytePath)) Directory.CreateDirectory(bytePath);
        var jsonStr = JsonConvert.SerializeObject(sheetIni);
        string byteName = bytePath + "/" + name + ".bytes";
        var bytes = Encoding.UTF8.GetBytes(jsonStr);
        File.WriteAllBytes(byteName, bytes);
    }

    [MenuItem("打表/打开导出json数据路径", false)]
    static void OpenConfigJsonPath()
    {
        Process.Start("explorer.exe", bytePath.Replace("/", "\\"));
    }
}