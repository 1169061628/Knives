using UnityEngine;
using DG.Tweening;

public class Effect_Partical_Item : ItemBase
{
    GameScene sceneMgr;
    ParticleSystem particle;
    string type;
    Tween tween;
    public override void InitComponent()
    {
        particle = gameObject.GetComponentInChildren<ParticleSystem>();
    }

    public void Init(GameScene sceneMgr, string type)
    {
        this.sceneMgr = sceneMgr;
        this.type = type;
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

    // actionType 1-延迟消失
    public void Play(int actionType, Vector3 startPos, float dur, float scale = 1)
    {
        KillTween();
        transform.SetParent(sceneMgr.levelRoot.transform);
        if (particle != null)
        {
            particle.Simulate(0, true);
            particle.Play();
        }
        if (actionType == 1)
        {
            transform.localScale = Vector3.one * scale;
            transform.position = startPos;
            tween = DOVirtual.DelayedCall(dur, () => sceneMgr.PushEffect(type, this), false);
        }
        else if (actionType == 2)
        {
            transform.position = startPos;
            tween = DOVirtual.DelayedCall(dur, () => sceneMgr.PushEffect(type, this), false);
        }
        RefreshPause();
    }

    void KillTween()
    {
        tween?.Kill();
        tween = null;
    }

    void Dispose() => KillTween();
    void Recycle()
    {
        KillTween();
        sceneMgr.pauseBind.Remove(PauseListener);
    }
}