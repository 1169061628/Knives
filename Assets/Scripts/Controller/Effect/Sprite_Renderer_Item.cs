using UnityEngine;

public class Sprite_Renderer_Item : ItemBase
{
    Scene sceneMgr;
    string type;
    Material mat;
    // 1:boxFill,2:AngleFill
    int fillFlag;
    Vector2 targetSize;

    SpriteRenderer spriteRenderer, spriteRendererFill;

    const string Name_Angle = "_Angle";
    const string Name_Scale = "_Scale";

    const float boxBound = 0.087f * 2;
    const float scaleBySize = 0.78125f;
    public override void InitComponent()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRendererFill = transform.GetChild(0).GetComponent<SpriteRenderer>();
        if (spriteRendererFill.name == "Fill") fillFlag = 1;
        else if (spriteRendererFill.name == "FillFan")
        {
            fillFlag = 2;
            mat = Object.Instantiate(spriteRendererFill.material);
            spriteRendererFill.material = mat;
            spriteRenderer.material = Object.Instantiate(spriteRenderer.material);
        }
        else if (spriteRendererFill.name == "Circle") fillFlag = 0;
    }

    public void Init(Scene sceneMgr, string type)
    {
        this.sceneMgr = sceneMgr;
        this.type = type;
        transform.SetParent(sceneMgr.levelRoot.transform);
    }

    // 1:设置尺寸
    // type: 1-方形 2-扇形 3-圆形
    public void Play(int type, float angle, Vector2 targetSize, Vector3 pos, Quaternion rot)
    {
        if (type == 1)
        {
            transform.SetPositionAndRotation(pos, rot);
            spriteRenderer.size = targetSize;
            SetFill(0);
        }
        else if (type == 2)
        {
            var tmpScale = (scaleBySize * targetSize.x);
            spriteRenderer.transform.localScale = Vector3.one * tmpScale;
            spriteRenderer.material.SetFloat(Name_Angle, angle);
            mat.SetFloat(Name_Angle, angle);
            SetFill(0);
        }
        else if (type == 3)
        {
            var tmpScale = (scaleBySize * targetSize.x);
            spriteRenderer.transform.localScale = Vector3.one * tmpScale;
            SetFill(0);
        }
    }
    void SetFill(float value)
    {
        if (fillFlag == 1) spriteRendererFill.size = new Vector2(Mathf.Lerp(0, targetSize.x, value), targetSize.y);
        else spriteRendererFill.transform.localScale = Vector3.one * value;
    }

    public void PushInPool() => sceneMgr.PushEffect(type, this);
    public void Recycle() { }
    public void Dispose() => Recycle();
}