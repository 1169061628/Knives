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
    readonly KnifeObjectPool<BladeBase> bladePool = new();
    // 道具的对象池
    readonly KnifeObjectPool<PropBase> propPool = new();
    // 角色对象池
    readonly KnifeObjectPool<RoleBase> rolePool = new();
    // 特效对象池
    readonly KnifeObjectPool<EffectBase> effectPool = new();

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

    public GameObject levelRoot;

    public void AddBlade()
    {
        bladeCount++;
    }
}
