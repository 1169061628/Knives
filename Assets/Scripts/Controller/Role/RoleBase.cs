using System.Collections.Generic;
using DG.Tweening;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

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

    protected GameScene sceneMgr;

    protected UIMgr uiMgr;
    // 是否为boss
    public bool isBoss;
    // 角色名字
    protected string roleName;

    protected RoleConfigArgs roleData;
    // 初始位置
    protected Vector3 initPos;
    public Dictionary<GameObject, BladeBase> bladeList;
    protected Transform disply;
    // 无敌状态
    public bool invincible;
    // 无伤状态
    protected bool noInjury;
    // 不受击退控制
    protected bool uncontrolled;
    // 移动速度
    protected EventHandler<int> moveSpBind = new();
    // 记的当前速度
    protected int curMoveSp;
    private EventHandler<bool> aiMoveBind = new();
    // 特殊静止状态
    protected bool fixedlyFlag;
    // 攻击速度
    protected EventHandler<int> atkSpBind = new();
    // 刀刃旋转速度
    private EventHandler<float> bladeSpBind = new();
    // bladeRotTween（这个属性没用到）
    
    // 真实血量
    public EventHandler<float> hpValueBind = new();
    // 最大血量
    public EventHandler<int> hpMaxBind = new();
    // 刀刃数量
    public EventHandler<int> bladeNumBind = new();
    // 动画控制器(后面写)
    protected AnimControl animCtrl;
    private HpSliderCtrl hpSlider;
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
    protected bool debuff_freeze_Flag;
    private float debuff_freeze_Timer;
    protected bool debuff_light_Flag;
    private float debuff_light_Timer;
    private int debuff_lightID;
    
    // 碰了多少个毒气
    private Dictionary<GameObject, int> hurtByMiasmaPair;
    // 毒气计时器
    private float hurtByMiasmaTimer;
    private int hurtByMiasmaCount;
    private GameObject miasmaObj;
    private float miasmaDmg;
    // 死亡回收标记
    private Sequence deadTween;
    // 是否准备好
    private bool readyFlag;
    // 移动方向随机偏移
    protected Quaternion moveOffRot;
    protected float moveOffTimer;
    // 寻路相关
    private AIDestinationSetter aiDestinationSetter;
    protected AIPath aiPath;
    private Vector3 lastAIPosition;
    private string mpb_FillPhase = "_FillPhase";
    private string mpb_FillColor = "_FillColor";
    private string mpb_DissolveThreshold = "_DissolveThreshold";
    private string mpb_Color = "_Color";

    public Rigidbody2D rigidbody;
    public Transform bladeTran;
    private Animator animator;
    private SkinnedMeshRenderer skinMeshRenderer;
    private Material material;
    private Transform effectParent;
    public bool isPlayer;
    protected RolePlayer player;
    protected bool deadFlag;
    private int hurtByRoleCount;
    private Tweener backUpTween;
    private Sequence hitTween;
    public int curBladeType;

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
        animCtrl = new AnimControl(animator, moveSpBind, atkSpBind);
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

    protected void SetFillPhase(float value, bool upLoad = false)
    {
        material.SetFloat(mpb_FillPhase, value);
    }

    protected void SetFillColor(Color value, bool upLoad = false)
    {
        material.SetColor(mpb_FillColor, value);
    }

    protected void SetDissolve(float value, bool upLoad = false)
    {
        material.SetFloat(mpb_DissolveThreshold, value);
    }

    protected void SetColor(Color value, bool upLoad = false)
    {
        material.SetColor(mpb_Color, value);
    }

    public virtual void Init(GameScene scene, UIMgr uiMgr, string roleName, RoleConfigArgs configData, Vector3 spawnPos)
    {
        sceneMgr = scene;
        this.uiMgr = uiMgr;
        this.roleName = roleName;
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
        animCtrl.Freeze(false);
        if (!isPlayer)
        {
            Start();
        }
        
        scene.pauseBind.Add(PauseListener);
    }

    protected void RefreshPause()
    {
        PauseListener(sceneMgr.pauseBind.value);
    }
    
    protected virtual void PauseListener(bool pause)
    {
        int timeScale = pause ? 0 : 1;
        if (!backUpTween.Equals(null))
        {
            backUpTween.timeScale = timeScale;
        }

        if (hitTween.Equals(null))
        {
            hitTween.timeScale = timeScale;
        }

        if (!deadTween.Equals(null))
        {
            deadTween.timeScale = timeScale;
        }

        if (pause)
        {
            // animCtrl.Freeze(true);
            SetAICanMove(false);
        }
        else
        {
            SetAICanMove(true);
            if (!debuff_freeze_Flag || debuff_light_Flag)
            {
                // animCtrl.Freeze(false);
            }
        }
    }
    
    protected virtual void Start()
    {
        hpSlider = uiMgr.popHpSlider(this, hpValueBind, hpMaxBind);
        UpdateHPSliderPos();
        readyFlag = true;
        moveOffTimer = ManyKnivesDefine.RoleMoveOff.offTimeCD;
        moveOffRot = Quaternion.identity;
        lastAIPosition = default;
        aiMoveBind.Send(true);
    }
    
    protected void SetAICanMove(bool value)
    {
        if (!aiPath)
        {
            var canM = value && !(debuff_freeze_Flag || debuff_light_Flag || fixedlyFlag);
            aiMoveBind.Send(canM);
            // moveSpBind.Send(canM ? curMoveSp : 0);
        }
    }

    protected void MoveFollow()
    {
        if (deadFlag) return;
        var tarPos = player.transform.position;
        aiPath.destination = tarPos;
        RefreshDisplayFlip();
    }

    protected void RefreshDisplayFlip()
    {
        var curPos = transform.position;
        if (lastAIPosition == default || Mathf.Abs(curPos.x - lastAIPosition.x) >= 0.05f)
        {
            SetDisplayFlip(aiPath.desiredVelocity.x < 0);
            lastAIPosition = curPos;
        }
    }
    
    protected virtual void BladeOnTriggerEnter2D(Collider2D collider2D)
    {
        if (!readyFlag || deadFlag || sceneMgr.overFlag || noInjury)
            return;
        string[] splitStrs = collider2D.name.Split(ManyKnivesDefine.Names.split);
        var type = int.Parse(splitStrs[1]);
        var targetObj = collider2D.gameObject;
        //碰到了刀刃
        if (type == ManyKnivesDefine.TriggerType.blade)
        {
            var targetClass = sceneMgr.bladePairWithGO[targetObj];
            if (targetClass == null || !targetClass.valid || targetClass.roleBase == null ||
                targetClass.roleBase == this || targetClass.roleBase.invincible)
            {
                return;
            }

            var pos = collider2D.bounds.ClosestPoint(transform.position);
            //从角色刀刃池中移除
            targetClass.roleBase.bladeList[targetClass.gameObject] = null;
            targetClass.roleBase.bladeNumBind.Send(targetClass.roleBase.bladeNumBind.value - 1);
            var bladeFlyDir = targetClass.Trigger(rigidbody.position);
            var fx = sceneMgr.PopEffect(ManyKnivesDefine.SkillNames.fx_pindao) as Effect_Partical_Item;
            fx?.Init(sceneMgr, ManyKnivesDefine.SkillNames.fx_pindao);
            fx?.Play(2, pos, 0.5f);
            fx.transform.localScale = Vector3.one * Mathf.Lerp(0.8f, 1, Random.Range(0,2f));
            fx.transform.localRotation = Quaternion.Euler(0,0,Random.Range(0, 360f));
            if (targetClass.roleBase.player == null)
            {
                //往一个方向上抖动
                sceneMgr.cameraCtrl.Shake_BladeTrigger(bladeFlyDir);
                targetClass.roleBase.BackUpTween(transform.position);
                sceneMgr.audioMgr.PlayKnifeFightSound(curBladeType);
            }
        }
        //碰到了角色
        else if (type == ManyKnivesDefine.TriggerType.role)
        {
            RoleBase targetClass = sceneMgr.rolePairWithGO[targetObj];
            //碰到了自己的角色
            if (targetClass == this || targetClass.invincible)
            {
                return;
            }

            if (isPlayer)
            {
                sceneMgr.cameraCtrl.Shake_EnemyHurt();
                var dmg = ManyKnivesDefine.bladeDmgWithType[ManyKnivesDefine.PropType.blade_default] -
                          targetClass.roleData.defence;
                dmg = dmg * (ManyKnivesDefine.bladeDmgWithType[curBladeType] /
                             ManyKnivesDefine.bladeDmgWithType[ManyKnivesDefine.PropType.blade_default]);
                targetClass.RoleTrigger(transform.position, dmg, true, true);
            }
            else
            {
                targetClass.RoleTrigger(transform.position, roleData.bladeDmg, false);
            }
            sceneMgr.audioMgr.PlayHitSound();
        }
    }
    
    protected virtual void OnTriggerExit2D(Collider2D collider2D)
    {
        if (deadFlag || sceneMgr.overFlag)
        {
            return;
        }

        string[] splitStrs = collider2D.name.Split(ManyKnivesDefine.Names.split);
        int type = int.Parse(splitStrs[1]);
        var targetObj = collider2D.gameObject;
        //碰到了特效
        if (type == ManyKnivesDefine.TriggerType.effect)
        {
            var effectType = int.Parse(splitStrs[3]);
            //都能生效的特效
            if (effectType == ManyKnivesDefine.EffectType.miasma)
            {
                if (hurtByMiasmaPair.ContainsKey(targetObj))
                {
                    hurtByMiasmaPair[targetObj] = 0;
                    hurtByMiasmaCount -= 1;
                }
            }
        }
    }
    
    protected virtual void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (deadFlag || sceneMgr.overFlag)
        {
            return;
        }

        string[] splitStrs = collider2D.name.Split(ManyKnivesDefine.Names.split);
        var type = int.Parse(splitStrs[1]);
        var targetObj = collider2D.gameObject;
        //碰到了特效
        if (type == ManyKnivesDefine.TriggerType.effect)
        {
            var targetClass = sceneMgr.effectPairWithGO[targetObj] as EffectBase;
            //碰到了自己的特效
            if (targetClass == null || targetClass.roleBase == this)
            {
                return;
            }

            float dmg = 0;
            var effectType = int.Parse(splitStrs[3]);
            if (!noInjury && invincible)
            {
                //只有敌人生效的特效
                if (!isPlayer)
                {
                    if (effectType == ManyKnivesDefine.EffectType.lightning)
                    {
                        if (debuff_lightID != targetClass.effectID)
                        {
                            debuff_light_Flag = true;
                            debuff_freeze_Timer = 0;
                            fx_Light.gameObject.SetActive(true);
                            animCtrl.Freeze(true);
                            SetAICanMove(false);
                            dmg = ManyKnivesDefine.PlayerEffectDmg.lightning / 100 * hpMaxBind.value;
                            RoleTrigger(targetObj.transform.position, dmg, true);
                            debuff_lightID = targetClass.effectID;
                        }
                    }
                    //冰冻
                    else if (effectType == ManyKnivesDefine.EffectType.snow)
                    {
                        targetClass.Trigger();
                        debuff_freeze_Flag = true;
                        debuff_freeze_Timer = 0;
                        freezeFx.gameObject.SetActive(true);
                        animCtrl.Freeze(true);
                        SetAICanMove(false);
                        dmg = ManyKnivesDefine.PlayerEffectDmg.snow / 100 * hpMaxBind.value;
                        RoleTrigger(targetClass.roleBase.transform.position, dmg, true);
                        sceneMgr.audioMgr.PlayFreezeSound();
                    }
                }
            }
            //都能生效的特效
            if (effectType == ManyKnivesDefine.EffectType.miasma)
            {
                if (!hurtByMiasmaPair.ContainsKey(targetObj))
                {
                    if (hurtByMiasmaCount < 1)
                    {
                        hurtByMiasmaTimer = ManyKnivesDefine.RoleDebuffConfig.hurtByMiasmaCD;
                    }

                    targetClass = sceneMgr.effectPairWithGO[targetObj] as EffectBase;
                    if (targetClass.roleBase.isPlayer)
                    {
                        miasmaDmg = ManyKnivesDefine.PlayerEffectDmg.miasma / 100 * hpMaxBind.value;
                    }
                    else
                    {
                        miasmaDmg = targetClass.roleBase.miasmaDmg;
                    }

                    miasmaObj = targetObj;
                    hurtByMiasmaCount += 1;
                    hurtByMiasmaPair[targetObj] = 1;
                }
            }
            else if (effectType == ManyKnivesDefine.EffectType.fire)
            {
                if (!noInjury || !invincible)
                {
                    if (targetClass.roleBase.isPlayer)
                    {
                        dmg = ManyKnivesDefine.PlayerEffectDmg.fire / 100 * hpMaxBind.value;
                    }
                    else
                    {
                        if (targetClass.roleBase.isBoss)
                        {
                            Debug.LogError("被调用了");
                            //dmg = targetClass.roleBase.fireBossDmg;
                        }
                        else
                        {
                            dmg = targetClass.roleBase.roleData.skillDmg;
                        }
                    }
                    RoleTrigger(targetClass.roleBase.transform.position, dmg, true);
                    targetClass.Trigger();
                }
            }
        }
    }

    public override void Update()
    {
        float deltaTime = Time.deltaTime;
        if (deadFlag)
        {
            return;
        }

        if (bladeTran != null)
        {
            bladeTran.Rotate(new Vector3(0, 0, bladeSpBind.value * -deltaTime));
        }
        UpdateHPSliderPos();
        UpdateDebuff(deltaTime);
    }

    public override void LateUpdate()
    {
        if (deadFlag)
        {
            return;
        }
        UpdateHPSliderPos();
    }

    protected void UpdateDebuff(float deltaTime)
    {
        //只有敌人才生效
        if (!isPlayer)
        {
            //冷冻
            if (debuff_freeze_Flag)
            {
                debuff_freeze_Timer += deltaTime;
                if (debuff_freeze_Timer >= ManyKnivesDefine.RoleDebuffConfig.debuff_freeze_CD)
                {
                    debuff_freeze_Flag = false;
                    freezeFx.gameObject.SetActive(false);
                    SetAICanMove(true);
                    if (!debuff_light_Flag)
                    {
                        animCtrl.Freeze(false);
                    }
                }
            }

            if (debuff_light_Flag)
            {
                debuff_freeze_Timer += deltaTime;
                if (debuff_light_Timer >= ManyKnivesDefine.RoleDebuffConfig.debuff_light_CD)
                {
                    debuff_light_Flag = false;
                    fx_Light.gameObject.SetActive(false);
                    SetAICanMove(true);
                    if (!debuff_freeze_Flag)
                    {
                        animCtrl.Freeze(false);
                    }
                }
            }
        }

        if (hurtByMiasmaCount > 0)
        {
            hurtByMiasmaTimer += deltaTime;
            if (hurtByMiasmaTimer >= ManyKnivesDefine.RoleDebuffConfig.hurtByMiasmaCD)
            {
                hurtByMiasmaTimer = 0;
                RoleTrigger(miasmaObj.transform.position, miasmaDmg, !isPlayer);
            }
        }
    }

    protected void DropBlade()
    {
        if (isBoss)
        {
            return;
        }
        //带刀刃小兵，必掉刀刃
        if (roleData.bladeType > 0)
        {
            var oriPos = transform.position;
            for (int i = 0; i < 5; i++)
            {
                //随机出现在周围
                var tmpPos = new Vector3(oriPos.x + Mathf.Lerp(-1, 1, Random.Range(0, 1)) * 1.5f,
                    oriPos.y + Mathf.Lerp(-1, 1, Random.Range(0, 1)) * 1.5f);
                var newBlade = sceneMgr.BladePoolPopOne(curBladeType);
                newBlade.Init(sceneMgr, curBladeType, this);
                newBlade.Drop(tmpPos, true);
            }
        }
    }

    protected void DropProp()
    {
        if (isPlayer || isBoss || roleData.dropConfig.Equals(null))
        {
            return;
        }

        var config = roleData.dropConfig;
        int num = config.num;
        while (num > 0)
        {
            int type = Random.Range(config.typeMin, config.typeMax);
            sceneMgr.SpawnPropPrefab(1, type, transform.position);
            num--;
        }
    }

    protected void SetDisplayFlip(bool forward)
    {
        disply.localScale = forward ? ManyKnivesDefine.RoleDir.forward : ManyKnivesDefine.RoleDir.back;
    }

    protected virtual void Deaded()
    {
        if (deadFlag)
        {
            return;
        }

        deadFlag = true;
        readyFlag = false;
        //敌人死亡时，身上有刀刃，直接回收
        if (!isPlayer)
        {
            DropBlade();
            if (bladeNumBind.value > 0)
            {
                foreach (var v in bladeList)
                {
                    v.Value.PushInPool();
                }
            }
        }

        bladeList = null;
        rigidbody.simulated = false;
        animCtrl.Play(ManyKnivesDefine.AnimatorState.die);
        animCtrl.Freeze(false);
        freezeFx.gameObject.SetActive(false);
        fx_Light.gameObject.SetActive(false);
        uiMgr.PushHpSlider(hpSlider);
        hpSlider = null;
        //死亡回收
        deadTween = DOTween.Sequence();
        deadTween.Insert(0.6f, DOVirtual.Float(0, 1, 0.5f, value => SetDissolve(value)));
        //主角不回收
        if (sceneMgr.EnemyDie(this))
        {
            deadTween.InsertCallback(1.1f, () =>
            {
                sceneMgr.RolePoolPushOne(roleName, this);
            });
        }
        RefreshPause();
    }

    void HitTween(Vector3 point, bool dieFlag)
    {
        KillHitTween();
        hitTween = DOTween.Sequence();
        hitTween.Insert(0, DOVirtual.Float(0, 1, ManyKnivesDefine.MK_Hit.fillColDur1,
            value => { SetFillPhase(value); })).SetEase(Ease.OutQuad);
        hitTween.Insert(ManyKnivesDefine.MK_Hit.fillColDur1, DOVirtual.Float(1, 0, ManyKnivesDefine.MK_Hit.fillColDur2,
            value => { SetFillPhase(value); })).SetEase(Ease.InQuad);
        if (deadFlag)
        {
            sceneMgr.audioMgr.PlayOneShot(isBoss
                ? ManyKnivesDefine.AudioClips.boss_die
                : ManyKnivesDefine.AudioClips.monster_death);
            int limitDis = 5;
            if (Vector3.Distance(transform.position, point) < limitDis)
            {
                var dir = transform.position - point;
                SetDisplayFlip(dir.x > 0);
                dir.z = 0;
                dir = dir.normalized;
                var tarPos = sceneMgr.GetSafetyPosition(point + (dir * 5.5f));
                hitTween.Insert(0, transform.DOMove(tarPos, 0.5f)).SetEase(Ease.OutQuad);
            }

            hitTween.InsertCallback(0.22f, () =>
            {
                sceneMgr.audioMgr.PlayOneShot(isBoss
                    ? ManyKnivesDefine.AudioClips.boss_die2
                    : ManyKnivesDefine.AudioClips.monster_death2);
            });
            hitTween.InsertCallback(0.5f, DropProp);
        }

        if (isPlayer)
        {
            sceneMgr.cameraCtrl.Shake_PlayerHurt();
        }
        RefreshPause();
    }

    protected virtual void UpdateHPSliderPos()
    {
        hpSlider?.RefreshPos(hpAnchor.position);
    }

    protected virtual void InitBlade(int bladeType, int bladeNum)
    {
        bladeList = new Dictionary<GameObject, BladeBase>();
        curBladeType = bladeType;
        bladeNumBind.Send(bladeNum);
        int rotation = 360 / bladeNum * ManyKnivesDefine.bladeSide;
        for (int i = 0; i < bladeNum; i++)
        {
            BladeBase tmpGO = sceneMgr.BladePoolPopOne(bladeType);
            tmpGO.transform.SetParent(bladeTran.transform);
            tmpGO.Init(sceneMgr, bladeType, this);
            int deg = (i - 1) * rotation;
            float radius = ManyKnivesDefine.bladeRadius;
            float x = radius * Mathf.Cos(deg * Mathf.Deg2Rad);
            float y = radius * Mathf.Sin(deg * Mathf.Deg2Rad);
            var localRot = Quaternion.Euler(0, 0, deg - 90);
            tmpGO.transform.localPosition = new Vector3(x, y, 0);
            tmpGO.transform.localRotation = localRot;
            tmpGO.transform.localScale = Vector3.one;
            bladeList[tmpGO.gameObject] = tmpGO;
        }
    }

    //受伤掉血
    protected void RoleTrigger(Vector3 point, float dmgValue, bool realInjury, bool bladeFlag = default)
    {
        if (deadFlag)
        {
            return;
        }
        //敌方有防御值
        if (!noInjury && !isPlayer)
        {
            dmgValue = Mathf.Max(0, dmgValue - roleData.defence);
        }

        dmgValue = Mathf.Floor(dmgValue);
        bool dieFlag = false;
        uiMgr.ShowDmgText((!bladeFlag && realInjury) ? 0 : 1, dmgValue, transform.position + new Vector3(0, dmgOffY, 0));
        if (dmgValue > 0)
        {
            var fx = sceneMgr.PopEffect(ManyKnivesDefine.SkillNames.fx_juese_shouji) as Effect_Partical_Item;
            fx.Init(sceneMgr, ManyKnivesDefine.SkillNames.fx_juese_shouji);
            var fxPos = transform.position;
            fxPos.x = fxPos.x + Mathf.Lerp(-0.2f, 0.2f, Random.Range(0, 1));
            fxPos.y = fxPos.y + Mathf.Lerp(0, dmgOffY, Random.Range(0, 1));
            fx.Play(2, fxPos, 0.5f);
            fx.transform.localScale = Vector3.one * Mathf.Lerp(0.4f, 0.6f, Random.Range(0, 1));
            fx.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0,360));
            hpValueBind.Send(bladeNumBind.value - dmgValue);
            if (bladeNumBind.value <= 0)
            {
                //死亡被击飞
                deadFlag = true;
                Deaded();
            }
        }
        HitTween(point, dieFlag);
    }
    
    protected virtual void BackUpTween(Vector3 point, float dis = default)
    {
        if (deadFlag || uncontrolled)
        {
            return;
        }
        KillDeadTween();
        var dir = transform.position - point;
        SetDisplayFlip(dir.x > 0);
        dir.z = 0;
        dir = dir.normalized;
        if (dir == null)
        {
            dis = 0.2f;
        }

        var tarPos = sceneMgr.GetSafetyPosition(transform.position + (dir * dis));
        backUpTween = transform.DOMove(tarPos, 0.1f).SetEase(Ease.OutQuad);
        if (aiPath != null)
        {
            SetAICanMove(false);
            backUpTween.OnComplete(() =>
            {
                SetAICanMove(true);
            });
        }
        RefreshPause();
    }

    protected void Stop()
    {
        if (!deadFlag)
        {
            moveSpBind.Send(0);
        }

        readyFlag = false;
        SetAICanMove(false);
    }

    //演出状态下角色无伤
    protected void NoInjury(bool value)
    {
        noInjury = value;
    }
    
    protected void KillBackUpTween()
    {
        backUpTween?.Kill();
        backUpTween = null;
    }
    protected void KillDeadTween()
    {
        if (deadTween != null)
        {
            deadTween.Kill();
            deadTween = null;
        }
    }

    void KillHitTween()
    {
        if (hitTween != null)
        {
            hitTween.Kill();
            hitTween = null;
        }
    }
    

    protected virtual void Recycle()
    {
        KillDeadTween();
        KillHitTween();
        KillBackUpTween();
        readyFlag = false;
        sceneMgr.pauseBind.Remove(PauseListener);
    }

    protected void Dispose()
    {
        KillDeadTween();
        KillHitTween();
        KillBackUpTween();
        animCtrl.Dispose();
        animCtrl = null;
        base.Dispose();
    }
}
