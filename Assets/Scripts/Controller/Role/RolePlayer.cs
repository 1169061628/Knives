using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 角色类
/// </summary>
public class RolePlayer : RoleBase
{
    protected CollisionTriggerListener collectRangeListener;
    public CircleCollider2D collectRangeCollider;
    private Transform center;
    private Vector2 lastCtrlVel;
    //回血特效
    private ParticleSystem fx_addHP;
    //获得道具
    private ParticleSystem fx_getProp;
    //狂暴特效
    private ParticleSystem fx_craze;
    //加速拖尾
    private ParticleSystem fx_fleetfoot;
    //狂暴
    private EventHandler<bool> crazeBind;
    private EventHandler<float> crazeTimerBind;
    //大宝剑
    private EventHandler<bool> bigSwordBind;
    private EventHandler<float> bigSwordTimerBind;
    //无敌效果tween
    private Tweener bigSowrdTween;
    //飞毛腿
    private EventHandler<bool> fleetfootBind;
    private EventHandler<float> fleetfootTimerBind;
    //武器特效攻击
    private int bladeFxState;
    private bool bladeFxFlag;
    private float bladeFxTimer;
    private float bladeFxTimerCD;
    //记录发射次数
    private bool bladeFxFlag2;
    private float bladeFxTimer2;
    private float bladeFxTimerCD2;
    //被击退tween
    private Tweener repulseTween;
    private bool repulseFlag;
    //刀刃的父对象(先注释掉，后续有用)
    // bladeTmpPool 
    // bladeTmpList
    //记录闪电技能id，每次只能造成一次伤害
    private int lightID;
    //闪烁
    private Color colYellow = new Color(1, 1, 0, 1);
    private Color colWhite = new Color(1, 1, 1, 1);
    //触碰到的敌人
    private Dictionary<RoleBase, float> hurtByRolePair;
    //碰到的敌人计数
    private int hurtByRoleCount;

    private KnifeObjectPool<BladeParent> bladeTmpPool;

    public override void InitComponent()
    {
        base.InitComponent();
        center = Util.GetComponentByObjectName<Transform>(gameObject, "center");
        fx_addHP = Util.GetComponentByObjectName<ParticleSystem>(gameObject, "fx_huixue");
        fx_getProp = Util.GetComponentByObjectName<ParticleSystem>(gameObject, "fx_huoqudaoju");
        fx_craze = Util.GetComponentByObjectName<ParticleSystem>(gameObject, "fx_kuangbao");
        fx_fleetfoot = Util.GetComponentByObjectName<ParticleSystem>(gameObject, "fx_jiasu_trail");
        bigSwordBind.Send(false);
        bigSwordTimerBind.Send(0);
        crazeBind.Send(false);
        crazeTimerBind.Send(0);
        fleetfootBind.Send(false);
        fleetfootTimerBind.Send(0);
        collectRangeCollider = Util.GetComponentByObjectName<CircleCollider2D>(gameObject, "collectRange");
        collectRangeCollider.gameObject.layer = ManyKnivesDefine.LayerID.triggerPlayer;
        ManyKnivesDefine.Func_IgnoreCollision(collider, collectRangeCollider);
        collectRangeListener = Util.GetComponentByObjectName<CollisionTriggerListener>(gameObject, "collectRange");
        //注册监听事件
        collectRangeListener.RegisterTriggerEnter2D(CollectTriggerEnter2D);
        GameObject parentAndPrefab = Util.GetGameObject(gameObject, "bladeTmp");
        bladeTmpPool = new KnifeObjectPool<BladeParent>();
        bladeNumBind.Add(value =>
        {
            if (sceneMgr.startFlag && value >= ManyKnivesDefine.bladeMaxNum)
            {
                //这里是引导，可以不加
            }
        });
        
        //Init方法里面的
        MoveListener(Vector2.zero);
        fx_addHP.gameObject.SetActive(false);
        fx_getProp.gameObject.SetActive(false);
        fx_craze.gameObject.SetActive(false);
        fx_fleetfoot.gameObject.SetActive(false);
        bigSwordBind.Send(false);
        bigSwordTimerBind.Send(0);
        crazeBind.Send(false);
        crazeTimerBind.Send(0);
        fleetfootBind.Send(false);
        fleetfootTimerBind.Send(0);
        bladeTmpPool.Clear();
        // bladeTmpList
        InitBlade(roleData.bladeType, roleData.bladeNum);
        KillBigSowrdTween();
        SetFillColor(colWhite);
        collectRangeCollider.radius = ManyKnivesDefine.playerCollectRange;
        collectRangeCollider.enabled = false;
        repulseFlag = false;
        animCtrl.Play(ManyKnivesDefine.AnimatorState.idle);
        hurtByRolePair = new Dictionary<RoleBase, float>();
        hurtByRoleCount = 0;
        lightID = 0;

    }

    //玩家刀刃重载
    protected override void InitBlade(int bladeType, int bladeNum)
    {
        base.InitBlade(bladeType, bladeNum);
        bladeList = new Dictionary<GameObject, BladeBase>();
        
    }

    void CollectTriggerEnter2D(Collider2D collider2D)
    {
        
    }

    void MoveListener(Vector2 value)
    {
        if (deadFlag)
        {
            return;
        }

        lastCtrlVel = value;
    }

    void KillBigSowrdTween()
    {
        if (bigSowrdTween != null)
        {
            bigSowrdTween.Kill();
            bigSowrdTween = null;
        }
    }

    protected override void PauseListener(bool pause)
    {
        base.PauseListener(pause);
        int timeScale = pause ? 0 : 1;
    }

    /// <summary>
    /// 被击退
    /// </summary>
    public void PlayerRepulse(Vector3 pos)
    {
        KillRepulseTween();
        repulseFlag = true;
        repulseTween = transform.DOMove(pos, 0.5f).SetEase(Ease.OutQuad);
        repulseTween.OnComplete(() =>
        {
            repulseFlag = false;
        });
        RefreshPause();
    }

    void KillRepulseTween()
    {
        if (!repulseTween.Equals(null))
        {
            repulseTween.Kill();
        }
    }
}
