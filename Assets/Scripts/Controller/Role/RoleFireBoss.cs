using UnityEngine;
using static ManyKnivesDefine;
public class RoleFireBoss : RoleBossBase
{
    // 0-进场 1-自由态 2-扇形火球 3-砸地 4-技能后摇
    public readonly EventHandler<int> moveFlagBind = new();
    float fanTimer, circleTimer, attentTimer, waitTimer;
    FireConfigArgs fireConfig;
    // 扇形区域
    Sprite_Renderer_Item fanGuide;
    Vector3 fanDir;
    int fireBossDmg;
    // 圆形区域
    Sprite_Renderer_Item circleGuide;
    Vector3 circlePos;
    bool circleFlag;
    int circleBossDmg;
    ParticleSystem fx_rushReady;
    public override void InitComponent()
    {
        base.InitComponent();
        fx_rushReady = Util.GetComponent<ParticleSystem>(gameObject, "effect/fx_boss_chongci_xuli");
        moveFlagBind.Add(MoveFlagListener);
    }

    public override void Init(GameScene scene, UIMgr uiMgr, string roleName, RoleConfigArgs configData, Vector3 spawnPos)
    {
        base.Init(scene, uiMgr, roleName, configData, spawnPos);
        fireConfig = sceneMgr.GetFireConfigById(roleData.skillLevel);
        fireBossDmg = fireConfig.fan_fireDmg;
        circleBossDmg = fireConfig.fall_Dmg;
        moveFlagBind.Send(0);
    }

    void MoveFlagListener(int value)
    {
        if (value == 0)
        {
            fixedlyFlag = true;
            collision.enabled = false;
            SetAICanMove(false);
            KillBackUpTween();
            uncontrolled = true;
            fx_rushReady.gameObject.SetActive(false);
        }
        else if (value == 1 || value == 4)
        {
            fixedlyFlag = false;
            collision.enabled = true;
            SetAICanMove(true);
            uncontrolled = false;
            animCtrl.Play(AnimatorState.walk);
            fx_rushReady.gameObject.SetActive(false);
        }
        else
        {
            fixedlyFlag = true;
            KillBackUpTween();
            uncontrolled = true;
            collision.enabled = false;
            SetAICanMove(false);
            animCtrl.Play(AnimatorState.idle);
            fx_rushReady.Simulate(0, true);
            fx_rushReady.Play();
            fx_rushReady.gameObject.SetActive(true);
        }
    }
    public override void Ready()
    {
        base.Ready();
        moveFlagBind.Send(1);
        waitTimer = 0;
        fanTimer = 0;
        circleTimer = 0;
    }
    public override void Update()
    {
        base.Update();
        if (deadFlag || !actionFlag) return;
        var deltaTime = Time.deltaTime;
        var value = moveFlagBind.value;
        if (value == 1)
        {
            MoveFollow();
            fanTimer += deltaTime;
            circleTimer += deltaTime;
            if (fireConfig.fan_Enable && fanTimer >= fireConfig.fan_SkillCD)
            {
                moveFlagBind.Send(2);
                attentTimer = 0;
            }
            else if (fireConfig.fall_Enable && circleTimer >= fireConfig.fall_SkillCD)
            {
                moveFlagBind.Send(3);
                attentTimer = 0;
                circleFlag = false;
            }
        }
        else if (value == 4)
        {
            MoveFollow();
            waitTimer += deltaTime;
            if (waitTimer >= fireConfig.followCD) moveFlagBind.Send(1);
        }
        else if (!(debuff_freeze_Flag || debuff_light_Flag))
        {
            attentTimer += deltaTime;
            if (value == 2)
            {
                if (fanGuide == null)
                {
                    fanDir = (player.rigidbody.position - rigidbody.position).normalized;
                    fanGuide = sceneMgr.PopEffect(SkillNames.fanGuide) as Sprite_Renderer_Item;
                    fanGuide.Init(sceneMgr, SkillNames.fanGuide);
                    var angle = Vector3.Angle(fanDir, Vector3.right);
                    if (fanDir.y < 0) angle = -angle;
                    angle -= fireConfig.fan_Angle * 0.5f;
                    fanGuide.Play(2, fireConfig.fan_Angle, new Vector2(fireConfig.fan_FireDis * 2, fireConfig.fan_FireDis * 2), bladeTran.position, Quaternion.Euler(0, 0, angle));
                }
                // Fire!!!
                if (attentTimer >= fireConfig.fan_Attent)
                {
                    sceneMgr.audioMgr.PlayOneShot(AudioClips.boss_fireball);
                    // 每个火球角度
                    var perDeg = (float)fireConfig.fan_Angle / (fireConfig.fan_FireNum - 1);
                    var startAngle = fanGuide.transform.eulerAngles.z;
                    var startPos = bladeTran.position;
                    for (int i = 0; i < fireConfig.fan_FireNum; ++i)
                    {
                        var offDeg = i * perDeg + startAngle;
                        var fireBall = sceneMgr.PopEffect(SkillNames.fx_fire_boss) as EffectBase;
                        fireBall.gameObject.name = TriggerType.effect + Names.split + Names.Effect + Names.split + EffectType.fire;
                        fireBall.Init(sceneMgr, SkillNames.fx_fire_boss, this);
                        var x = Mathf.Cos(Mathf.Rad2Deg * offDeg);
                        var y = Mathf.Sin(Mathf.Rad2Deg * offDeg);
                        var dir = new Vector3(x, y, 0).normalized;
                        fireBall.Play(startPos + dir, dir, fireConfig.fan_FireDis, 10, -90);
                    }
                    moveFlagBind.Send(4);
                    waitTimer = 0;
                    fanTimer = 0;
                    HideFanGuide();
                }
                else fanGuide?.SetFill(attentTimer / fireConfig.fan_Attent);
            }
            else if (value == 3)
            {
                if (!circleFlag && circleGuide == null)
                {
                    circlePos = sceneMgr.GetSafetyPosition(player.transform.position);
                    circleGuide = sceneMgr.PopEffect(SkillNames.circleGuide) as Sprite_Renderer_Item;
                    circleGuide.Init(sceneMgr, SkillNames.circleGuide);
                    //circleGuide.Play(3, )
                }
            }
        }

    }

    void HideFanGuide()
    {
        fanGuide?.PushInPool();
        fanGuide = null;
    }
    void HideCircleGuide()
    {
        circleGuide?.PushInPool();
        circleGuide = null;
    }
}