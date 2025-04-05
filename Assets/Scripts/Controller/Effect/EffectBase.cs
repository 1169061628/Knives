using DG.Tweening;
using UnityEngine;

public class EffectBase : ItemBase
{
    // 对应的角色
    RoleBase roleBase;
    string type;
    Scene sceneMgr;
    Collider2D collider;
    ParticleSystem particle;
    int effectID;
    Tween tween;
    public override void InitComponent()
    {
        collider = gameObject.GetComponent<Collider2D>();
        particle = gameObject.GetComponentInChildren<ParticleSystem>();
    }

    public void Init(Scene sceneMgr, string type, RoleBase roleBase)
    {
        this.roleBase = roleBase;
        this.sceneMgr = sceneMgr;
        this.type = type;
        gameObject.layer = roleBase.isPlayer ? ManyKnivesDefine.LayerID.triggerPlayer : ManyKnivesDefine.LayerID.triggerEnemy;
        collider.enabled = true;
        sceneMgr.pauseBind.Add(PauseListener);
    }

    void RefreshPause()
    {
        PauseListener(sceneMgr.pauseBind.value);
    }

    void PauseListener(bool value)
    {
        var timeScale = value ? 0 : 1;
        if (tween != null) tween.timeScale = timeScale;
    }

    public void Play(Vector3 startPos, Vector3 dir, float dis, float moveSp, float degOff)
    {
        KillTween();
        transform.SetParent(sceneMgr.levelRoot.transform);
        transform.position = startPos;
        transform.localScale = Vector3.one;
        var deg = Vector2.Angle((Vector2)dir, Vector2.right);
        if (dir.y < 0) deg = -deg;
        transform.localRotation = Quaternion.Euler(0, 0, deg + degOff);
        tween = transform.DOMove(startPos + dir * dis, dis / moveSp).SetEase(Ease.Linear).OnComplete(() => sceneMgr.PushEffect(type, this));
        if (particle != null)
        {
            particle.Simulate(0, true);
            particle.Play();
        }
        RefreshPause();
    }
    public void Play(Transform parent, float tarScale, float dur, int param)
    {
        KillTween();
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one * tarScale;
        tween = DOVirtual.DelayedCall(dur, () => sceneMgr.PushEffect(type, this), false);
        effectID = param;
        if (particle != null)
        {
            particle.Simulate(0, true);
            particle.Play();
        }
        RefreshPause();
    }

    public void Play(Vector3 startPos, float dur, float scale = 1)
    {
        KillTween();
        transform.SetParent(sceneMgr.levelRoot.transform);
        transform.position = startPos;
        transform.localScale = Vector3.one * scale;
        tween = DOVirtual.DelayedCall(dur, () => sceneMgr.PushEffect(type, this), false);
        if (particle != null)
        {
            particle.Simulate(0, true);
            particle.Play();
        }
        RefreshPause();
    }

    public void Dispose()
    {
        KillTween();
    }
    public void Recycle()
    {
        KillTween();
        sceneMgr.pauseBind.Remove(PauseListener);
    }

    // 生效后，直接回收
    public void Trigger()
    {
        KillTween();
        collider.enabled = false;
        sceneMgr.PushEffect(type, this);
    }

    void KillTween()
    {
        tween?.Kill();
        tween = null;
    }

}
