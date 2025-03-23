using System;
using System.Collections.Generic;

public class KnifeObjectPool<T> where T : new()
{
    readonly Func<T> onCreate;
    readonly Stack<T> stack = new();

    public KnifeObjectPool() { onCreate = () => new T(); }
    public KnifeObjectPool(Func<T> onCreate = null)
    {
        this.onCreate = onCreate ?? (() => new T());
    }

    public T Get()
    {
        if (stack.Count > 0) return stack.Pop();
        else return onCreate();
    }

    public void Put(T obj)
    {
        stack.Push(obj);
    }
}
