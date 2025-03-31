using System.Collections.Generic;
using bc.MiniGameBase;
using DG.Tweening;
using Pathfinding;
using UnityEditor.UI;
using UnityEngine;

public class RoleBase : ItemBase
{
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

    protected CircleCollider2D collider;
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

    protected UIMgr uiMgr;
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
    private EventHandler<bool> aiMoveBind = new();
    // 特殊静止状态
    private bool fixedlyFlag;
    // 攻击速度
    private EventHandler<int> atkSpBind = new();
    // 刀刃旋转速度
    private EventHandler<float> bladeSpBind = new();
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
    private bool debuff_moveSp_Flag;
    private bool debuff_moveSp_Timer;
    private bool debuff_freeze_Flag;
    private bool debuff_freeze_Timer;
    private bool debuff_light_Flag;
    private bool debuff_light_Timer;
    private int debuff_lightID;
    
    // 碰了多少个毒气
    private Dictionary<GameObject, int> hurtByMiasmaPair;
    // 毒气计时器
    private int hurtByMiasmaTimer;
    private int hurtByMiasmaCount;
    private GameObject miasmaObj;
    private float miasmaDmg;
    // 死亡回收标记
    private Sequence deadTween;
    // 是否准备好
    private bool readyFlag;
    // 移动方向随机偏移
    private Quaternion moveOffRot;
    private float roleMoveOff;
    // 寻路相关
    private AIDestinationSetter aiDestinationSetter;
    private AIPath aiPath;
    private Vector3 lastAIPosition;
    private string mpb_FillPhase = "_FillPhase";
    private string mpb_FillColor = "_FillColor";
    private string mpb_DissolveThreshold = "_DissolveThreshold";
    private string mpb_Color = "_Color";

    private Rigidbody2D rigidbody;
    private Transform bladeTran;
    private Animator animator;
    private SkinnedMeshRenderer skinMeshRenderer;
    private Material material;
    private Transform effectParent;
    public bool isPlayer;
    private RolePlayer player;
    private bool deadFlag;
    private int hurtByRoleCount;
    
    public override void InitComponent()
    {
        aiDestinationSetter = Util.GetComponentByObjectName<AIDestinationSetter>(gameObject, "ironBoss");
        aiPath = Util.GetComponentByObjectName<AIPath>(gameObject, "ironBoss");
        triggerListener = Util.GetComponentByObjectName<CollisionTriggerListener>(gameObject, "player");
        rigidbody = Util.GetComponentByObjectName<Rigidbody2D>(gameObject, "player");
        bladeTriggerListener = Util.GetComponentByObjectName<CollisionTriggerListener>(gameObject, "blade");
        bladeRigbody = Util.GetComponentByObjectName<Rigidbody2D>(gameObject, "blade");
        bladeTran = bladeTriggerListener.transform;
        collider = Util.GetComponentByObjectName<CircleCollider2D>(gameObject, "trigger");
        collision = Util.GetComponentByObjectName<CircleCollider2D>(gameObject, "collision");
        collisionRadius = collision.radius;
        // collision.gameObject.SetActive(false);
        animator = Util.GetComponentByObjectName<Animator>(gameObject, "edit_hauptfigur");
        skinMeshRenderer = Util.GetComponentByObjectName<SkinnedMeshRenderer>(gameObject, "COMB");
        material = skinMeshRenderer.material;
        disply = Util.GetComponentByObjectName<Transform>(gameObject, "display");
        effectParent = Util.GetComponentByObjectName<Transform>(gameObject, "effect");
        // self.animCtrl = require("bcmanyknives_anim_ctrl").new(self.animator, self.moveSpBind, self.atkSpBind)
        hpAnchor = Util.GetComponentByObjectName<Transform>(gameObject, "hpAnchor");
        dmgOffY = skinMeshRenderer.bounds.size.y * 0.5f;
        freezeFx = Util.GetComponentByObjectName<ParticleSystem>(gameObject, "fx_snow02");
        fx_Light = Util.GetComponentByObjectName<ParticleSystem>(gameObject, "fx_shandianjingu");
        triggerListener.RegisterTriggerEnter2D(OnTriggerEnter2D);
        triggerListener.RegisterTriggerExit2D(OnTriggerExit2D);
        bladeTriggerListener.RegisterTriggerEnter2D(BladeOnTriggerEnter2D);
        // self.aiMoveBind = tinyrush_bindable.new(true)
        if (aiPath)
        {
            moveSpBind.Add(value => aiPath.maxSpeed = value);
            aiMoveBind.Add(value => aiPath.canMove = value);
        }
    }

    protected GameObject GetColliderKey()
    {
        return collider.gameObject;
    }

    void SetFillPhase(int value, bool upLoad)
    {
        material.SetFloat(mpb_FillPhase, value);
    }

    void SetFillColor(Color value, bool upLoad)
    {
        material.SetColor(mpb_FillColor, value);
    }

    void SetDissolve(float value, bool upLoad)
    {
        material.SetFloat(mpb_DissolveThreshold, value);
    }

    void SetColor(Color value, bool upLoad)
    {
        material.SetColor(mpb_Color, value);
    }

    public virtual void Init(Scene scene, UIMgr uiMgr, string roleName, RoleConfigArgs configData, Vector3 spawnPos)
    {
        sceneMgr = scene;
        uiMgr = uiMgr;
        roleName = roleName;
        roleData = configData;
        initPos = spawnPos;
        transform.position = spawnPos;
        isPlayer = roleName == ManyKnivesDefine.RoleNames.player;
        player = scene.rolePlayer;
        bladeSpBind.Send(isPlayer ? ManyKnivesDefine.bladeInitSpeed : ManyKnivesDefine.bladeInitSpeed_Enemy);
        //这里需要只设置值，不调用回调
        hpMaxBind.Send(roleData.hp);
        hpValueBind.Send(roleData.hp);
        curMoveSp = roleData.speed;
        moveSpBind.Send(curMoveSp);
        //这里需要只设置值，不调用回调
        bladeNumBind.Send(0);
        SetFillPhase(0, false);
        SetDissolve(0, true);
        collider.enabled = true;
        rigidbody.simulated = true;
        bladeRigbody.simulated = true;
        collider.gameObject.layer =
            isPlayer ? ManyKnivesDefine.LayerID.triggerPlayer : ManyKnivesDefine.LayerID.triggerEnemy;
        collision.gameObject.layer =
            isPlayer ? ManyKnivesDefine.LayerID.collisionMap : ManyKnivesDefine.LayerID.collisionEnemy;
        if (!isPlayer)
        {
            ManyKnivesDefine.Func_IgnoreCollision(player.collision, collision);
            ManyKnivesDefine.Func_IgnoreCollision(player.collectRangeCollider, collision);
            ManyKnivesDefine.Func_IgnoreCollision(player.collectRangeCollider, collider);
        }

        noInjury = false;
        invincible = false;
        uncontrolled = false;
        deadFlag = false;
        isBoss = false;
        fixedlyFlag = false;
        hurtByMiasmaPair = new Dictionary<GameObject, int>();
        hurtByRoleCount = 0;
        hurtByMiasmaTimer = 0;
        hurtByMiasmaCount = 0;
        freezeFx.gameObject.SetActive(false);
        fx_Light.gameObject.SetActive(false);
        debuff_freeze_Flag = false;
        debuff_light_Flag = false;
        debuff_lightID = 0;
        // animCtrl.Freeze(false);
        if (!isPlayer)
        {
            Start();
        }
        
        scene.pauseBind.Add(PauseListener);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider2D)
    {
    }

    protected virtual void OnTriggerExit2D(Collider2D collider2D)
    {
    }

    protected virtual void BladeOnTriggerEnter2D(Collider2D collider2D)
    {
    }

    void PauseListener(bool value)
    {

    }

    void Start()
    {
    }

    public void Recycle()
    {
        sceneMgr.pauseBind.Remove(PauseListener);
    }
}
