
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class UIMgr
{
    GameMgr gameMgr;
    AudioMgr audioMgr;
    GameObject mainPanel;
    GameScene sceneMgr;
    Camera uiCamera;

    // 当前关卡
    TextMeshProUGUI lvTitleText;

    // 剩余多少敌人显示
    TextMeshProUGUI roleNumText;
    GameObject bossGuideObj;
    CanvasGroup bossCG;
    TextMeshProUGUI bossText, startGuideText;

    UICtrl uiCtrl;
    RectTransform playerFollower, playerArrow;
    bool startFlag;

    KnifeObjectPool<HpSliderCtrl> hpSliderPool;
    RectTransform hpSliderCon;

    KnifeObjectPool<DamageMgr> dmgPool;
    RectTransform dmgCon;
    public Vector2 realCanvaSize;
    readonly Dictionary<GameObject, DamageMgr> dmgPair = new();

    RectTransform swordBg, fleetfootBG, crazeBg;
    Image swordFill, fleetfootFill, crazeFill;
    Image startGuideImg;
    GameObject startGuide, CDSlider;

    Sequence bossTweener, bossCGTween;
    Tween breathSequence;

    public bool bossFlag;

    public void Init(GameScene scene, AudioMgr audioMgr, GameMgr gameMgr)
    {
        this.gameMgr = gameMgr;
        this.audioMgr = audioMgr;
        sceneMgr = scene;
    }

    public void Dispose()
    {
        startFlag = false;
        hpSliderPool?.Clear();
        hpSliderPool = null;
        dmgPool?.Clear();
        dmgPool = null;
        uiCtrl.joystickBindable.Remove(MoveListener);
        bossTweener?.Kill();
        bossTweener = null;
        breathSequence?.Kill();
        breathSequence = null;
        uiCtrl?.Dispose();
        uiCtrl = null;
    }

    public void Update()
    {
        if (!startFlag) return;
        var anchorPos = sceneMgr.GetPlayerViewPos();
        anchorPos.x = realCanvaSize.x * (anchorPos.x * 0.5f);
        anchorPos.y = realCanvaSize.y * (anchorPos.y * 0.5f);
        playerFollower.anchoredPosition = (Vector2)anchorPos;
        foreach(var item in dmgPair)
        {
            item.Value.Update();
        }
    }

    public void Ready()
    {
        if (mainPanel == null) mainPanel = ResManager.LoadPrefab("MainPanel", null, Vector3.one, Vector3.zero);
        uiCamera = Util.GetComponent<Camera>(mainPanel, "UICamera");
        var canvas = Util.GetGameObject(mainPanel, "Canvas");
        var topRect = Util.GetGameObject(canvas, "topRect");
        swordBg = Util.GetComponent<RectTransform>(topRect, "CDSlider/sword");
        swordFill = Util.GetComponent<Image>(swordBg.gameObject, "Fill");
        fleetfootBG = Util.GetComponent<RectTransform>(topRect, "CDSlider/fleetfoot");
        swordFill = Util.GetComponent<Image>(fleetfootBG.gameObject, "Fill");
        crazeBg = Util.GetComponent<RectTransform>(topRect, "CDSlider/craze");
        crazeFill = Util.GetComponent<Image>(crazeBg.gameObject, "Fill");

        lvTitleText = Util.GetComponent<TextMeshProUGUI>(topRect, "bg/lvText");
        startGuideText = Util.GetComponent<TextMeshProUGUI>(canvas, "startGuide/startText");
        startGuideImg = Util.GetComponent<Image>(canvas, "startGuide/startGuideImg");

        hpSliderCon = Util.GetComponent<RectTransform>(canvas, "hpSliderCon");
        playerFollower = Util.GetComponent<RectTransform>(canvas, "playerFollower");
        playerArrow = Util.GetComponent<RectTransform>(playerFollower.gameObject, "arrow");

        roleNumText = Util.GetComponent<TextMeshProUGUI>(topRect, "enemy/enemyText");
        startGuide = Util.GetGameObject(canvas, "startGuide");
        CDSlider = Util.GetGameObject(topRect, "CDSlider");
        bossGuideObj = Util.GetGameObject(canvas, "bossGuide");
        bossCG = Util.GetComponent<CanvasGroup>(bossGuideObj, "bossCG");
        bossText = Util.GetComponent<TextMeshProUGUI>(bossCG.gameObject, "txt_boss");
        bossText.text = "BOSS来袭";

        lvTitleText.text = "关卡:" + gameMgr.level;
        var tempRealSize = canvas.GetComponent<CanvasScaler>().referenceResolution;
        var realScale = (float)Screen.width / Screen.height;
        var standardScale = tempRealSize.x / tempRealSize.y;
        // 更宽
        if (realScale > standardScale) tempRealSize.x *= realScale / standardScale;
        else if (realScale < standardScale) tempRealSize.y *= standardScale / realScale;
        realCanvaSize = new(Mathf.Round(tempRealSize.x), Mathf.Round(tempRealSize.y));
        var topSize = topRect.GetComponent<RectTransform>().sizeDelta;
        topSize.y = 100 + sceneMgr.TopRatio * realCanvaSize.y;
        topRect.GetComponent<RectTransform>().sizeDelta = topSize;

        var ctrlPanel = Util.GetGameObject(canvas, "ctrlPanel");

        uiCtrl = new(this, ctrlPanel.GetComponent<EventTrigger>(), Util.GetComponent<RectTransform>(ctrlPanel, "Joystick"),
            Util.GetComponent<RectTransform>(ctrlPanel, "Joystick/head"), Util.GetComponent<RectTransform>(ctrlPanel, "joyOriPos"));
        uiCtrl.Hide();
        // 移动控制监听
        uiCtrl.joystickBindable.Add(MoveListener);
        hpSliderPool = new(hpSliderCon);
        dmgPool = new(dmgCon);

        if (gameMgr.level == 1) startGuideText.text = "滑动屏幕拾取刀刃";
        else startGuideText.text = "滑动屏幕开始游戏";

        sceneMgr.enemyNumBind.Add(RoleNumListener);
        sceneMgr.bossTimerBind.Add(BossTimerListener);
        var rolePlayer = sceneMgr.rolePlayer;
        //rolePlayer.crazeBind.Add(value =>TODO
        //{
        //    crazeBg.gameObject.SetActive(value);
        //    if (value) crazeBg.SetAsLastSibling();
        //});
        //rolePlayer.bigSwordBind.Add(value =>TODO
        //{
        //    swordBg.gameObject.SetActive(value);
        //    if (value) swordBg.SetAsLastSibling();
        //});
        //rolePlayer.fleetfootBind.Add(value =>TODO
        //{
        //    fleetfootBG.gameObject.SetActive(value);
        //    if (value) fleetfootBG.SetAsLastSibling();
        //});
        //rolePlayer.bigSwordTimerBind.Add(BigSwordTimer);TODO
        //rolePlayer.crazeTimerBind.Add(CrazeTimer);TODO
        //rolePlayer.fleetfootTimerBind.Add(FleetfootTimer);TODO
        sceneMgr.pauseBind.Add(PauseListener);
    }

    void RefreshPause()
    {
        PauseListener(sceneMgr.pauseBind.value);
    }

    void PauseListener(bool pause)
    {
        var timeScale = pause ? 0 : 1;
        if (bossCGTween != null) bossCGTween.timeScale = timeScale;
        if (breathSequence != null) breathSequence.timeScale = timeScale;
    }

    void CrazeTimer(float value) => crazeFill.fillAmount = 1 - value / ManyKnivesDefine.PlayerEffectConfig.CD_craze;
    void BigSwordTimer(float value) => swordFill.fillAmount = 1 - value / ManyKnivesDefine.PlayerEffectConfig.CD_bigSword;
    void FleetfootTimer(float value) => fleetfootFill.fillAmount = 1 - value / ManyKnivesDefine.PlayerEffectConfig.CD_fleetfoot;

    void MoveListener(Vector2 value)
    {
        if (!startFlag) gameMgr.StartBattle();
        if (value.x != 0 || value.y != 0)
        {
            int radius = 180;
            playerArrow.anchoredPosition = radius * value;
            var deg = Vector2.Angle(value, Vector2.right);
            if (value.y < 0) deg = -deg;
            playerArrow.localEulerAngles = new Vector3(0, 0, deg - 90);
        }
    }

    void RoleNumListener(int value)
    {
        if (!sceneMgr.bossFlag)
        {
            roleNumText.text = "击败目标" + value + sceneMgr.enemyNumMax.ToString();
        }
    }

    void BossTimerListener(float value)
    {
        if (sceneMgr.bossFlag)
        {
            if (value > 0) roleNumText.text = "BOSS刷新" + Math.Floor((float)value / 60) + ":" + Math.Ceiling((float)value % 60);
            else roleNumText.text = "击败BOSS";
        }
    }



    public void BossCGPlay(Action call = null)
    {
        KillBossCG();
        audioMgr.PlayOneShot(ManyKnivesDefine.AudioClips.boss_appears);
        bossGuideObj.SetActive(true);
        bossCG.alpha = 0;
        bossCGTween = DOTween.Sequence();
        bossCGTween.Append(DOVirtual.Float(0, 1, 0.3f, val => bossCG.alpha = val));
        bossCGTween.AppendInterval(1);
        bossCGTween.Append(DOVirtual.Float(1, 0, 0.3f, val => bossCG.alpha = val));
        bossCGTween.OnComplete(() =>
        {
            bossGuideObj.SetActive(false);
            call?.Invoke();
        });
        bossCGTween.SetLink(bossGuideObj);
        RefreshPause();
    }

    void StartBtnTween()
    {
        KillStartTween();
        startGuide.SetActive(true);
        startGuideImg.transform.localScale = Vector3.one;
        breathSequence = startGuideImg.transform.DOScale(Vector3.one * 1.1f, 0.5f).SetEase(Ease.InSine).SetLoops(-1, LoopType.Yoyo);
        RefreshPause();
    }

    void KillStartTween()
    {
        breathSequence?.Kill();
        breathSequence = null;
        startGuide.SetActive(false);
    }

    public void StartBattle()
    {
        KillStartTween();
        playerFollower.gameObject.SetActive(true);
        uiCtrl.Ready();
        CDSlider.SetActive(true);
        startFlag = true;
    }
    public HpSliderCtrl popHpSlider(RoleBase roleBase, EventHandler<int> hpValueBind, EventHandler<int> hpMaxBind)
    {
        var slider = hpSliderPool.Get();
        slider.Init(roleBase, hpValueBind, hpMaxBind, sceneMgr.mainCamera, this);
        slider.transform.SetParent(hpSliderCon);
        slider.transform.localScale = Vector3.one;
        return slider;
    }

    public void PushHpSlider(HpSliderCtrl slider) => hpSliderPool.Put(slider);

    // 伤害 type: 1-普通伤害  2-真伤
    public void ShowDmgText(int type, int dmgValue, Vector3 pos)
    {
        var dmg = dmgPool.Get();
        dmgPair[dmg.gameObject] = dmg;
        dmg.transform.SetParent(dmgCon);
        dmg.Play(type, dmgValue.ToString(), pos, sceneMgr.mainCamera, this);
    }
    public void PushDmgText(DamageMgr item)
    {
        dmgPair.Remove(item.gameObject);
        dmgPool.Put(item);
    }
    public void OverBattle() => uiCtrl.Hide();

    void KillBossCG()
    {
        bossCGTween?.Kill();
        bossCGTween = null;
        bossGuideObj.SetActive(false);
    }

    public void ResetGame()
    {
        sceneMgr.enemyNumBind.Send(sceneMgr.enemyNumBind.value);
        CDSlider.SetActive(false);
        hpSliderPool?.Clear();
        dmgPool?.Clear();
        dmgPair.Clear();
        startFlag = false;
        playerFollower.gameObject.SetActive(false);
        uiCtrl.Hide();
        StartBtnTween();
        KillBossCG();
        bossFlag = false;
    }
}
