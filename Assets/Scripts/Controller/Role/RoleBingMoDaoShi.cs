using UnityEngine;
using static ManyKnivesDefine;
public class RoleBingMoDaoShi : RoleBase
{
    const string shotPoolName = "iceShot";
    const int shotSpeed = 15;
    const int shotDistance = 30;
    const int shotLimit = 13;
    const int runDisLimit = 8;

    // 发射子弹的位置
    Transform shotPos;
    bool changeActionFlag;

    float atkTimerCD, atkTimer;
    float idleTimer, idleTimerCD;
    int actionState;// 1-移动 2-攻击
    public override void InitComponent()
    {
        base.InitComponent();
        shotPos = Util.GetTransform(gameObject, "display/edit_Bingmodaoshi/Bone/Main/Zhixin/Body/R_Arm/R_Arm1/R_Arm2/Fazhang/shotPos");
    }
    public override void Init(GameScene scene, UIMgr uiMgr, string roleName, RoleConfigArgs configData, Vector3 spawnPos)
    {
        base.Init(scene, uiMgr, roleName, configData, spawnPos);
        atkSpBind.Send(1);
        atkTimerCD = 2.3f;
        atkTimer = atkTimerCD;
        idleTimer = 0;
        idleTimerCD = 0.5f;
        actionState = 1;
        changeActionFlag = false;
    }
    public override void Update()
    {
        base.Update();
        if (deadFlag) return;
        var deltaTime = Time.deltaTime;
        atkTimer += deltaTime;
        // 距离玩家15m时才发射子弹
        var disFormPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (actionState == 2)
        {
            SetAICanMove(false);
            animCtrl.Play(AnimatorState.attack);
            idleTimer += deltaTime;
            if (idleTimer > idleTimerCD)
            {
                actionState = 1;
                idleTimer = 0;
            }
        }
        else
        {
            animCtrl.Play(AnimatorState.walk);
            // 距离玩家大于15m，往玩家方向走
            if (disFormPlayer > shotLimit)
            {
                SetAICanMove(true);
                MoveFollow();
                if (changeActionFlag) aiPath.SearchPath();
                changeActionFlag = true;
            }
            else if (disFormPlayer < runDisLimit && !(debuff_freeze_Flag || debuff_light_Flag)) // 小于10m，逃跑
            {
                SetAICanMove(true);
                var dir = (transform.position - player.transform.position).normalized;
                moveOffTimer += deltaTime;
                if (moveOffTimer >= RoleMoveOff.offTimeCD)
                {
                    moveOffTimer = 0;
                    float r2 = Random.Range((float)0, 2);
                    r2 = r2 < 0.5 ? -1 : 1;
                    moveOffRot = Quaternion.Euler(0, 0, Random.Range(RoleMoveOff.offAngleMin, RoleMoveOff.offAngleMax + 1) * r2);
                    dir = (moveOffRot * new Vector3(dir.x, dir.y, 0)).normalized;
                }
                var tarPos = transform.position + dir * (runDisLimit * 2);
                tarPos = sceneMgr.GetSafetyPosition(tarPos);
                aiPath.destination = tarPos;
                RefreshDisplayFlip();
                if (changeActionFlag)
                {
                    aiPath.SearchPath();
                    changeActionFlag = false;
                }
            }
            else if (disFormPlayer <= shotLimit && !debuff_freeze_Flag && atkTimer >= atkTimerCD)
            {
                SetAICanMove(false);
                changeActionFlag = true;
                // 射程范围内
                actionState = 2;
                atkTimer = 0;
                var dir = player.rigidbody.position - rigidbody.position;
                SetDisplayFlip(dir.x < 0);
                //var shotDir = (player.center.position - shotPos.position).normalized;TODO
                var shotDir = Vector3.one;
                shotDir.z = 0;
                var startPos = shotPos.position;
                var shotObj = sceneMgr.PopEffect(SkillNames.fx_bingzhangjineng) as EffectBase;
                shotObj.gameObject.name = TriggerType.effect + Names.split + Names.Effect + Names.split + EffectType.enemyBall;
                shotObj.Init(sceneMgr, SkillNames.fx_bingzhangjineng, this);
                shotObj.Play(startPos, shotDir, 20, 10, -90);
            }
        }
    }
}