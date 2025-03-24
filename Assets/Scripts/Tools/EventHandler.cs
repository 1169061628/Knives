using System;
using System.Collections.Generic;

public class EventHandler
{
    readonly List<Action> _event;
    public EventHandler(int capacity = 4)
    {
        _event = new(capacity);
    }
    public event Action Handler
    {
        add => Add(value);
        remove => Remove(value);
    }
    public void Clear() => _event.Clear();
    public void Add(Action e) => _event.Add(e);
    public void Remove(Action e)
    {
        for (int i = _event.Count - 1; i > -1; --i)
        {
            if (_event[i] == e)
            {
                if (_r == 0)
                {
                    _event.RemoveAt(i);
                }
                else
                {
                    _event[i] = null;
                    _c++;
                }
                break;
            }
        }
    }
    int _r, _c;
    public void Send()
    {
        var count = _event.Count;
        var size = _c;
        _r++;
        for (int i = 0; i < count; ++i)
        {
            _event[i]?.Invoke();
        }
        _r--;
        if (_r == 0 && _c > 0 && _c == size && count == _event.Count)
        {
            _c = 0;
            for (int i = 0; i < count; ++i)
            {
                if (_event[i] != null)
                {
                    if (_c < i) _event[_c++] = _event[i];
                    else _c++;
                }
            }
            if (_c < count) _event.RemoveRange(_c, count - _c);
            _c = 0;
        }
    }
    //public void Once(Action action, Func<bool> predicate = null)
    //{
    //    Action once = null;
    //    once = () =>
    //    {
    //        if (predicate == null || predicate())
    //        {
    //            Remove(once);
    //            action();
    //        }
    //    };
    //    Add(once);
    //}
}

public class EventHandler<T>
{
    readonly List<Action<T>> _event;
    public T value;
    public EventHandler(int capacity = 4)
    {
        _event = new(capacity);
    }
    public event Action<T> Handler
    {
        add => Add(value);
        remove => Remove(value);
    }
    public void Clear() => _event.Clear();
    public void Add(Action<T> e) => _event.Add(e);
    public void Remove(Action<T> e)
    {
        for (int i = _event.Count - 1; i > -1; --i)
        {
            if (_event[i] == e)
            {
                if (_r == 0)
                {
                    _event.RemoveAt(i);
                }
                else
                {
                    _event[i] = null;
                    _c++;
                }
                break;
            }
        }
    }
    int _r, _c;
    public void Send(T args)
    {
        value = args;
        var count = _event.Count;
        var size = _c;
        _r++;
        for (int i = 0; i < count; ++i)
        {
            _event[i]?.Invoke(args);
        }
        _r--;
        if (_r == 0 && _c > 0 && _c == size && count == _event.Count)
        {
            _c = 0;
            for (int i = 0; i < count; ++i)
            {
                if (_event[i] != null)
                {
                    if (_c < i) _event[_c++] = _event[i];
                    else _c++;
                }
            }
            if (_c < count) _event.RemoveRange(_c, count - _c);
            _c = 0;
        }
    }
    //public void Once(Action<T> action, Func<T, bool> predicate = null)
    //{
    //    Action<T> once = null;
    //    once = arg =>
    //    {
    //        if (predicate == null || predicate(arg))
    //        {
    //            Remove(once);
    //            action(arg);
    //        }
    //    };
    //    Add(once);
    //}
}