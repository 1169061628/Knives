using UnityEngine;
using DG.Tweening;
public class BladeBase : ItemBase
{
    RoleBase role;  // 对应的角色
    bool isValid;   // 刀刃是否可用
    bool propFlag;  // 是否可捡起来
    BladeType bladeType;// 刀刃类型
    float height;    // 刀刃高度
    bool flyFlag;   // 是否在飞行状态
    Transform parent;   // 刀刃父物体，只有RolePlayer有用
    bool autoSpawnFlag; // 是否自动生成
    Scene sceneMgr;

    GameObject blade;
    Collider2D collider2D;
    SpriteRenderer spriteRenderer;

    Tween flyTween, resetTween, tweenScale;

    public override void InitComponent()
    {
        blade = Util.GetGameObject(gameObject, "blade");
        spriteRenderer = blade.GetComponent<SpriteRenderer>();
        collider2D = blade.GetComponent<Collider2D>();
        height = spriteRenderer.bounds.size.y * 0.5f;
    }

    public void Init(Scene scene, BladeType bladeType, RoleBase roleBase)
    {
        role = roleBase;
        //InitWithoutRole(scene, bladeType, role.isPlayer); TODO
    }

    public void InitWithoutRole(Scene scene, BladeType bladeType, bool isPlayer)
    {
        sceneMgr = scene;
        this.bladeType = bladeType;
        gameObject.layer = isPlayer ? ManyKnivesDefine.LayerID.triggerPlayer : ManyKnivesDefine.LayerID.triggerEnemy;
        collider2D.enabled = true;
        blade.transform.localPosition = Vector3.zero;
        spriteRenderer.sortingOrder = ManyKnivesDefine.SortOrder.blade_base;
        isValid = true;
        propFlag = false;
        flyFlag = false;
        parent = null;
        autoSpawnFlag = false;
        scene.OnPauseStateChange.Add(PauseListener);
    }

    void PauseListener(bool pause)
    {
        var timeScale = pause ? 0 : 1;
        if (flyTween != null) flyTween.timeScale = timeScale;
        if (resetTween != null) resetTween.timeScale = timeScale;
        if (tweenScale != null) tweenScale.timeScale = timeScale;
    }

    // 直接扔地上
    public void Drop(Vector3 pos, bool autoHide)
    {
        sceneMgr.AddBlade();
        spriteRenderer.sortingOrder = ManyKnivesDefine.SortOrder.blade_base;
        gameObject.layer = ManyKnivesDefine.LayerID.triggerEnemy;
        collider2D.enabled = true;
        propFlag = true;
        isValid = false;
        var tmpLocalPos = spriteRenderer.transform.localPosition;
        tmpLocalPos.y = -height;
        spriteRenderer.transform.localPosition = tmpLocalPos;
        transform.SetParent(sceneMgr.levelRoot.transform);
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 361));
        transform.position = pos;
        if (autoHide )
        {
            // 5秒后消失
            flyTween = DOVirtual.DelayedCall(5, () =>
            {

            });
        }
    }
}
