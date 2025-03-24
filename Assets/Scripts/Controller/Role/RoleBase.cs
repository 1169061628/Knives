using System.Collections.Generic;
using bc.MiniGameBase;
using DG.Tweening;
using UnityEngine;

public class RoleBase : ItemBase
{
    public override void InitComponent()
    {

    }

    // private var EaseTemp = DG.Tweening.Ease;
    // private var easeMove = EaseTemp.OutQuad;
    // private var easeHit = EaseTemp.OutQuad;
    // private var easeHIt2 = EaseTemp.InQuad;
    enum ItemNames
    {
        bladeTriggerListener,
        collider,
        collision,
        animator,
        sort_blade,
        sort_role,
        skinMeshRenderer,
        sort_effect,
        display
    }

    protected Collider collider;
    protected CircleCollider2D collision;
    protected float collisionRadius;
    /// <summary>
    /// 角色物理碰撞监听器
    /// </summary>
    protected CollisionTriggerListener triggerListener;
    /// <summary>
    /// 刀刃碰撞监听器
    /// </summary>
    protected Rigidbody2D bladeRigbody;

    protected CollisionTriggerListener bladeTriggerListener;

    protected Scene sceneMgr;

    // protected UIMgr uiMgr;
    // 是否为boss
    protected bool isBoss;
    // 角色名字
    protected string roleName;

    protected RoleConfigArgs roleData;
    // 初始位置
    protected Vector3 initPos;
    protected Dictionary<GameObject, GameObject> bladeList;
    protected Transform disply;
    // 无敌状态
    protected bool invincible;
    // 无伤状态
    protected bool noInjury;
    // 不受击退控制
    protected bool uncontrolled;
    // 移动速度
    private EventHandler<int> moveSpBind = new();
    // 记的当前速度
    private int curMoveSp;
    private EventHandler<int> aiMoveBind = new();
    // 特殊静止状态
    private bool fixedlyFlag;
    // 攻击速度
    private EventHandler<int> atkSpBind = new();
    // 刀刃旋转速度
    private EventHandler<int> bladeSpBind = new();
    // bladeRotTween（这个属性没用到）
    
    // 真实血量
    private EventHandler<int> hpValueBind = new();
    // 最大血量
    private EventHandler<int> hpMaxBind = new();
    // 刀刃数量
    private EventHandler<int> bladeNumBind = new();
    // 动画控制器(后面写)
    // AnimCtr
    private GameObject hpSlider;
    // 血条的位置
    private Transform hpAnchor;
    // 伤害数字位置
    private float dmgOffY;
    // 冷冻特效
    private ParticleSystem freezeFx;
    // 闪电特效
    private ParticleSystem fx_Light;
    
    // debuff相关
    // 减速标记
    // private bool debuff_moveSp_Flag;
    // private bool debuff_moveSp_Timer;
    // private bool debuff_freeze_Flag;
    // private bool debuff_freeze_Timer;
    // private bool debuff_light_Flag;
    // private bool debuff_light_Timer;
    // private bool debuff_lightID;
    
    // 碰了多少个毒气
    private Dictionary<GameObject, int> hurtByMiasmaPair;
    // 毒气计时器
    private int hurtByMiasmaTimer;
    private int hurtByMiasmaCount;
    private GameObject miasmaObj;
    private float miasmaDmg;
    // 死亡回收标记
    private Sequence deadTween;

}
