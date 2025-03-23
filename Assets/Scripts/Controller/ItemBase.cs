using UnityEngine;

public class ItemBase
{
    protected GameObject gameObject;
    protected Transform transform;

    protected virtual void Reset()
    {
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}
