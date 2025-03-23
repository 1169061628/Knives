using UnityEngine;

public class ItemBase
{
    public GameObject gameObject;
    public Transform transform;

    public virtual void Reset()
    {
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}
