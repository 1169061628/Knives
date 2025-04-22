using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
public class BladeBase : ItemBase
{
    public RoleBase roleBase;  // 对应的角色
    public bool valid;   // 刀刃是否可用
    bool propFlag;  // 是否可捡起来
    int bladeType;// 刀刃类型
    float height;    // 刀刃高度
    bool flyFlag;   // 是否在飞行状态
    BladeParent parent;   // 刀刃父物体，只有RolePlayer有用
    public bool autoSpawnFlag; // 是否自动生成
    GameScene sceneMgr;
    Transform bladeTran;

    GameObject blade;
    Collider2D collider2D;
    SpriteRenderer spriteRenderer;

    Sequence flyTween, resetTween, tweenScale;

    public override void InitComponent()
    {
        blade = Util.GetGameObject(gameObject, "blade");
        spriteRenderer = blade.GetComponent<SpriteRenderer>();
        collider2D = blade.GetComponent<Collider2D>();
        height = spriteRenderer.bounds.size.y * 0.5f;
    }

    public virtual void Init(GameScene scene, int bladeType, RoleBase roleBase)
    {
        this.roleBase = roleBase;
        InitWithoutRole(scene, bladeType, roleBase.isPlayer);
    }

    public void InitWithoutRole(GameScene scene, int bladeType, bool isPlayer)
    {
        sceneMgr = scene;
        this.bladeType = bladeType;
        gameObject.layer = isPlayer ? ManyKnivesDefine.LayerID.triggerPlayer : ManyKnivesDefine.LayerID.triggerEnemy;
        collider2D.enabled = true;
        blade.transform.localPosition = Vector3.zero;
        spriteRenderer.sortingOrder = ManyKnivesDefine.SortOrder.blade_base;
        valid = true;
        propFlag = false;
        flyFlag = false;
        parent = null;
        autoSpawnFlag = false;
        scene.pauseBind.Add(PauseListener);
    }

    void RefreshPause()
    {
        PauseListener(sceneMgr.pauseBind.value);
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
        valid = false;
        var tmpLocalPos = spriteRenderer.transform.localPosition;
        tmpLocalPos.y = -height;
        spriteRenderer.transform.localPosition = tmpLocalPos;
        transform.SetParent(sceneMgr.levelRoot.transform);
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 361));
        transform.position = pos;
        if (autoHide)
        {
            // 5秒后消失
            flyTween = DOTween.Sequence();
            flyTween.Append(DOVirtual.DelayedCall(5, () =>
            {
                sceneMgr.ReduceBlade();
                PushInPool();
            }));
            RefreshPause();
        }
        else autoSpawnFlag = true;
    }

    public void SetParentIndex(BladeParent parent, float scaleMulti)
    {
        this.parent = parent;
        transform.SetParent(parent.transform);
        transform.localScale = Vector3.one * scaleMulti;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void PushInPool()
    {
        valid = false;
        propFlag = false;
        collider2D.enabled = false;
        sceneMgr.BladePoolPushOne(bladeType, this);
    }

    public void Dispose()
    {
        Recycle();
    }

    void Recycle()
    {
        KillFlyTween();
        KillScaleTween();
        KillResetTween();
        sceneMgr.pauseBind.Remove(PauseListener);
    }

    void KillFlyTween()
    {
        flyTween?.Kill();
        flyTween = null;
    }
    void KillResetTween()
    {
        resetTween?.Kill();
        resetTween = null;
    }
    void KillScaleTween()
    {
        tweenScale?.Kill();
        tweenScale = null;
    }

    public void SetScale(float scaleMulti)
    {
        KillScaleTween();
        transform.localScale = Vector3.one * scaleMulti;
    }

    int flyMoveSp = 35;
    void FlyInRole(RoleBase roleBase, BladeParent parent, float fromDeg, float tarDeg, float scaleMulti)
    {
        this.roleBase = roleBase;
        flyFlag = true;
        this.parent = parent;
        collider2D.enabled = false;
        gameObject.layer = roleBase.isPlayer ? ManyKnivesDefine.LayerID.triggerPlayer : ManyKnivesDefine.LayerID.triggerEnemy;
        KillFlyTween();
        flyTween = DOTween.Sequence();
        var tmpLocalPos = spriteRenderer.transform.localPosition;
        tmpLocalPos.z = 0;
        spriteRenderer.transform.localPosition = tmpLocalPos;
        tmpLocalPos = transform.localPosition;
        tmpLocalPos += tmpLocalPos.normalized * -height;
        transform.localPosition = tmpLocalPos;
        transform.SetParent(parent.transform);
        tmpLocalPos = transform.localPosition;
        spriteRenderer.sortingOrder = ManyKnivesDefine.SortOrder.blade_fly;
        List<Vector3> pathList = new();
        float dis = 0;
        var range = scaleMulti * (ManyKnivesDefine.playerCollectRange + 1.5f);
        var tmpI = 0;
        var perDeg = Mathf.Min(10, Mathf.Abs(tarDeg - fromDeg));
        tarDeg -= 360;
        while(fromDeg > tarDeg)
        {
            pathList[tmpI] = new Vector3(range * Mathf.Cos(fromDeg * Mathf.Deg2Rad), range * Mathf.Sin(fromDeg * Mathf.Deg2Rad));
            pathList[tmpI] = parent.transform.InverseTransformPoint(roleBase.bladeTran.TransformPoint(pathList[tmpI]));
            if (tmpI > 0) dis += Vector3.Distance(pathList[tmpI - 1], pathList[tmpI]);
            tmpI++;
            fromDeg -= perDeg;
        }
        pathList[tmpI] = Vector3.zero;
        dis += Vector3.Distance(pathList[0], tmpLocalPos);
        var lastDis = Vector3.Distance(pathList[tmpI - 1], pathList[tmpI]);
        dis += lastDis;
        var dur = dis / flyMoveSp;
        flyTween.Insert(0, transform.DOLocalPath(pathList.ToArray(), dur, PathType.CatmullRom).SetEase(Ease.Linear));
        var recoverTime = lastDis / flyMoveSp;
        flyTween.Insert(0, DOVirtual.Float(0, 1, dur - recoverTime, val =>
        {
            var dir = roleBase.bladeTran.position - transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Deg2Rad - 180;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }));
        flyTween.Insert(dur - recoverTime, transform.DOLocalRotateQuaternion(Quaternion.identity, recoverTime));
        flyTween.OnComplete(() =>
        {
            // 刀刃转换成当前类型
            if (bladeType != roleBase.curBladeType)
            {
                if (roleBase.bladeList.ContainsKey(gameObject)) roleBase.bladeList.Remove(gameObject);
                //roleBase.AddBladeForce(parent); TODO
                PushInPool();
            }
            else
            {
                spriteRenderer.sortingOrder = ManyKnivesDefine.SortOrder.blade_base;
                collider2D.enabled = true;
                propFlag = false;
                flyFlag = false;
                valid = true;
            }
        });
        // 缩放单独一个Tween
        tweenScale = DOTween.Sequence();
        tweenScale.Append(transform.DOScale(Vector3.one * scaleMulti, dur));
    }

    float recucleDur = 0.2f;
    float recycleScale = 0.1f;
    void Reduce()
    {
        KillFlyTween();
        KillScaleTween();
        KillResetTween();
        parent.Clear();
        parent = null;
        PushInPool();
    }

    public Vector2 Trigger(Vector2 point) => !roleBase.invincible ? Dead(point) : default;

    float moveSpeedMin = 7.5f;
    float moveSpeedMax = 8;
    int rotateSpeedMin = 1600;
    int rotateSpeedMax = 2000;
    float flyDurMin = 0.5f;
    float flyDurMax = 0.6f;
    int dirOffMin = -30;
    int dirOffMax = 30;

    Vector2 Dead(Vector2 point)
    {
        KillFlyTween();
        KillScaleTween();
        KillResetTween();
        valid = false;
        collider2D.enabled = false;
        if (parent != null)
        {
            parent.Clear();
            transform.SetParent(bladeTran);
            parent = null;
        }
        flyTween = DOTween.Sequence();
        var tmpLocalPos = spriteRenderer.transform.localPosition;
        tmpLocalPos.y = -height;
        spriteRenderer.transform.localPosition = tmpLocalPos;
        tmpLocalPos = transform.localPosition;
        tmpLocalPos += tmpLocalPos.normalized * height;
        transform.localPosition = tmpLocalPos;
        transform.SetParent(sceneMgr.levelRoot.transform);
        spriteRenderer.sortingOrder = ManyKnivesDefine.SortOrder.blade_fly;
        var dir = (point - roleBase.rigidbody.position).normalized;
        dir = Vector2.Perpendicular(new Vector2(dir.x, dir.y).normalized);
        var flyDirOff = Mathf.Lerp(dirOffMin, dirOffMax, Util.Random01f());
        dir = (Quaternion.Euler(0, 0, flyDirOff) * (Vector3)dir).normalized;
        var flyDur = Mathf.Lerp(flyDurMin, flyDurMax, Util.Random01f());
        var moveSpeed = Mathf.Lerp(moveSpeedMin, moveSpeedMax, Util.Random01f());
        var rotateSpeed = Mathf.Lerp(rotateSpeedMin, rotateSpeedMax, Util.Random01f());
        tmpLocalPos = transform.position;
        var tarPos = sceneMgr.GetSafetyPosition(tmpLocalPos + (Vector3)dir * (moveSpeed * flyDur));
        flyTween.Insert(0, transform.DOMove(tarPos, flyDur, false).SetEase(Ease.Linear));
        var startAngle = transform.eulerAngles.z;
        flyTween.Insert(0, DOVirtual.Float(startAngle, flyDur * rotateSpeed + startAngle, flyDur, val => transform.localEulerAngles = new Vector3(0, 0, val)).SetEase(Ease.Linear));
        flyTween.InsertCallback(flyDur, () =>
        {
            if (!roleBase.isPlayer)
            {
                spriteRenderer.sortingOrder = ManyKnivesDefine.SortOrder.blade_base;
                gameObject.layer = ManyKnivesDefine.LayerID.triggerEnemy;
                collider2D.enabled = true;
                propFlag = true;
                sceneMgr.AddBlade();
            }
            else PushInPool();
        });
        if (!roleBase.isPlayer)
        {
            // 5秒后消失
            flyTween.InsertCallback(flyDur + 5, () =>
            {
                sceneMgr.ReduceBlade();
                // 回收
                PushInPool();
            });
        }
        RefreshPause();
        return dir;
    }
}
