using UnityEngine;
using static ManyKnivesDefine;

public class RoleRushBoss : RoleBossBase
{
    // 冲刺
    Transform rushCenter;
    // 冲刺标记
    bool rushFlag;
    Sprite_Renderer_Item rushGuide;
    ParticleSystem fx_rushReady;
    // 冲刺配置
    RushConfigArgs rushConfig;
    // 1-闲逛 2-准备冲 3-冲刺状态
    public readonly EventHandler<int> rushFlagBind = new();
    // 1-技能冷区中 2-释放技能中
    int moveFlag;
    public readonly EventHandler<bool> furyFlagBind = new();
    // 冲刺伤害
    int rushBossDmg;

    float rushGuide_height = 3.6f;

    ParticleSystem fx_Fury, fx_Fury_burst;

    float skillTimer, rushTimer, moveDis;
    int rushCount;
    Vector3 moveDir;

    public override void InitComponent()
    {
        base.InitComponent();
        rushCenter = Util.GetTransform(gameObject, "effect/rushCenter");
        fx_rushReady = Util.GetComponent<ParticleSystem>(gameObject, "effect/fx_boss_chongci_xuli");
        fx_Fury = Util.GetComponent<ParticleSystem>(gameObject, "effct/fx_boss_erjieduan");
        fx_Fury_burst = Util.GetComponent<ParticleSystem>(gameObject, "effct/fx_boss_erjieduan_baofa");
        rushFlagBind.Add(MoveFlagListener);
        furyFlagBind.Add(FuryListener);
        hpValueBind.Add(HpListener);
    }

    void MoveFlagListener(int value)
    {
        switch (value)
        {
            case 0:
                {
                    fixedlyFlag = true;
                    rushCenter.gameObject.SetActive(false);
                    fx_rushReady.gameObject.SetActive(false);
                    collision.enabled = true;
                    SetAICanMove(false);
                    uncontrolled = false;
                    rushFlag = false;
                    break;
                }
            case 1:
                {
                    fixedlyFlag = false;
                    rushCenter.gameObject.SetActive(false);
                    fx_rushReady.gameObject.SetActive(false);
                    collision.enabled = true;
                    SetAICanMove(true);
                    animCtrl.Play(AnimatorState.walk);
                    uncontrolled = false;
                    rushFlag = false;
                    break;
                }
            case 2:
                {
                    fixedlyFlag = true;
                    rushCenter.gameObject.SetActive(false);
                    fx_rushReady.gameObject.SetActive(true);
                    collision.enabled = true;
                    SetAICanMove(false);
                    animCtrl.Play(AnimatorState.idle);
                    KillBackUpTween();
                    uncontrolled = true;
                    rushFlag = false;
                    break;
                }
            default:
                {
                    fixedlyFlag = true;
                    SetAICanMove(false);
                    collision.enabled = false;
                    animCtrl.Play(AnimatorState.walk);
                    rushCenter.gameObject.SetActive(true);
                    fx_rushReady.gameObject.SetActive(false);
                    uncontrolled = true;
                    rushFlag = true;
                    break;
                }
        }
    }

    public override void Init(GameScene scene, UIMgr uiMgr, string roleName, RoleConfigArgs configData, Vector3 spawnPos)
    {
        base.Init(scene, uiMgr, roleName, configData, spawnPos);
        rushConfig = sceneMgr.GetRushConfigById(roleData.skillLevel);
        rushBossDmg = rushConfig.rush_Dmg;
    }

    protected override void Start()
    {
        furyFlagBind.Send(false);
        rushFlagBind.Send(0);
        base.Start();
    }

    public override void Ready()
    {
        base.Ready();
        rushFlagBind.Send(1);
        moveFlag = 1;
        skillTimer = 0;
        rushTimer = 0;
        rushCount = 0;
        moveDir = default;
        HpListener(0f);
    }

    void FuryListener(bool value)
    {
        if (value )
        {
            fx_Fury.Simulate(0, true);
            fx_Fury.Play();
            fx_Fury_burst.Simulate(0, true);
            fx_Fury_burst.Play();
        }
        fx_Fury.gameObject.SetActive(value);
        fx_Fury_burst.gameObject.SetActive(value);
        if (actionFlag && value)
        {
            var roleName = RoleNames.axeboss2;
            var roleLevel = rushConfig.retinueLevel;
            var centerPos = transform.position;
            // 生成几个分身
            for (int i = 1; i < rushConfig.retinueNum; ++i)
            {
                var tmpRole = sceneMgr.RolePoolPopOne(roleName);
                tmpRole.transform.SetParent(sceneMgr.levelRoot.transform);
                //  生成在3米外
                var random = new System.Random();
                var dir = new Vector3(Mathf.Lerp(-1, 1, (float)random.NextDouble()), Mathf.Lerp(-1, 1, (float)random.NextDouble())).normalized;
                var newPos = sceneMgr.GetSafetyPosition(centerPos + dir * 3);
                tmpRole.Init(sceneMgr, sceneMgr.uiMgr, roleName, sceneMgr.GetRoleConfig(roleName, roleLevel), newPos);
                tmpRole.gameObject.name = TriggerType.role + Names.split + Names.enemy;
            }
            sceneMgr.PlayerRepulse(centerPos);
        }
    }

    void HpListener(float _ = 0)
    {
        if (actionFlag && !furyFlagBind.value && rushConfig.retinue_Enable)
        {
            // 转阶段
            if ((float)hpValueBind.value / hpMaxBind.value * 100 <= rushConfig.transLimit) furyFlagBind.Send(true);
        }
    }
    void HpListener(int _ = 0) => HpListener(0f);

    public override void Update()
    {
        base.Update();
        if (deadFlag || !actionFlag) return;
        var deltaTime = Time.deltaTime;
        // 技能冷却中
        if (moveFlag == 1)
        {
            MoveFollow();
            if (rushConfig.rush_Enable)
            {
                skillTimer += deltaTime;
                if (skillTimer > rushConfig.skillCD)
                {
                    rushFlagBind.Send(2);
                    rushTimer = 0;
                    moveFlag = 2;
                    rushCount = 0;
                }
            }
        }
        else if (!(debuff_freeze_Flag || debuff_light_Flag))
        {
            // 冲刺间隔闲逛
            if (rushFlagBind.value == 1)
            {
                rushTimer += deltaTime;
                if (rushTimer >= rushConfig.rush_interval)
                {
                    rushFlagBind.Send(2);
                    rushTimer = 0;
                }
            }
            // 准备冲
            else if (rushFlagBind.value == 2)
            {
                rushTimer += deltaTime;
                if (rushGuide == null)
                {
                    fx_rushReady.Simulate(0, true);
                    fx_rushReady.Play();
                    fx_rushReady.gameObject.SetActive(true);
                    moveDir = (player.rigidbody.position - rigidbody.position).normalized;
                    rushGuide = sceneMgr.PopEffect(SkillNames.rushGuide) as Sprite_Renderer_Item;
                    rushGuide.Init(sceneMgr, SkillNames.rushGuide);
                    var angle = Vector3.Angle(moveDir, Vector3.right);
                    if (moveDir.y < 0) angle = -angle;
                    rushGuide.Play(1, 0, new Vector2(rushConfig.rush_Distance, rushGuide_height), bladeTran.position, Quaternion.Euler(0, 0, angle));
                }
                var attentCD = rushCount == 0 ? rushConfig.rush_attent_First : rushConfig.rush_attent;
                if (rushTimer >= attentCD)
                {
                    rushFlagBind.Send(3);
                    moveDis = 0;
                    HideRushGuide();
                    var deg = Vector2.Angle((Vector2)moveDir, Vector2.right);
                    if (moveDir.y < 0) deg = -deg;
                    rushCenter.localRotation = Quaternion.Euler(0, 0, deg);
                    sceneMgr.audioMgr.PlayOneShot(AudioClips.boss_dash);
                }
                rushGuide?.SetFill(rushTimer / attentCD);
            }
            else
            {
                curMoveSp = rushConfig.rush_MoveSpeed;
                moveSpBind.Send(curMoveSp);
                SetDisplayFlip(moveDir.x < 0);
                var tmpVec2Pos = (Vector2)transform.position;
                tmpVec2Pos = sceneMgr.cameraCtrl.LimitPosInMap(tmpVec2Pos + (Vector2)moveDir * (moveSpBind.value * deltaTime));
                transform.position = (Vector3)tmpVec2Pos;
                moveDis = moveDis + (moveDir * moveSpBind.value).magnitude * deltaTime;
                if (moveDis >= rushConfig.rush_Distance)
                {
                    curMoveSp = roleData.speed;
                    moveSpBind.Send(curMoveSp);
                    rushCount++;
                    if (rushCount >= rushConfig.rushCount)
                    {
                        moveFlag = 1;
                        skillTimer = 0;
                        var safetyPos = sceneMgr.GetSafetyPosition(transform.position);
                        transform.position = safetyPos;
                    }
                    rushTimer = 0;
                    rushFlagBind.Send(1);
                }
            }
        }
    }

    void HideRushGuide()
    {
        rushGuide?.PushInPool();
        rushGuide = null;
    }
    protected override void Recycle()
    {
        rushCenter.gameObject.SetActive(false);
        fx_rushReady.gameObject.SetActive(false);
        fx_Fury.gameObject.SetActive(false);
        fx_Fury_burst.gameObject.SetActive(false);
        HideRushGuide();
        base.Recycle();
    }
}