using DG.Tweening;
using UnityEngine;
using static ManyKnivesDefine;
public class RoleBossBase : RoleBase
{
    protected bool actionFlag;
    // boss技能计数相关配置
    // 火球
    const float CD_fire = 0.8f;
    const float CD_fire2 = 0.05f;
    // 毒气
    const int CD_miasma = 2;
    // 毒气存在时长
    const int miasma_Stay = 10;
    // 冲刺
    const int CD_rush = 3;
    // 冲刺速度
    const int rush_MoveSpeed = 10;
    // 冲刺多远
    const int rush_Limit = 15;
    // 冲刺提前1.5秒定位方向
    const float rush_DirTimerOff = 1.5f;
    // 冲刺指引的高度
    const float rushGuide_height = 3.6f;

    // 入场演出
    protected Sequence cgTween;

    int s_interval, callNum, callEnemyType, callLevel;
    float spawnTimer;
    int bladeFxFlag2;
    protected float fxTimer, fxTimer2, fxTimerCD, fxTimerCD2;

    public override void Init(GameScene scene, UIMgr uiMgr, string roleName, RoleConfigArgs configData, Vector3 spawnPos)
    {
        base.Init(scene, uiMgr, roleName, configData, spawnPos);
        isBoss = true;
        s_interval = roleData.callTime;
        callNum = roleData.callNum;
        callEnemyType = roleData.callEnemyType;
        callLevel = roleData.callLevel;
        int bladeType = 8;
        if (roleData.roleName == RoleNames.axeboss2) bladeType = 7;
        else if (roleData.roleName == RoleNames.ironboss) bladeType = 6;
        InitBlade(bladeType, roleData.bladeNum);
        spawnTimer = s_interval;
        // 行动标记
        actionFlag = false;
        fxTimer = 0;
        fxTimer2 = 0;
        fxTimerCD = 0;
        // 技能类型 1-冲刺 2-火球 3-毒瘴
        if (roleData.skillType == 1) fxTimerCD = CD_rush;
        else if (roleData.skillType == 2)
        {
            fxTimerCD = CD_fire;
            fxTimerCD2 = CD_fire2;
        }
        else if (roleData.skillType == 3) fxTimerCD = CD_miasma;
        bladeFxFlag2 = 0;
    }
    protected override void PauseListener(bool pause)
    {
        base.PauseListener(pause);
        var timeScale = pause ? 0 : 1;
        if (cgTween != null) cgTween.timeScale = timeScale;
    }
    protected override void Start()
    {
        base.Start();
        EntrancePlay(initPos);
    }

    public virtual void Ready()
    {
        fixedlyFlag = false;
        sceneMgr.RoleNoInjury(false);
        actionFlag = true;
    }

    void EntrancePlay(Vector3 pos)
    {
        KillCGTween();
        sceneMgr.RoleNoInjury(true);
        fixedlyFlag = true;
        SetAICanMove(false);
        actionFlag = false;
        animCtrl.Play(AnimatorState.entrance);
        cgTween = DOTween.Sequence();
        cgTween.InsertCallback(0.15f, () => Time.timeScale = 0.15f);
        cgTween.InsertCallback(0.24f, () =>
        {
            Time.timeScale = 1;
            sceneMgr.audioMgr.PlayOneShot(AudioClips.boss_fall);
        });
        cgTween.InsertCallback(0.3f, () =>
        {
            sceneMgr.cameraCtrl.Shake_BossEntrance();
            var fx = sceneMgr.PopEffect(SkillNames.fx_dimian) as Effect_Partical_Item;
            fx.Init(sceneMgr, SkillNames.fx_dimian);
            fx.Play(1, initPos, 2);
            sceneMgr.PlayerRepulse(initPos);
        });
        cgTween.InsertCallback(1.2f, () =>
        {
            //entity.Ready();TODO
        });
        cgTween.SetAutoKill(true);
        RefreshPause();
    }

    public override void Update()
    {
        base.Update();
        if (deadFlag || !actionFlag) return;
        // 召唤小怪
        spawnTimer += Time.deltaTime;
        if (spawnTimer > s_interval)
        {
            spawnTimer = 0;
            sceneMgr.SpawnEnemyPrefab(callNum, callEnemyType, callLevel);
        }
    }

    protected void KillCGTween()
    {
        cgTween?.Kill();
        cgTween = null;
    }

    protected override void Deaded()
    {
        base.Deaded();
        //entity.Deaded();TODO
    }

    protected override void Recycle()
    {
        KillCGTween();
        base.Recycle();
    }

    void Dispose()
    {

    }
}