using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public static class ConfigUtil
{
    static readonly Dictionary<string, string> cfgTypeName2SysTypeName = new()
    {
        {"bool",  "Boolean"},
        {"char",  "Char"},
        {"float",  "Single"},
        {"double",  "Double"},
        {"sbyte",  "SByte"},
        {"short",  "Int16"},
        {"int",  "Int32"},
        {"long",  "Int64"},
        {"byte",  "Byte"},
        {"ushort",  "UInt16"},
        {"uint",  "UInt32"},
        {"ulong",  "UInt64"},
        {"string",  "String"},
    };

    public static bool IsIncludeTypeName(string name)
    {
        return cfgTypeName2SysTypeName.ContainsKey(name);
    }

    public static string GetSysTypeName(string name)
    {
        return cfgTypeName2SysTypeName[name];
    }

    public static bool NeedExport(string t)
    {
        return Convert.ToInt32(t) == (int)ExportType.Client || Convert.ToInt32(t) == (int)ExportType.BothServerAndClient;
    }
}


public enum ExportType
{
    UnExport = 0,
    BothServerAndClient = 2,
    Client = 3,
    Server = 4,
}

public class SingleExcelColumn
{
    public int index;
    public string name;
    public string typeName;
    public string defaultValue;
    public List<string> fieldValues;
}

public class ExcelSheet
{
    public string sheetName;
    public Dictionary<string, SingleExcelColumn> columnsInfo;
    public ExcelSheet() { }

    public ExcelSheet(ExcelWorksheet sheet)
    {
        var dimension = sheet.Dimension;
        int column = dimension.Columns, row = dimension.Rows;
        var cells = sheet.Cells;
        sheetName = sheet.Name;
        for (int c = 1; c <= column; c++)
        {
            if (cells[1, c].Value == null) continue;
            if (!ConfigUtil.NeedExport(cells[3, c].Value.ToString())) continue;
            columnsInfo ??= new();
            var cName = cells[1, c].Value.ToString();
            var tName = cells[2, c].Value.ToString();
            var tNameWithPair = tName.Replace("[]", "");
            if (ConfigUtil.IsIncludeTypeName(tNameWithPair))
            {
                var columnObj = new SingleExcelColumn
                {
                    index = c,
                    name = cName,
                    typeName = tName.Replace(tNameWithPair, ConfigUtil.GetSysTypeName(tNameWithPair))
                };
                var df = cells[5, c].Value;
                columnObj.defaultValue = df == null ? "" : df.ToString();
                List<string> valsList = new();
                for (int r = 6; r <= row; r++)
                {
                    if (cells[r, 2].Value == null) continue;
                    var rowValue = cells[r, c].Value;
                    valsList.Add(rowValue == null ? "" : rowValue.ToString());
                }
                columnObj.fieldValues = valsList;
                columnsInfo.Add(cName, columnObj);
            }
        }
    }
}

public interface IConfig { }
public static class ConfigManager
{
    static readonly Dictionary<Type, Dictionary<int, IConfig>> configData = new();

    public static T Get<T>(int id) where T : IConfig, new()
    {
        var tType = typeof(T);
        if (configData.ContainsKey(tType)) return (T)configData[tType][id];
        configData[tType] = ReadConfig<T>();
        return (T)configData[tType][id];
    }

    static Array ParseArrData(Type fType, string dataStr, int dimension)
    {
        if (string.IsNullOrEmpty(dataStr)) return null;
        var ttName = fType.Name.Replace("[]", "");
        var transType = Type.GetType("System." + ttName);
        Array SetArrVal(string str)
        {
            var s2 = str.Split('#');
            Array sArr = Array.CreateInstance(transType, s2.Length);
            for (int i = 0; i < s2.Length; i++)
            {
                if (!transType.IsInstanceOfType(s2[i])) sArr.SetValue(Convert.ChangeType(s2[i], transType), i);
                else sArr.SetValue(s2[i], i);
            }
            return sArr;
        }
        
        var s1 = dataStr.Split('|');
        Array arr;
        if (dimension == 1)
        {
            arr = SetArrVal(s1[0]);
        }
        else
        {
            arr = Array.CreateInstance(Type.GetType(transType.FullName + "[]"), s1.Length);
            for (int i = 0; i < s1.Length; i++)
            {
                arr.SetValue(SetArrVal(s1[i]), i);
            }
        }
        return arr;
    }

    static string GetFieldVal(string str, string defaultStr)
    {
        if (string.IsNullOrEmpty(str)) return defaultStr;
        return str;
    }

    static Dictionary<int, IConfig> ReadConfig<T>() where T : IConfig, new()
    {
        var configJson = Path.Combine(Application.persistentDataPath, "ConfigBytes", nameof(T));
        configJson += ".bytes";
        var bytes = File.ReadAllBytes(configJson);
        var obj = JsonConvert.DeserializeObject<ExcelSheet>(Encoding.UTF8.GetString(bytes));
        Dictionary<int, IConfig> data = new();

        List<string> IDVal = null;

        int bigIndex = 999999;
        foreach(var item in obj.columnsInfo)
        {
            if (item.Value.index < bigIndex)
            {
                bigIndex = item.Value.index;
                IDVal = item.Value.fieldValues;
            }
        }
        for (int i = 0; i < IDVal.Count; i++)
        {
            T t = new();
            var fields = typeof(T).GetFields();
            for (int k = 0; k < fields.Length; k++)
            {
                var field = fields[k];
                var fType = field.FieldType;
                if (fType.IsArray)
                {
                    var fieldValues = obj.columnsInfo[field.Name].fieldValues;
                    var val = GetFieldVal(fieldValues[i], obj.columnsInfo[field.Name].defaultValue);
                    Array arr = ParseArrData(fType, val, Regex.Matches(fType.Name, Regex.Escape("[]"), RegexOptions.None).Count);   // 正则的作用是匹配[]出现的次数以确定数组维度 Regex.Escape("[]")等价于"\[\]"
                    t.GetType().GetField(field.Name).SetValueDirect(__makeref(t), arr);
                }
                else
                {
                    var fieldValues = obj.columnsInfo[field.Name].fieldValues;
                    string val = GetFieldVal(fieldValues[i], obj.columnsInfo[field.Name].defaultValue);
                    t.GetType().GetField(field.Name).SetValueDirect(__makeref(t), Convert.ChangeType(val, fType));
                }
            }
            var ID = Convert.ToInt32(IDVal[i]);
            data[ID] = t;
        }
        return data;
    }
}
