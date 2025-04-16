using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DamageMgr : ItemBase
{
    Text text;
    RectTransform bgRect;
    Sequence tween;
    Vector3 worldPos;
    Camera mainCamera;
    UIMgr uiMgr;

    const int leftOff = -50;
    const int rightOff = 50;
    const int height = 40;
    readonly Color colorRed = new(1, 0, 0, 1);
    readonly Color colorRedClear = new(1, 0, 0, 1);
    readonly Color colorWhite = Color.white;
    readonly Color colorWhiteClear = new(1, 1, 1, 0);

    public override void InitComponent()
    {
        text = Util.GetComponent<Text>(gameObject, "Text");
        bgRect = gameObject.GetComponent<RectTransform>();
    }

    public override void Update()
    {
        var anchorPos = mainCamera.WorldToViewportPoint(worldPos);
        anchorPos.x = uiMgr.realCanvasSize.x * (anchorPos.x - 0.5f);
        anchorPos.y = uiMgr.realCanvasSize.y * (anchorPos.y - 0.5f);
        bgRect.anchoredPosition = (Vector2)anchorPos;
    }

    // type:1-普通伤害 2-真伤
    public void Play(int type, string str, Vector3 pos, Camera carema, UIMgr uiMgr)
    {
        mainCamera = carema;
        this.uiMgr = uiMgr;
        worldPos = pos;
        KillTween();
        text.text = str;
        var oriCol = type == 1 ? colorRed : colorWhite;
        var tarCol = type == 1 ? colorRedClear : colorWhiteClear;
        text.color = oriCol;
        tween = DOTween.Sequence();
        Update();
        transform.localScale = Vector3.one;
        text.rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(leftOff, rightOff, Util.Random01f()), 0);
        text.transform.localScale = Vector3.one * 0.4f;
        // 放大
        tween.Insert(0, text.transform.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutQuad));
        // 向上移动
        tween.Insert(0, text.transform.DOLocalMoveY(height, 0.8f, false).SetEase(Ease.OutSine));
        // 渐隐
        tween.Insert(0.5f, DOVirtual.Float(0, 1, 0.3f, val => text.color = Color.Lerp(oriCol, tarCol, val)).SetEase(Ease.OutQuad));
        // 缩放
        tween.Insert(0.5f, text.transform.DOScale(Vector3.one * 0.5f, 0.3f).SetEase(Ease.Linear));
        tween.OnComplete(() => uiMgr.PushDmgText(this));
        tween.SetLink(gameObject);
        uiMgr.sceneMgr.pauseBind.Add(PauseListener);
    }

    void PauseListener(bool value)
    {
        if (tween != null) tween.timeScale = value ? 0 : 1;
    }

    void KillTween()
    {
        tween?.Kill();
        tween = null;
    }

    public void Recycle()
    {
        KillTween();
        uiMgr.sceneMgr.pauseBind.Remove(PauseListener);
    }

    public void Dispose()
    {
        KillTween();
    }
}