using System;
using System.Collections.Generic;
using UnityEngine;
public class KnifeObjectPool<T> where T : ItemBase, new()
{
    readonly Func<T> onCreate;
    readonly Stack<T> stack = new();
    readonly Transform parent;
    readonly string resName;

    public KnifeObjectPool() { }
    public KnifeObjectPool(Transform parent, Func<T> onCreate = null)
    {
        resName = typeof(T).Name;
        this.parent = parent;
        this.onCreate = onCreate ?? (() => new T());
    }

    public T Get()
    {
        if (stack.Count > 0) return stack.Pop();
        var t = onCreate();
        if (t.gameObject == null)
        {
            t.gameObject = ResManager.LoadPrefab(resName, parent, Vector3.one, Vector3.zero);
            t.transform = t.gameObject.transform;
        }
        t.InitComponent();
        return t;
    }

    public void Put(T obj)
    {
        ResManager.UnloadPrefab(resName, obj.gameObject);
        obj.Dispose();
        stack.Push(obj);
    }

    public void Clear()
    {
        for (int i = 0; i < stack.Count; ++i)
        {
            Put(stack.Pop());
        }
        stack.Clear();
    }
}
