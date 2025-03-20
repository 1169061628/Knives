using UnityEngine;

public class Util
{
    public static GameObject GetGameObject(GameObject go, string name)
    {
        return go.transform.Find(name).gameObject;
    }

    public static T GetComponent<T>(GameObject go, string name) where T : Component
    {
        return GetGameObject(go, name).GetComponent<T>();
    }

    public static GameObject NewObjToParent(GameObject go, GameObject parent, string name = null)
    {
        var obj = GameObject.Instantiate(go);
        obj.transform.SetParent(parent.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        obj.SetActive(true);
        return obj;
    }
}
