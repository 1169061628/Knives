using UnityEngine;

public class ItemBase
{
    public GameObject gameObject;
    public Transform transform;

    public virtual void InitComponent() { }

    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void LateUpdate() { }
    public virtual void Reset()
    {
        transform.localScale = Vector3.one;
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    protected void Dispose()
    {
        Object.Destroy(gameObject);
        gameObject = null;
        transform = null;
    }
}
