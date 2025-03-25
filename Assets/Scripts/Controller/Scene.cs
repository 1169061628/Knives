using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public struct LevelConfigArgs
{
    public int waveId;
    public int time;
    public int propType;
    public int propNum;
    public int enemyType;
    public int enemyNum;
    public int enemyLevel;
}

public struct RoleConfigArgs
{
    public int level;
    public string name;
    public int hp;
    public int bladeType;
    public int bladeNum;
    public int speed;
    public int bodyDmg;
    public int bladeDmg;
    public int skillDmg;
    public int defence;
    public int skillType;
    public int callEnemyType;
    public int callTime;
    public int callNum;
    public int callLevel;
    public int skillLevel;
}

public class Scene
{
    public bool isPause;
    public EventHandler<bool> OnPauseStateChange = new();

    public AudioSource AS_BGM;
    public AudioSource AS_FX;
    public AudioSource AS_FX_Loop_Craze;

    // 刀刃的对象池
    readonly Dictionary<string, KnifeObjectPool<BladeBase>> bladePool = new();
    // 道具的对象池
    readonly Dictionary<string, KnifeObjectPool<PropBase>> propPool = new();
    // 角色对象池
    readonly Dictionary<string, KnifeObjectPool<RoleBase>> rolePool = new();
    // 特效对象池
    readonly Dictionary<string, KnifeObjectPool<EffectBase>> effectPool = new();

    Camera mainCamera;
    SpriteRenderer bgSR;
    AstarPath pathFinder;
    Transform playerPos;

    RoleBase rolePlayer;
    bool readyFlag;

    CameraCtrl cameraCtrl;

    // 记录关卡生成波次
    public readonly EventHandler<int> levelWaveBind = new();
    int levelWaveMax;
    // 关卡配置
    readonly Dictionary<int, LevelConfigArgs> levelConfigTable = new();
    // 全部角色配置
    readonly Dictionary<string, Dictionary<int, RoleConfigArgs>> roleConfigTable = new();
    // 生成关卡内容计时器
    float spawnTimer;
    // 关卡敌人总数量
    int enemyNumMax;
    // 记录已经击杀数量
    readonly EventHandler<int> enemyNumBind = new();
    // 是否boss关卡
    public bool bossFlag;
    // 记录当前boss类型
    int curBossType;
    int curBossLevel;
    int curBossRetinueNum;
    // 记录当前关卡有哪些敌人，对象池只创建需要的敌人
    readonly List<int> curEnemyTypeList = new();
    // boss生成时间
    float bossTimer;
    // boss倒计时
    readonly EventHandler<float> bossTimerBind = new();
    // 演出状态下，所有角色无伤
    public bool NoInjury;
    // 游戏结束标记
    public bool overFlag;
    // 随时间生成刀刃
    float soawnBladeTimer;
    int bladeCount = 0;

    // 记录所有自动掉落的刀刃
    readonly List<BladeBase> autoDropBladeList = new();
    (int initNum, int maxNum) autoBladeConfig;

    // 自动生成的刀刃回收检测CD
    const float autoBladeCheckCD = 0.5f;
    const int spawnViewMaxOff = 5;
    const int spawnViewMinOff = 1;
    const int mapBound = 2;
    // 每帧最多生成多少
    const int spawnCountPerFreme = 5;

    GameMgr gameMgr;
    UIMgr uiMgr;
    AudioMgr audioMgr;

    Tween cgTween;

    readonly Dictionary<GameObject, RoleBase> rolePairWithGO = new();
    readonly Dictionary<GameObject, PropBase> propPairWithGO = new();


    public GameObject levelRoot;

    public void AddBlade()
    {
        bladeCount++;
    }

    public void Init()
    {
        gameMgr = new();
        uiMgr = new();
        audioMgr = new();
        OnPauseStateChange.Add(val =>
        {
            isPause = val;
            PauseListener(val);
        });
        readyFlag = false;
    }
    void RefreshPause()
    {

    }

    void PauseListener(bool pause)
    {
        var timeScale = pause ? 0 : 1;
        if (cgTween != null) cgTween.timeScale = timeScale;
    }

    void Update()
    {
        if (isPause) return;
        if (readyFlag)
        {
            foreach(var item in rolePairWithGO)
            {
                item.Value.Update();
            }
        }
    }

    void StartSpawn()
    {
        if (overFlag) return;
        if (levelWaveBind.value > levelWaveMax) return;
        if (!levelConfigTable.TryGetValue(levelWaveBind.value, out var curWaveData)) return;
        var deltaTime = Time.deltaTime;
        spawnTimer += deltaTime;
        bossTimerBind.Send(bossTimerBind.value - deltaTime);
        if (spawnTimer >= curWaveData.time)
        {
            // 生成道具
            if (curWaveData.propNum != 0) SpawnPropPrefab(curWaveData.propNum, curWaveData.propType);
            // 生成敌人
            if (curWaveData.enemyNum != 0) SpawnEnemyPrefab(curWaveData.enemyNum, curWaveData.enemyType, curWaveData.enemyLevel);
            levelWaveBind.Send(levelWaveBind.value + 1);
        }
    }
    // 生产道具对象
    void SpawnPropPrefab(int num, int type, Vector3 pos = default)
    {
        var spawnPosFlag = pos == default;
        for (int i = 0; i < num; ++i)
        {
            var temGo = PropPoolPopOne(type);
            temGo.transform.SetParent(levelRoot.transform);
            temGo.Init(this, type);
            temGo.gameObject.name = ManyKnivesDefine.TriggerType.prop + ManyKnivesDefine.Names.split + ManyKnivesDefine.Names.Prop;

            Vector3 tmpPos;
            // 随机出现在周围
            if (spawnPosFlag) tmpPos = GetPropSpawnPos();
            else
            {
                var random = new System.Random();
                tmpPos = pos;
                tmpPos.x += Mathf.Lerp(-1, 1, (float)random.NextDouble()) * 1.5f;
                tmpPos.y += Mathf.Lerp(-1, 1, (float)random.NextDouble()) * 1.5f;
            }
            temGo.transform.position = tmpPos;
        }
    }

    // 生产敌人对象
    void SpawnEnemyPrefab(int num, int type, int level)
    {
        var roleName = ManyKnivesDefine.roleTypeWithName[type];
        // boss入场
        if (type >= 7 && type <= 9)
        {
            uiMgr.BossCGPlay(() =>
            {
                var spawnPos = new Vector3(0.5f, 0.7f, 0);
                spawnPos = cameraCtrl.ViewportToWorldPoint(spawnPos);
                spawnPos.z = 0;
                SpawnEnemy(roleName, num, level, spawnPos);
            });
        }
        else SpawnEnemy(roleName, num, level);
    }

    RoleConfigArgs GetRoleConfig(string roleName, int levelOrId)
    {
        return roleConfigTable[roleName][levelOrId];
    }

    void SpawnEnemy(string roleName, int num, int level, Vector3 spawnPos = default)
    {
        for (int i = 0;i < num; ++i)
        {
            var tmpGo = RolePoolPopOne(roleName);
            tmpGo.transform.SetParent(levelRoot.transform);
            tmpGo.Init(this, uiMgr, roleName, GetRoleConfig(roleName, level), GetRoleSpawnPos(spawnPos));
            tmpGo.gameObject.name = ManyKnivesDefine.TriggerType.role + ManyKnivesDefine.Names.split + ManyKnivesDefine.Names.enemy;
        }
    }

    RoleBase RolePoolPopOne(string roleName)
    {
        var obj = rolePool[roleName].Get();
        rolePairWithGO[obj.gameObject] = obj;
        return obj;
    }


    Vector3 GetPropSpawnPos()
    {
        var boundMin = 0.05f;
        var boundMax = 0.3f;
        // 1/4屏死随机位置
        var random = new System.Random();
        float rz = (float)new System.Random().NextDouble();
        var rPos = new Vector3(Mathf.Lerp(boundMin, boundMax, (float)random.NextDouble()), Mathf.Lerp(boundMin, boundMax, (float)random.NextDouble()), 0);
        var spawnDir = (float) random.NextDouble();
        // 左下角
        if (spawnDir < 0.25f) { }
        else if (spawnDir < 0.5f) // 左上角
            rPos.y = 1 - rPos.y;
        else if (spawnDir < 0.75f)  // 右下角
            rPos.x = 1 - rPos.x;
        else  // 右上角
        {
            rPos.x = 1 - rPos.x;
            rPos.y = 1 - rPos.y;
        }
        rPos = cameraCtrl.ViewportToWorldPoint(rPos);
        rPos.z = 0;
        rPos = GetSafetyPosition(rPos);
        rPos.z = 0;
        return rPos;
    }

    Vector3 GetRoleSpawnPos(Vector3 spawnPos = default)
    {
        if (spawnPos == default)
        {
            var viewMin = cameraCtrl.ViewportToWorldPoint(Vector2.zero);
            var viewMax = cameraCtrl.ViewportToWorldPoint(Vector2.one);
            Dictionary<int, int> sideList = new();
            var sideNum = 0;
            // 左边可用
            if (viewMin.x > cameraCtrl.mapLeftBound + mapBound)
            {
                sideNum++;
                sideList[sideNum] = 1;
                sideNum++;
                sideList[sideNum] = 1;
                sideNum++;
                sideList[sideNum] = 1;
                sideNum++;
                sideList[sideNum] = 1;
            }
            // 下方可用
            if (viewMin.y > cameraCtrl.mapBottomBound + mapBound)
            {
                sideNum++;
                sideList[sideNum] = 2;
            }
            // 右方可用
            if (viewMax.x < cameraCtrl.mapRightBound - mapBound)
            {
                sideNum++;
                sideList[sideNum] = 3;
                sideNum++;
                sideList[sideNum] = 3;
                sideNum++;
                sideList[sideNum] = 3;
                sideNum++;
                sideList[sideNum] = 3;
            }
            // 上方可用
            if (viewMax.y < cameraCtrl.mapTopBound - mapBound)
            {
                sideNum++;
                sideList[sideNum] = 4;
            }
            if (sideNum < 1)
            {
                sideList = new() { [1] = 1, [2] = 1, [3] = 1, [4] = 1, [5] = 2, [6] = 3, [7] = 3, [8] = 3, [9] = 3, [10] = 4 };
                sideNum = sideList.Count;
            }
            var spawnDir = sideList[Random.Range(1, sideNum + 1)];
            // 上方
            var random = new System.Random();
            if (spawnDir == 4) spawnPos = new Vector3(Mathf.Lerp(viewMin.x, viewMax.x, (float)random.NextDouble()), viewMax.y + mapBound);
            else if (spawnDir == 2) spawnPos = new Vector3(Mathf.Lerp(viewMin.x, viewMax.x, (float)random.NextDouble()), viewMin.y - mapBound);
            else if (spawnDir == 1) spawnPos = new Vector3(viewMin.x - mapBound, Mathf.Lerp(viewMin.y, viewMax.y, (float)random.NextDouble()));
            else spawnPos = new Vector3(viewMax.x - mapBound, Mathf.Lerp(viewMin.y, viewMax.y, (float)random.NextDouble()));
        }
        spawnPos = GetSafetyPosition(spawnPos);
        spawnPos.z = 0;
        return spawnPos;
    }

    Vector3 GetSafetyPosition(Vector3 pos)
    {
        return pathFinder.GetNearest(pos, Pathfinding.NNConstraint.Default).position;
    }


    KnifeObjectPool<PropBase> GetPropPool(int type)
    {
        return type switch
        {
            1 => propPool[ManyKnivesDefine.PropNames.defaultBladeProp],
            2 => propPool[ManyKnivesDefine.PropNames.iceBladeProp],
            3 => propPool[ManyKnivesDefine.PropNames.fireBladeProp],
            4 => propPool[ManyKnivesDefine.PropNames.miasmaBladeProp],
            5 => propPool[ManyKnivesDefine.PropNames.lightningBladeProp],
            6 => propPool[ManyKnivesDefine.PropNames.crazeProp],
            7 => propPool[ManyKnivesDefine.PropNames.swordProp],
            8 => propPool[ManyKnivesDefine.PropNames.fleetfootedProp],
            9 => propPool[ManyKnivesDefine.PropNames.loveProp],
            10 => propPool[ManyKnivesDefine.PropNames.defaultBladeProp2],
            _ => null
        };
    }
    PropBase PropPoolPopOne(int type)
    {
        var obj = GetPropPool(type).Get();
        propPairWithGO[obj.gameObject] = obj;
        return obj;
    }
}
