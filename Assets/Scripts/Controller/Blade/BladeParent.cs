using DG.Tweening;
using UnityEngine;
public class BladeParent : ItemBase
{
    BladeBase blade;
    int index;
    Scene sceneMgr;
    Sequence resetTween;

    public override void InitComponent() { }

    public void Init(Scene scene, BladeBase blade)
    {
        sceneMgr = scene;
        this.blade = blade;
        scene.pauseBind.Add(PauseListener);
    }

    void SetIndex(int index)
    {
        this.index = index;
        gameObject.name = index.ToString();
    }

    public bool IsEmpty() => blade == null;

    void RefreshPause()
    {
        PauseListener(sceneMgr.pauseBind.value);
    }

    void PauseListener(bool pause)
    {
        var timeScale = pause ? 0 : 1;
        if (resetTween != null) resetTween.timeScale = timeScale;
    }

    public void Clear() => blade = null;

    public void ResetRotation(Vector3 localPos, Quaternion localRot)
    {
        KillResetTween();
        resetTween = DOTween.Sequence();
        var angle = Quaternion.Angle(transform.localRotation, localRot);
        var dur = Quaternion.Angle(transform.localRotation, localRot) / 260;
        resetTween.Insert(0, transform.DOLocalMove(localPos, dur));
        resetTween.Insert(0, transform.DOLocalRotateQuaternion(localRot, dur));
        RefreshPause();
    }

    void KillResetTween()
    {
        resetTween?.Kill();
        resetTween = null;
    }

    public void Recycle()
    {
        KillResetTween();
        sceneMgr.pauseBind.Remove(PauseListener);
    }
}