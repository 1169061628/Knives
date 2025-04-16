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
    private Dictionary<RoleBase, float> hurtByRoleCD;
    //碰到的敌人计数
    private int hurtByRoleCount;

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
