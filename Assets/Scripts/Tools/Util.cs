using UnityEngine;

public class Util
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
}
