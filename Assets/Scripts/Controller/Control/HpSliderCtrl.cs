﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class HpSliderCtrl : ItemBase
{
    Image playerFill, enemyFill;
    Camera mainCamera;
    UIMgr uiMgr;
    TextMeshProUGUI text;
    RectTransform bgRect;
    RoleBase roleBase;

    const int fillMin = 0;
    const int fillMax = 100;
    readonly RectTransform.Axis horAxis = RectTransform.Axis.Horizontal;

    public override void InitComponent()
    {
        playerFill = Util.GetComponent<Image>(gameObject, "playerFill");
        enemyFill = Util.GetComponent<Image>(gameObject, "enemyFill");
        bgRect = gameObject.GetComponent<RectTransform>();
        text = Util.GetComponent<TextMeshProUGUI>(gameObject, "text");
    }

    public void Init(RoleBase roleBase, EventHandler<float> hpBind, EventHandler<int> hpMaxBind, Camera camera, UIMgr uiMgr)
    {
        this.roleBase = roleBase;
        mainCamera = camera;
        this.uiMgr = uiMgr;
        playerFill.gameObject.SetActive(roleBase.isPlayer);
        enemyFill.gameObject.SetActive(!roleBase.isPlayer);
        hpBind.Add(HpListener);
        hpMaxBind.Add(HpListener);
    }

    void HpListener(float _)
    {
        var process = (float)roleBase.hpValueBind.value / roleBase.hpMaxBind.value;
        var fill = roleBase.isPlayer ? playerFill : enemyFill;
        if (process <= 0) fill.rectTransform.SetSizeWithCurrentAnchors(horAxis, 0);
        else fill.rectTransform.SetSizeWithCurrentAnchors(horAxis, Mathf.Lerp(fillMin, fillMax, process));
        text.text = Mathf.Ceil(roleBase.hpValueBind.value).ToString();
    }
    void HpListener(int _) => HpListener(0f);

    public void RefreshPos(Vector3 pos)
    {
        var anchorPos = mainCamera.WorldToViewportPoint(pos);
        anchorPos.x = uiMgr.realCanvasSize.x * (anchorPos.x - 0.5f);
        anchorPos.y = uiMgr.realCanvasSize.y * (anchorPos.y - 0.5f);
        bgRect.anchoredPosition = (Vector2)anchorPos;
    }

    public void Dispose() => Recycle();

    void Recycle()
    {
        roleBase.hpValueBind.Remove(HpListener);
        roleBase.hpMaxBind.Remove(HpListener);
    }
}