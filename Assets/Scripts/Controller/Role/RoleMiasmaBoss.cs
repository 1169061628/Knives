using System.Collections.Generic;
using UnityEngine;
using static ManyKnivesDefine;

public class RoleMiasmaBoss : RoleBossBase
{
    MiasmaConfigArgs miasmaConfig;
    float pathTimer, rangeTimer, rangeAttentTimer;
    bool rangeAttentFlag;
    readonly List<Sprite_Renderer_Item> rangeGuideList = new();

    int miasmaDmg;

    Vector3 vecZero = Vector3.zero;
    Vector3 vecOne = Vector3.one;

    public override void Init(GameScene scene, UIMgr uiMgr, string roleName, RoleConfigArgs configData, Vector3 spawnPos)
    {
        base.Init(scene, uiMgr, roleName, configData, spawnPos);
        miasmaConfig = sceneMgr.GetMiasmaConfigById(roleData.skillLevel);
        miasmaDmg = miasmaConfig.dmg;
    }
    protected override void Start()
    {
        base.Start();
    }
    public override void Ready()
    {
        base.Ready();
        pathTimer = 0;
        rangeTimer = 0;
        rangeAttentFlag = false;
        animCtrl.Play(AnimatorState.walk);
        SetAICanMove(true);
    }
    public override void Update()
    {
        base.Update();
        if (deadFlag || !actionFlag) return;
        var deltaTime = Time.deltaTime;
        MoveFollow();
        if (miasmaConfig.path_Enable)
        {
            pathTimer += deltaTime;
            if (pathTimer > miasmaConfig.path_CD)
            {
                pathTimer = 0;
                var fx = sceneMgr.PopEffect(SkillNames.fx_miasma_boss) as EffectBase;
                fx.Init(sceneMgr, SkillNames.fx_miasma_boss, this);
                fx.gameObject.name = TriggerType.effect + Names.split + Names.Effect + Names.split + EffectType.miasma;
                fx.Play(transform.position, miasmaConfig.path_ExitTime);
            }
        }
        if (miasmaConfig.range_Enable)
        {
            if (!rangeAttentFlag)
            {
                rangeTimer += deltaTime;
                if (rangeTimer >= miasmaConfig.range_CD)
                {
                    rangeAttentFlag = true;
                    rangeAttentTimer = 0;
                    // 选取屏幕中的位置
                    var bound = new Vector3(miasmaConfig.range_Range * 0.5f, miasmaConfig.range_Range * 0.5f);
                    var viewMin = sceneMgr.cameraCtrl.ViewportToWorldPoint(vecOne) + bound;
                    var viewMax = sceneMgr.cameraCtrl.ViewportToWorldPoint(vecOne) - bound;
                    var tmpPos = new Vector3(Mathf.Lerp(viewMin.x, viewMax.x, Util.Random01f()), Mathf.Lerp(viewMin.y, viewMax.y, Util.Random01f()));
                    rangeGuideList.Clear();
                    var perRange = miasmaConfig.range_Range * 0.5f;
                    for (int i = 0; i < miasmaConfig.range_Num; ++i)
                    {
                        var newPos = tmpPos + new Vector3(Mathf.Lerp(-perRange, perRange, Util.Random01f()), Mathf.Lerp(-perRange, perRange, Util.Random01f()));
                        newPos = sceneMgr.GetSafetyPosition(newPos);
                        var circleGuide = sceneMgr.PopEffect(SkillNames.circleGuide) as Sprite_Renderer_Item;
                        circleGuide.Init(sceneMgr, SkillNames.circleGuide);
                        circleGuide.Play(3, 0, new Vector2(miasmaConfig.range_PerRange, miasmaConfig.range_PerRange), newPos, Quaternion.identity);
                        rangeGuideList.Add(circleGuide);
                    }
                }
            }
            else
            {
                rangeAttentTimer += deltaTime;
                if (rangeAttentTimer >= miasmaConfig.range_Attent)
                {
                    foreach(var v in rangeGuideList)
                    {
                        var fx = sceneMgr.PopEffect(SkillNames.fx_miasma_boss) as EffectBase;
                        fx.Init(sceneMgr, SkillNames.fx_miasma_boss, this);
                        fx.gameObject.name = TriggerType.effect + Names.split + Names.Effect + Names.split + EffectType.miasma;
                        fx.Play(transform.position, miasmaConfig.range_ExitTime, miasmaConfig.range_PerRange / 6.8f);
                    }
                    HideRangeGuide();
                    rangeTimer = 0;
                    rangeAttentFlag = false;
                }
                if (rangeGuideList.Count > 0)
                {
                    var process = rangeAttentTimer / miasmaConfig.range_Attent;
                    for (int i = 0; i < rangeGuideList.Count; i++)
                    {
                        rangeGuideList[i].SetFill(process);
                    }
                }
            }
        }
    }
    void HideRangeGuide()
    {
        for (int i = 0; i < rangeGuideList.Count; i++)
        {
            rangeGuideList[i].PushInPool();
        }
        rangeGuideList.Clear();
    }
    protected override void Deaded()
    {
        base.Deaded();
    }
    protected override void Recycle()
    {
        HideRangeGuide();
        base.Recycle();
    }
}