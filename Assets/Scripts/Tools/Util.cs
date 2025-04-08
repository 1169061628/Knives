using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static GameObject GetGameObject(GameObject go, string name)
    {
        return go.transform.Find(name).gameObject;
    }

    public static Transform GetTransform(GameObject go, string name)
    {
        return GetGameObject(go, name).transform;
    }

    public static T GetComponent<T>(GameObject go, string name) where T : Component
    {
        return GetGameObject(go, name).GetComponent<T>();
    }

    public static GameObject NewObjToParent(GameObject go, GameObject parent, string name = null)
    {
        var obj = Object.Instantiate(go, parent.transform, true);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        obj.SetActive(true);
        return obj;
    }

    public static T GetComponentByObjectName<T>(GameObject source, string targetName) where T : Component
    {
        T[] components = source.GetComponentsInChildren<T>(true);

        foreach (T comp in components)
        {
            if (comp.gameObject.name == targetName)
            {
                return comp;
            }
        }

        Debug.LogWarning($"未找到名称包含 {targetName} 的 {typeof(T).Name} 组件");
        return null;
    }

    public static EventTriggerListener GetEventTriggerListener(GameObject go)
    {
        var trigger = go.GetComponent<EventTriggerListener>();
        if (trigger == null) trigger = go.AddComponent<EventTriggerListener>();
        return trigger;
    }

    static readonly List<List<int>> cfgData = new();
    static readonly List<(int, int, int)> dropNumData = new();

    public static List<List<int>> ReadSingleConfig(string str)
    {
        cfgData.Clear();
        var sp1 = str.Split("\r\n");
        for (int i = 1; i < sp1.Length; ++i)
        {
            List<int> data = new();
            var sp2 = sp1[i].Split(',');
            for (int j = 0; j < sp2.Length; ++j)
            {
                if (!int.TryParse(sp2[j], out var val))
                {
                    var sp3 = sp2[j].Split('_');
                    int.TryParse(sp3[0], out var d1);
                    int.TryParse(sp3[1], out var d2);
                    int.TryParse(sp3[2], out var d3);
                    dropNumData.Add((d1, d1, d3));
                }
                data.Add(val);
            }
            cfgData.Add(data);
        }
        return cfgData;
    }
    public static List<(int, int, int)> GetDropNumData() => dropNumData;
}
