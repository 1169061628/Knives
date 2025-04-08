using DG.Tweening;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ManyKnivesDefine;
public struct LevelConfigArgs
{
    public int waveId;
    public int time;
    public int propType;
    public int propNum;
    public int enemyType;
    public int enemyNum;
    public int enemyLevel;
    public LevelConfigArgs(List<int> datas)
    {
        waveId = datas[0];
        time = datas[1];
        propType = datas[2];
        propNum = datas[3];
        enemyType = datas[4];
        enemyNum = datas[5];
        enemyLevel = datas[6];
    }
}

public struct RoleConfigArgs
{
    public int level;
    public string roleName;
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
    public (int num, int typeMin, int typeMax) dropConfig;
    public RoleConfigArgs(string name, List<int> datas, (int num, int typeMin, int typeMax) drop)
    {
        var len = datas.Count;
        level = datas[0];
        roleName = name;
        hp = datas[1];
        bladeType = datas[2];
        bladeNum = datas[3];
        speed = datas[4];
        bodyDmg = datas[5];
        bladeDmg = datas[6];
        skillDmg = datas[7];
        defence = datas[8];
        skillType = len > 10 ? datas[10] : 0;
        callEnemyType = len > 11 ? datas[11] : 0;
        callTime = len > 12 ? datas[12] : 0;
        callNum = len > 13 ? datas[13] : 0;
        callLevel = len > 14 ? datas[14] : 0;
        skillLevel = len > 15 ? datas[15] : 0;
        dropConfig = drop;
    }
}
public struct RushConfigArgs
{
    public bool rush_Enable;    // 冲刺技能是否开启
    public int rush_Dmg;        // 冲刺技能伤害
    public int skillCD;         // 技能冷却时间
    public int rushCount;       // 冲刺次数
    public int rush_Distance;   // 冲刺距离
    public int rush_MoveSpeed;  // 冲刺速度
    public int rush_attent_First; // 首次冲刺预警时长
    public int rush_interval;   // 冲刺间隔时间
    public int rush_attent;     // 冲刺预警时间
    public bool retinue_Enable; // 二阶段分身是否开启
    public int transLimit;      // 二阶段血量百分比
    public int retinueNum;      // 二阶段几个分身
    public int retinueLevel;    // 分身等级

    public RushConfigArgs(List<int> data)
    {
        rush_Enable = data[0] == 1;
        rush_Dmg = data[1];
        skillCD = data[2];
        rushCount = data[3];
        rush_Distance = data[4];
        rush_MoveSpeed = data[5];
        rush_attent_First = data[6];
        rush_interval = data[7];
        rush_attent = data[8];
        retinue_Enable = data[9] == 1;
        transLimit = data[10];
        retinueNum = data[11];
        retinueLevel = data[12];
    }
}
public struct FireConfigArgs
{
    public bool fan_Enable; // 扇形技能是否开启
    public int fan_fireDmg;
    public int fan_SkillCD; // 扇形火球技能冷却时间
    public int fan_Angle;   // 扇形角度
    public int fan_FireNum; // 扇形火球数量
    public int fan_FireDis; // 火球距离
    public int fan_Attent;  // 扇形预警时间
    public bool fall_Enable;// 砸地技能释放开启
    public int fall_Dmg;
    public int fall_SkillCD;// 砸地冷却时间
    public int fall_Attent; // 砸地预警时间
    public int fall_Range;  // 砸地范围
    public int followCD;    // 每次技能后摇
    public FireConfigArgs(List<int> data)
    {
        fan_Enable = data[1] == 1;
        fan_fireDmg = data[2];
        fan_SkillCD = data[3];
        fan_Angle = data[4];
        fan_FireNum = data[5];
        fan_FireDis = data[6];
        fan_Attent = data[7];
        fall_Enable = data[8] == 1;
        fall_Dmg = data[9];
        fall_SkillCD = data[10];
        fall_Attent = data[11];
        fall_Range = data[12];
        followCD = data[13];
    }
}
public struct MiasmaConfigArgs
{
    public bool path_Enable;      // 路径毒气是否开启
    public int dmg;              // 路径毒气伤害值
    public int path_CD;          // 路径毒气冷却时间
    public int path_ExitTime;    // 路径存在时长

    public bool range_Enable;    // 范围毒气是否开启
    public int range_CD;         // 范围毒气冷却时间
    public int range_Range;      // 选取范围(米)
    public int range_Attent;     // 预警时间
    public int range_Num;        // 毒气数量
    public int range_PerRange;   // 每个毒气范围(米)
    public int range_ExitTime;   // 范围存在时长
    public MiasmaConfigArgs(List<int> data)
    {
        path_Enable = data[1] == 1;
        dmg = data[2];
        path_CD = data[3];
        path_ExitTime = data[4];
        range_Enable = data[5] == 1;
        range_CD = data[6];
        range_Range = data[7];
        range_Attent = data[8];
        range_Num = data[9];
        range_PerRange = data[10];
        range_ExitTime = data[11];
    }
}

public class GameScene
{
    public float TopRatio = 0;

    public EventHandler<bool> pauseBind = new();

    public AudioSource AS_BGM;
    public AudioSource AS_FX;
    public AudioSource AS_FX_Loop_Craze;

    GameObject mainScene = null;

    // 刀刃的对象池
    readonly Dictionary<string, KnifeObjectPool<BladeBase>> bladePool = new();
    // 道具的对象池
    readonly Dictionary<string, KnifeObjectPool<PropBase>> propPool = new();
    // 角色对象池
    readonly Dictionary<string, KnifeObjectPool<RoleBase>> rolePool = new();
    // 特效对象池
    readonly Dictionary<string, KnifeObjectPool<ItemBase>> effectPool = new();

    Camera mainCamera;
    Transform cameraParent;
    SpriteRenderer bgSR;
    AstarPath pathFinder;
    Transform playerPos;

    public RolePlayer rolePlayer;
    bool readyFlag;

    CameraCtrl cameraCtrl;

    // 记录关卡生成波次
    public readonly EventHandler<int> levelWaveBind = new();
    int levelWaveMax;
    // 关卡配置
    Dictionary<int, LevelConfigArgs> levelConfigTable = new();
    // 生成关卡内容计时器
    float spawnTimer;
    // 关卡敌人总数量
    public int enemyNumMax;
    // 记录已经击杀数量
    public readonly EventHandler<int> enemyNumBind = new();
    // 是否boss关卡
    public bool bossFlag;
    // 记录当前boss类型
    int curBossType;
    int curBossLevel;
    int curBossRetinueNum;
    // 记录当前关卡有哪些敌人，对象池只创建需要的敌人
    readonly List<int> curEnemyTypeList = new();
    // boss生成时间
    float bossTimer = 0;
    // boss倒计时
    public readonly EventHandler<int> bossTimerBind = new();
    // 演出状态下，所有角色无伤
    public bool NoInjury = false;
    // 游戏结束标记
    public bool overFlag = false;
    public bool startFlag = false;
    // 随时间生成刀刃
    float spawnBladeTimer = 0;
    int bladeCount = 0;

    // 记录所有自动掉落的刀刃
    readonly Dictionary<int, BladeBase> autoDropBladeList = new();
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

    Sequence cgTween;

    readonly Dictionary<GameObject, RoleBase> rolePairWithGO = new();
    readonly Dictionary<GameObject, PropBase> propPairWithGO = new();
    readonly Dictionary<GameObject, BladeBase> bladePairWithGO = new();
    readonly Dictionary<GameObject, ItemBase> effectPairWithGO = new();


    public GameObject levelRoot;
    int curLevel;

    readonly static List<Dictionary<int, LevelConfigArgs>> allLevelData = new();    // 所有关卡数据
    readonly static Dictionary<int, (int, int)> allAutoBladeData = new();
    readonly static Dictionary<string, Dictionary<int, RoleConfigArgs>> roleConfigTable = new();    // 全部角色配置
    readonly static Dictionary<int, RushConfigArgs> rushConfigData = new();
    readonly static Dictionary<int, FireConfigArgs> fireConfigData = new();
    readonly static Dictionary<int, MiasmaConfigArgs> miasmaConfigData = new();
    public void AddBlade()
    {
        bladeCount++;
    }

    public void ReduceBlade(BladeBase blade = null)
    {
        bladeCount--;
        if (blade != null && blade.autoSpawnFlag)
        {
            int idx = -1;
            foreach(var item in autoDropBladeList)
            {
                if (item.Value == blade)
                {
                    idx = item.Key;
                    break;
                }
            }
            if (idx != -1) autoDropBladeList.Remove(idx);
        }
    }

    public void InitSpawnBlade()
    {
        var viewMin = cameraCtrl.ViewportToWorldPoint(Vector2.zero);
        var viewMax = cameraCtrl.ViewportToWorldPoint(Vector2.one);
        var abNum = autoDropBladeList.Count;
        var random = new System.Random();
        for (int i = 1; i <= autoBladeConfig.initNum; i++)
        {
            // 随机出现在周围
            var tmpPos = new Vector3(Mathf.Lerp(viewMin.x, viewMax.x, (float)random.NextDouble()), Mathf.Lerp(viewMin.y, viewMax.y, (float)random.NextDouble()));
            tmpPos = GetSafetyPosition(tmpPos);
            tmpPos.z = 0;
            var newBlade = BladePoolPopOne(1);
            newBlade.InitWithoutRole(this, 1, false);
            newBlade.Drop(tmpPos, false);
            autoDropBladeList[abNum + i] = newBlade;
        }
    }

    public void Init()
    {
        gameMgr = new();
        uiMgr = new();
        audioMgr = new();
        pauseBind.Add(val => PauseListener(val));
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

    public void Update()
    {
        if (pauseBind.value) return;
        if (startFlag)
        {
            foreach(var item in rolePairWithGO)
            {
                item.Value.Update();
            }
            StartSpawn();
            AutoSpawnBlade();
        }
        else if (readyFlag && rolePlayer != null) rolePlayer.Update();
    }

    public void FixedUpdate()
    {
        if (pauseBind.value) return;
        if (readyFlag) cameraCtrl.FixedUpdate();
    }

    public void LateUpdate()
    {
        if (pauseBind.value) return;
        if (startFlag)
        {
            cameraCtrl.Update();
            foreach (var item in rolePairWithGO)
            {
                item.Value.LateUpdate();
            }
        }
        else if (readyFlag && rolePlayer != null)
        {
            rolePlayer.LateUpdate();
        }
    }

    public void RoleNoInjury(bool value)
    {
        NoInjury = value;
        foreach(var item in rolePairWithGO)
        {
            //item.Value.NoInjury(value);   TODO
        }
    }

    // 击退玩家
    public void PlayerRepulse(Vector3 pos, int disLimit = 0)
    {
        var dis = Vector3.Distance(pos, rolePlayer.transform.position);
        // 玩家被击退到5米外
        if (disLimit == 0) disLimit = 10;
        if (dis < disLimit)
        {
            var dir = rolePlayer.transform.position - pos;
            dir.z = 0;
            dir = dir.normalized;
            var newPos = pos + dir * disLimit;
            newPos = GetSafetyPosition(newPos);
            newPos.z = 0;
            //rolePlayer.PlayerRepulse();   TODO
        }
    }

    void AutoSpawnBlade()
    {
        // 场上少于50个，每帧生成10个
        if (bladeCount < autoBladeConfig.maxNum)
        {
            var viewMin = cameraCtrl.ViewportToWorldPoint(Vector3.zero);
            var viewMax = cameraCtrl.ViewportToWorldPoint(Vector3.one);
            Dictionary<int, int> sideList = new();
            // 左边可用
            if (viewMin.x - spawnViewMinOff > cameraCtrl.mapLeftBound + mapBound) sideList[sideList.Count + 1] = 1;
            // 下方可用
            if (viewMin.y - spawnViewMinOff > cameraCtrl.mapBottomBound + mapBound) sideList[sideList.Count + 1] = 2;
            // 右方可用
            if (viewMax.x + spawnViewMinOff < cameraCtrl.mapRightBound - mapBound) sideList[sideList.Count + 1] = 3;
            // 上方可用
            if (viewMax.y + spawnViewMinOff < cameraCtrl.mapTopBound - mapBound) sideList[sideList.Count + 1] = 4;
            var num = Mathf.Min(spawnCountPerFreme, autoBladeConfig.maxNum - bladeCount);
            var abNum = autoDropBladeList.Count;
            var sideNum = sideList.Count;
            if (sideNum < 1)
            {
                sideList = new() { [1] = 1, [2] = 2, [3] = 3, [4] = 4 };
                sideNum = 4;
            }
            var random = new System.Random();
            for (int i = 0; i < num; ++i)
            {
                // 随机出现在周围
                var spawnDir = sideList[Random.Range(1, sideNum + 1)];
                Vector3 tmpPos;
                // 上方
                if (spawnDir == 4) 
                    tmpPos = new(Mathf.Lerp(viewMin.x - spawnViewMaxOff, viewMax.x + spawnViewMaxOff, (float)random.NextDouble()), Mathf.Lerp(viewMax.y + spawnViewMinOff, viewMax.y + spawnViewMaxOff, (float)random.NextDouble()));
                else if (spawnDir == 2)
                    tmpPos = new(Mathf.Lerp(viewMin.x - spawnViewMaxOff, viewMax.x + spawnViewMaxOff, (float)random.NextDouble()), Mathf.Lerp(viewMin.y + spawnViewMaxOff, viewMin.y + spawnViewMinOff, (float)random.NextDouble()));
                else if (spawnDir == 1)
                    tmpPos = new(Mathf.Lerp(viewMin.x - spawnViewMaxOff, viewMin.x + spawnViewMinOff, (float)random.NextDouble()), Mathf.Lerp(viewMin.y + spawnViewMaxOff, viewMin.y + spawnViewMaxOff, (float)random.NextDouble()));
                else
                    tmpPos = new(Mathf.Lerp(viewMax.x - spawnViewMinOff, viewMax.x + spawnViewMaxOff, (float)random.NextDouble()), Mathf.Lerp(viewMin.y + spawnViewMaxOff, viewMax.y + spawnViewMaxOff, (float)random.NextDouble()));
                tmpPos = GetSafetyPosition(tmpPos);
                tmpPos.z = 0;
                var newBlade = BladePoolPopOne(1);
                newBlade.InitWithoutRole(this, 1, false);
                newBlade.Drop(tmpPos, false);
                autoDropBladeList[abNum + i] = newBlade;
            }
        }
        else
        {
            // 场上刀刃大于最大值，回收屏幕外的刀刃
            spawnBladeTimer += Time.deltaTime;
            var tLength = autoDropBladeList.Count;
            if (tLength > 0 && spawnBladeTimer >= autoBladeCheckCD)
            {
                spawnBladeTimer = 0;
                var viewMin = cameraCtrl.ViewportToWorldPoint(Vector2.zero);
                var viewMax = cameraCtrl.ViewportToWorldPoint(Vector2.one);
                Dictionary<int, int> idx = new();
                int tmpI = 0;
                for (int i = 1; i <= tLength; ++i)
                {
                    var tmpPos = autoDropBladeList[i].transform.position;
                    if (tmpPos.x < viewMin.x - spawnViewMaxOff || tmpPos.y < viewMin.y - spawnViewMaxOff || tmpPos.x > viewMax.x + spawnViewMaxOff || tmpPos.y > viewMax.y + spawnViewMaxOff)
                    {
                        tmpI++;
                        idx[tmpI] = i;
                        // 每帧最多回收10个
                        if (tmpI >= 10) break;
                    }
                }
                if (tmpI > 0)
                {
                    var removeI = 0;
                    foreach(var item in idx)
                    {
                        var v = item.Value;
                        autoDropBladeList[v - removeI].PushInPool();
                        autoDropBladeList.Remove(v - removeI);
                        removeI++;
                    }
                    bladeCount -= tmpI;
                }
            }
        }
    }

    public void BladePoolPushOne(int type, BladeBase item)
    {
        bladePairWithGO.Remove(item.gameObject);
        GetBladePool(type).Put(item);
    }

    BladeBase BladePoolPopOne(int type)
    {
        var obj = GetBladePool(type).Get();
        obj.gameObject.name = TriggerType.blade + Names.split + Names.Blade;
        bladePairWithGO[obj.gameObject] = obj;
        return obj;
    }

    KnifeObjectPool<BladeBase> GetBladePool(int type)
    {
        return type switch
        {
            1 => bladePool[BladeNames.defaultblade],
            2 => bladePool[BladeNames.snowblade],
            3 => bladePool[BladeNames.fireblade],
            4 => bladePool[BladeNames.miasmablade],
            5 => bladePool[BladeNames.lightningblade],
            6 => bladePool[BladeNames.ironblade],
            7 => bladePool[BladeNames.hugeAxe],
            8 => bladePool[BladeNames.hugeblade],
            _ => null
        };
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
            temGo.gameObject.name = TriggerType.prop + Names.split + Names.Prop;

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

    PropBase SpawnOneProp(int type,  Vector3 pos = default)
    {
        var tmpGO = PropPoolPopOne(type);
        tmpGO.transform.SetParent(levelRoot.transform);
        tmpGO.Init(this, type);
        tmpGO.gameObject.name = TriggerType.prop + Names.split + Names.Prop;
        pos = GetSafetyPosition(pos);
        pos.z = 0;
        tmpGO.transform.position = pos;
        return tmpGO;
    }

    // 生产敌人对象
    void SpawnEnemyPrefab(int num, int type, int level)
    {
        var roleName = roleTypeWithName[type];
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
            tmpGo.gameObject.name = TriggerType.role + Names.split + Names.enemy;
        }
    }

    bool EnemyDie(RoleBase role)
    {
        var recycleFlag = true;
        if (!role.isPlayer) enemyNumBind.Send(enemyNumBind.value - 1);
        if (!overFlag && (role.isPlayer || role.isBoss || (!bossFlag && enemyNumBind.value >= enemyNumMax)))
        {
            overFlag = true;
            readyFlag = false;
            if (role.isPlayer) recycleFlag = false;
            //if (role.isPlayer) audioMgr.PlayOneShot(AudioClips.shibai);TODO
            //else audioMgr.PlayOneShot(AudioClips.shengli);TODO
            KillCGTween();
            cgTween = DOTween.Sequence();
            cgTween.InsertCallback(0, () => Time.timeScale = 0.1f);
            cgTween.InsertCallback(0.2f, () => Time.timeScale = 1);
            cameraCtrl.FocusPos(true, role.transform, true);
            cameraCtrl.Shake_BossDie();
            cgTween.InsertCallback(1.1f, () =>
            {
                //if (role.isPlayer) gameMgr.GameFail();TODO
                //if (role.isPlayer) gameMgr.GameWin();TODO
            });
            RefreshPause();
        }
        return recycleFlag;
    }

    public void RolePoolPushOne(string roleName, RoleBase item)
    {
        rolePairWithGO.Remove(item.gameObject);
        rolePool[roleName].Put(item);
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

    public Vector3 GetSafetyPosition(Vector3 pos)
    {
        return pathFinder.GetNearest(pos, Pathfinding.NNConstraint.Default).position;
    }

    public void Ready(Transform canvas, int level)
    {
        if (Screen.height > Screen.safeArea.yMax) TopRatio = (Screen.height - Screen.safeArea.yMax) / Screen.height;
        curLevel = level;
        mainScene ??= ResManager.LoadPrefab("MainScene", canvas, Vector3.one, Vector3.zero);
        cameraParent = Util.GetTransform(mainScene, "CameraParent");
        mainCamera = Util.GetComponent<Camera>(cameraParent.gameObject, "Camera");
        AS_BGM = Util.GetComponent<AudioSource>(cameraParent.gameObject, "AS_BGM");
        AS_FX = Util.GetComponent<AudioSource>(cameraParent.gameObject, "AS_FX");
        AS_FX_Loop_Craze = Util.GetComponent<AudioSource>(cameraParent.gameObject, "AS_FX_Loop_Craze");

        levelRoot = ResManager.LoadPrefab("level" + level, mainScene.transform, Vector3.one, Vector3.zero);
        bgSR = Util.GetComponent<SpriteRenderer>(levelRoot, "map/Bg");
        playerPos = Util.GetTransform(levelRoot, "map/playerPos");
        pathFinder = levelRoot.GetComponent<AstarPath>();
        pathFinder.threadCount = Pathfinding.ThreadCount.AutomaticHighLoad;

        InitLevelConfig();
        InitRoleConfig();
        BladePoolReady();
        RolePoolReady();
        PropPoolReady();
        EffectPoolReady();
    }
    
    void InitLevelConfig()
    {
        bossFlag = false;
        curEnemyTypeList.Clear();
        enemyNumMax = 0;
        levelConfigTable = allLevelData[curLevel];
        foreach (var item in levelConfigTable)
        {
            var v = item.Value;
            if (v.enemyType > 0)
            {
                var tmpEnemyType = v.enemyType;
                enemyNumMax += v.enemyNum;
                if (!bossFlag && tmpEnemyType > 6 && tmpEnemyType < 10)
                {
                    bossFlag = true;
                    bossTimer = v.time;
                    curBossType = tmpEnemyType;
                    curBossLevel = v.enemyLevel;
                }
                curEnemyTypeList.Add(tmpEnemyType);
            }
        }
        autoBladeConfig = allAutoBladeData[curLevel];
    }

    void BladePoolReady()
    {
        var fields = typeof(BladeNames).GetFields();
        for (int i = 0; i < fields.Length; ++i)
        {
            var name = fields[i].Name;
            if (!bladePool.ContainsKey(name)) bladePool[name] = new(levelRoot.transform);
        }
    }

    void RolePoolReady()
    {
        foreach (var v in curEnemyTypeList)
        {
            var enemyName = roleTypeWithName[v - 1];
            if (!rolePool.ContainsKey(enemyName)) rolePool[enemyName] = new(levelRoot.transform);
        }
    }

    void PropPoolReady()
    {
        var fields = typeof(PropNames).GetFields();
        for (int i = 0; i < fields.Length; ++i)
        {
            var name = fields[i].Name;
            if (!propPool.ContainsKey(name)) propPool[name] = new(levelRoot.transform);
        }
    }

    void EffectPoolReady()
    {
        var fields = typeof(SkillNames).GetFields();
        for (int i = 0; i < fields.Length; ++i)
        {
            var name = fields[i].Name;
            if (!effectPool.ContainsKey(name)) effectPool[name] = new(levelRoot.transform);
        }
        //var parent = levelRoot.transform;
        //effectPool[SkillNames.fx_bingzhangjineng] = new(parent);
        //effectPool[SkillNames.fx_snow01] = new(parent);
        //effectPool[SkillNames.fx_fire01] = new(parent);
        //particalItemPool[SkillNames.fx_pindao] = new(parent);
        //particalItemPool[SkillNames.fx_dimian] = new(parent);
        //effectPool[SkillNames.fx_lightning01] = new(parent);
        //effectPool[SkillNames.fx_miasma] = new(parent);
        //effectPool[SkillNames.fx_miasma_boss] = new(parent);
        //particalItemPool[SkillNames.fx_juese_shouji] = new(parent);
        //effectPool[SkillNames.fx_fire_boss] = new(parent);
        //spriteRendererPool[SkillNames.rushGuide] = new(parent);
        //spriteRendererPool[SkillNames.fanGuide] = new(parent);
        //spriteRendererPool[SkillNames.circleGuide] = new(parent);
        //effectPool[SkillNames.circleDmg] = new(parent);

    }

    void InitRoleConfig()
    {
        if (bossFlag)
        {
            var roleName = roleTypeWithName[curBossType - 1];
            var tmpBossConfig = GetRoleConfig(roleName, curBossLevel);
            if (tmpBossConfig.callEnemyType > 0 && tmpBossConfig.callNum > 0)
            {
                curEnemyTypeList.Add(tmpBossConfig.callEnemyType);
            }
            if (roleName == RoleNames.axeboss)
            {
                var rushConfig = GetRushConfigById(tmpBossConfig.skillLevel);
                if (rushConfig.retinueNum > 0)
                {
                    // 分身类型是11
                    curBossRetinueNum = rushConfig.retinueNum;
                    curEnemyTypeList.Add(11);
                }
            }
        }
    }

    RushConfigArgs GetRushConfigById(int id) => rushConfigData[id];
    FireConfigArgs GetFireConfigById(int id) => fireConfigData[id];
    MiasmaConfigArgs GetMiasmaConfigById(int id) => miasmaConfigData[id];

    public void InitAllConfigWhenGameStart()
    {
        System.Diagnostics.Stopwatch watch = new();
        watch.Start();
        string autoPath = Application.dataPath + "/ManagedResources/autoBlade.csv";
        var autoData = Util.ReadSingleConfig(File.ReadAllText(autoPath));
        foreach(var item in autoData)
        {
            allAutoBladeData[item[0]] = (item[1], item[2]);
        }
        string configLevelPath = Application.dataPath + "/ManagedResources/Configs/Level";
        var cfgs = Directory.GetFiles(configLevelPath, "*.csv", SearchOption.TopDirectoryOnly);
        for (int i = 0; i < cfgs.Length; ++i)
        {
            var str = File.ReadAllText(cfgs[i]);
            var data = Util.ReadSingleConfig(str);
            Dictionary<int, LevelConfigArgs> singleLevel = new();
            for (int j = 0;j < data.Count; ++j)
            {
                var lca = new LevelConfigArgs(data[j]);
                singleLevel[data[j][0]] = lca;
            }
            allLevelData.Add(singleLevel);
        }
        string configRolePath = Application.dataPath + "/ManagedResources/Configs/Role";
        var allName = typeof(RoleNames).GetFields();
        for (int i = 0;i < allName.Length; ++i)
        {
            string name = allName[i].Name;
            var str = File.ReadAllText(configRolePath + "/" + name);
            var data = Util.ReadSingleConfig(str);
            Dictionary<int, RoleConfigArgs> singleLevel = new();
            var dropData = Util.GetDropNumData();
            for (int j = 0; j < data.Count; ++j)
            {
                var lca = new RoleConfigArgs(name, data[j], dropData[j]);
                singleLevel[data[j][0]] = lca;
            }
            roleConfigTable[name] = singleLevel;
        }

        string rushConfigPath = Application.dataPath + "/ManagedResources/rushConfig.csv";
        var rushs = Util.ReadSingleConfig(File.ReadAllText(rushConfigPath));
        for (int i = 0; i < rushs.Count; ++i)
        {
            rushConfigData[rushs[i][0]] = new RushConfigArgs(rushs[i]);
        }

        string fireConfigPath = Application.dataPath + "/ManagedResources/fireConfig.csv";
        var fires = Util.ReadSingleConfig(File.ReadAllText(fireConfigPath));
        for (int i = 0; i < fires.Count; ++i)
        {
            fireConfigData[fires[i][0]] = new FireConfigArgs(fires[i]);
        }

        string miasmaConfigPath = Application.dataPath + "/ManagedResources/miasmaConfig.csv";
        var miasmas = Util.ReadSingleConfig(File.ReadAllText(miasmaConfigPath));
        for (int i = 0; i < miasmas.Count; ++i)
        {
            miasmaConfigData[miasmas[i][0]] = new MiasmaConfigArgs(miasmas[i]);
        }

        watch.Stop();
        Debug.LogError("初始化表耗时：" + watch.ElapsedMilliseconds);
    }

    KnifeObjectPool<PropBase> GetPropPool(int type)
    {
        return type switch
        {
            1 => propPool[PropNames.defaultBladeProp],
            2 => propPool[PropNames.iceBladeProp],
            3 => propPool[PropNames.fireBladeProp],
            4 => propPool[PropNames.miasmaBladeProp],
            5 => propPool[PropNames.lightningBladeProp],
            6 => propPool[PropNames.crazeProp],
            7 => propPool[PropNames.swordProp],
            8 => propPool[PropNames.fleetfootedProp],
            9 => propPool[PropNames.loveProp],
            10 => propPool[PropNames.defaultBladeProp2],
            _ => null
        };
    }
    PropBase PropPoolPopOne(int type)
    {
        var obj = GetPropPool(type).Get();
        propPairWithGO[obj.gameObject] = obj;
        return obj;
    }

    public void PropPoolPushOne(int type, PropBase item)
    {
        propPairWithGO.Remove(item.gameObject);
        GetPropPool(type).Put(item);
    }

    public ItemBase PopEffect(string name)
    {
        ItemBase obj = effectPool[name].Get();
        effectPairWithGO[obj.gameObject] = obj;
        return obj;
    }

    public void PushEffect(string name, ItemBase item)
    {
        effectPairWithGO.Remove(item.gameObject);
        effectPool[name].Put(item);
    }

    public Vector3 GetPlayerViewPos()
    {
        //return mainCamera.WorldToViewportPoint(rolePlayer.center.position); TODO
        return default;
    }

    void KillCGTween()
    {
        cgTween?.Kill();
        cgTween = null;
    }
    public void OverBattle()
    {
        pauseBind.Send(true);
        startFlag = false;
        overFlag = true;
        //foreach (var item in rolePairWithGO) item.Value.Stop();   TODO
    }

    public void StartBattle()
    {
        // 开始出怪
        levelWaveMax = levelConfigTable.Count;
        levelWaveBind.Send(1);
        spawnTimer = 0;
        //rolePlayer.Start();TODO
        startFlag = true;
        overFlag = false;
    }

    public void ResetGame()
    {
        KillCGTween();
        pauseBind.Send(false);
        // 清空对象池
        foreach (var item in rolePool) item.Value.Clear();
        foreach (var item in bladePool) item.Value.Clear();
        foreach (var item in propPool) item.Value.Clear();
        foreach (var item in effectPool) item.Value.Clear();
        OverBattle();
        if (rolePlayer == null)
        {
            // 初始化主角
            rolePlayer = new RolePlayer
            {
                gameObject = ResManager.LoadPrefab(RoleNames.player, levelRoot.transform, Vector3.one, Vector3.zero)
            };
            rolePlayer.gameObject.name = TriggerType.role + Names.split + Names.Player;
        }
        if (cameraCtrl == null)
        {
            cameraCtrl = new CameraCtrl(mainCamera, cameraParent, bgSR, this);
            cameraCtrl.Init(rolePlayer);
        }
        var playerPosV3 = GetSafetyPosition(playerPos.position);
        playerPosV3.z = 0;
        rolePlayer.Init(this, uiMgr, RoleNames.player, GetRoleConfig(RoleNames.player, curLevel), playerPosV3);
        rolePairWithGO[rolePlayer.gameObject] = rolePlayer;
        cameraCtrl.Reset();
        if (bossFlag) bossTimerBind.Send(bossTimer);
        else enemyNumBind.Send(0);
        readyFlag = true;
        NoInjury = false;
        Time.timeScale = 1;
        bladeCount = 0;
        autoDropBladeList.Clear();
        InitSpawnBlade();
    }
}
