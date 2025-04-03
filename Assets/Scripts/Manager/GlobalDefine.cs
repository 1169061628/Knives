using System;
using UnityEngine;

public static class ManyKnivesDefine
{
    public static int ToInt(this Enum e)
    {
        return e.GetHashCode();
    }

    public readonly struct UnityAxis
    {
        public const RectTransform.Axis Horizontal = RectTransform.Axis.Horizontal;
        public const RectTransform.Axis Vertical = RectTransform.Axis.Vertical;
    }

    // 移动偏移
    public readonly struct RoleMoveOff
    {
        public const float offTimeCD = 1;
        public const float offAngleMin = 0;
        public const float offAngleMax = 30;
    }

    // 玩家搜集刀刃的范围
    public const float playerCollectRange = 3;

    public readonly struct RoleDebuffConfig
    {
        // 毒气受伤频率
        public const float hurtByMiasmaCD = 0.7f;
        // 毒气减速持续时间
        public const float debuff_moveSp_CD = 0.5f;
        // 冷冻持续几秒
        public const float debuff_freeze_CD = 2f;
        // 闪电禁锢持续几秒
        public const float debuff_light_CD = 2f;
        // 碰到敌人身体扣血频率
        public const float hurtByRoleCD = 0.9f;
    }

    // 玩家技能伤害(百分比)
    public readonly struct PlayerEffectDmg
    {
        // 冰冻
        public const float snow = 1.3f;
        // 火球
        public const float fire = 3.8f;
        // 毒气
        public const float miasma = 1.2f;
        // 闪电
        public const float lightning = 4f;
        // 狂暴系数
        public const float crazeMulti = 1.7f;
    }

    // 刀刃伤害
    // 1普通刀刃2冰冻3火球4瘴气5闪电
    public static int[] bladeDmgWithType = { 100, 180, 180, 180, 180 };

    // 玩家技能计数相关配置
    public readonly struct PlayerEffectConfig
    {
        // 冰冻
        public const float CD_snow = 1.5f;
        // 冰球数量
        public const int snow_Num = 6;
        // 冰球飞行多少米
        public const float snow_FlyLimit = 10f;
        // 冰球飞行速度
        public const float snow_FlySp = 10f;
        // 火球CD
        public const float CD_fire = 1f;
        // 每个火球间隔
        public const float CD_fire2 = 0.1f;
        // 每轮发射数量
        public const int fire_Num = 8;
        // 毒气
        public const float CD_miasma = 3f;
        // 玩家毒气存在时间
        public const float miasma_Stay = 5f;
        // 闪电
        public const float CD_lightning = 3.5f;
        // 闪电半径米
        public const float lightning_raduis = 8.5f;
        // 闪电存在时间
        public const float lightning_Dur = 1.8f;
        // 狂暴
        public const float CD_craze = 5f;
        // 狂暴状态，刀刃旋转速度倍率
        public const float craze_bladeSpeedMulti = 1.3f;
        // 大宝剑
        public const float CD_bigSword = 5f;
        public const float bigSwordMulti = 1.5f;
        // 移速
        public const float CD_fleetfoot = 10f;
        public const float moveSpMulti = 1.5f;
        public const float fx_craze_size = 0.125f;
    }

    // 刀刃预加载几个
    public const int bladePoolPreloadNum = 30;
    // 小兵预加载几个，boss不预加载
    public const int enemyPoolPreloadNum = 5;
    // 道具预加载几个，boss不预加载
    public const int propPoolPreloadNum = 5;
    // 血条预加载几个
    public const int hpSliderPreloadNum = 8;
    // 伤害数字
    public const int dmgPreloadNum = 15;

    // 1刀刃顺时针/-1逆时针
    public const int bladeSide = 1;
    // 刀刃旋转速度
    public const float bladeInitSpeed = 200f;
    // 敌人刀刃旋转速度
    public const float bladeInitSpeed_Enemy = 200f;
    // 刀刃外扩半径
    public const float bladeRadius = 1.1f;
    // 玩家发射技能时间
    public const float attackTime = 2f;
    // 火球数量
    public const int fireBallNum = 8;
    // 刀刃道具新增刀刃数量
    public const int newBladeNum = 5;
    // 刀刃最大数量
    public const int bladeMaxNum = 36;
    // 小怪掉落的刀增加几个
    public const int newBladeNumEnemy = 1;

    // 层级配置
    public readonly struct SortOrder
    {
        public const int role_base = 50;
        public const int role_display = 52;
        public const int blade_display = 51;
        // 角色底层的特效
        public const int effect_base = -1;
        // 角色上层的特效
        public const int effect_fly = 100;
        // 角色上层的特效2
        public const int effect_fly2 = 200;
        // 刀刃飞行时层级
        public const int blade_fly = 100;
        public const int blade_base = 0;
        public const int prop_default = 0;
    }

    // 名字分隔符
    public readonly struct Names
    {
        public const string split = "_";
        public const string enemy = "Enemy";
        public const string Player = "Player";
        public const string Prop = "Prop";
        public const string Effect = "Effect";
        public const string Blade = "Blade";
    }

    public readonly struct TriggerType
    {
        public const int role = 1;
        public const int prop = 2;
        public const int effect = 3;
        public const int blade = 4;
    }

    public readonly struct LayerID
    {
        public const int ui = 12;
        // 路障和主角
        public const int collisionMap = 13;
        // 敌人
        public const int collisionEnemy = 14;
        // 角色触发器
        public const int triggerPlayer = 15;
        public const int triggerEnemy = 16;
    }

    // 1普通刀刃2冰冻技能3火球技能4瘴气技能5闪电技能6狂暴7大宝剑8飞毛腿9爱心10小怪掉的刀
    public readonly struct PropType
    {
        public const int blade_default = 1;
        public const int blade_snow = 2;
        public const int blade_fire = 3;
        public const int blade_miasma = 4;
        public const int blade_lightning = 5;
        public const int craze = 6;
        public const int bigSword = 7;
        public const int moveSp = 8;
        public const int heart = 9;
        public const int blade_default2 = 10;
    }

    // 2冰冻技能3火球技能4瘴气技能5闪电技能。6敌人法师的光球，7Boss范围伤害,8boss范围毒气
    public readonly struct EffectType
    {
        public const int snow = 2;
        public const int fire = 3;
        public const int miasma = 4;
        public const int lightning = 5;
        public const int enemyBall = 6;
        public const int bossCircle = 7;
    }

    public readonly struct MK_Hit
    {
        public const float playerMoveDis = 1f;
        public const float playerMoveDur = 0.15f;
        public const float enemyMoveDis = 1f;
        public const float enemyMoveDur = 0.15f;
        public const float fillColDur1 = 0.1f;
        public const float fillColDur2 = 0.15f;
    }

    public readonly struct RoleDir
    {
        public static Vector3 forward = new(1, 1, 1);
        public static Vector3 back = new(-1, 1, 1);
    }

    public const string AnimatorName = "StateIndex";

    public readonly struct AnimatorState
    {
        // 走路
        public const int walk = 0;
        // 死亡
        public const int die = 1;
        // 攻击
        public const int attack = 2;
        // 入场
        public const int entrance = 3;
        public const int idle = 4;
    }

    // 音效列表
    public readonly struct AudioClips
    {
        public const string shengli = "shengli";
        public const string return_blood = "return_blood";
        public const string pick_knife = "pick_knife";
        public const string monster_death = "monster_death";
        public const string monster_death2 = "monster_death2";
        public const string monster_dash = "monster_dash";
        public const string lightning = "lightning";
        public const string knife_fight = "knife_fight";
        public const string diski_firlat = "diski_firlat";
        public const string character_fireball = "character_fireball";
        public const string boss_fireball = "boss_fireball";
        public const string boss_fall = "boss_fall";
        public const string boss_dash = "boss_dash";
        public const string boss_appears = "boss_appears";
        public const string bingdong = "bingdong";
        public const string accelerate = "accelerate";
        public const string hit = "hit";
        public const string violent = "violent";
        public const string Invincible = "Invincible";
        public const string shibai = "shibai";
        public const string boss_die = "boss_die";
        public const string boss_die2 = "boss_die2";
        public const string pick_props = "pick_props";
        public const string BGM2 = "BGM2";
    }

    public const string levelDataFormat = "level{0}";
    // 文案
    public const string langPath = "casualgame/bcmanyknives/tablecsv/lang.csv";

    public readonly struct SkillNames
    {
        public const string fx_snow01 = "fx_snow01";
        public const string fx_pindao = "fx_pindao";
        public const string fx_miasma = "fx_miasma";
        public const string fx_lightning01 = "fx_lightning01";
        public const string fx_fire01 = "fx_fire01";
        public const string fx_dimian = "fx_dimian";
        public const string fx_bingzhangjineng = "fx_bingzhangjineng";
        public const string fx_miasma_boss = "fx_miasma_boss";
        public const string fx_juese_shouji = "fx_juese_shouji";
        public const string fx_fire_boss = "fx_fire_boss";
        public const string rushGuide = "rushGuide";
        public const string fanGuide = "fanGuide";
        public const string circleGuide = "circleGuide";
        public const string circleDmg = "circleDmg";
    }

    public readonly struct BladeNames
    {
        public const string snowblade = "snowblade";
        public const string miasmablade = "miasmablade";
        public const string lightningblade = "lightningblade";
        public const string fireblade = "fireblade";
        public const string defaultblade = "defaultblade";
        public const string ironblade = "ironblade";
        public const string hugeAxe = "hugeAxe";
        public const string hugeblade = "hugeblade";
    }

    public readonly struct RoleNames
    {
        public const string player = "player";
        public const string onion = "onion";
        public const string knight = "knight";
        public const string gadiator = "gadiator";
        public const string gadiator2 = "gadiator2";
        public const string bingmodaoshi = "bingmodaoshi";
        public const string dafeilong = "dafeilong";
        public const string ironboss = "ironboss";
        public const string axeboss2 = "axeboss2";
        public const string axeboss = "axeboss";
        public const string bladeboss = "bladeboss";
    }


    public readonly struct PropNames
    {
        public const string defaultBladeProp = "defaultBladeProp";
        public const string defaultBladeProp2 = "defaultBladeProp2";
        public const string iceBladeProp = "iceBladeProp";
        public const string fireBladeProp = "fireBladeProp";
        public const string miasmaBladeProp = "miasmaBladeProp";
        public const string lightningBladeProp = "lightningBladeProp";
        public const string crazeProp = "crazeProp";
        public const string fleetfootedProp = "fleetfootedProp";
        public const string loveProp = "loveProp";
        public const string swordProp = "swordProp";
    }

    public const string levelNameFormat = "level{0}";

    public static RaycastHit2D Func_Raycast(LayerMask layerMask, Vector2 from, Vector2 to, float rayLength, bool draw = false, Color? color = null)
    {
        if (draw)
        {
            if (color == null)
            {
                color = Color.white;
            }
            Debug.DrawRay(from, to * rayLength, color.Value);
        }
        RaycastHit2D[] hitInfos = Physics2D.RaycastAll(from, to, rayLength, layerMask);
        if (hitInfos.Length > 0)
        {
            return hitInfos[0];
        }
        return default;
    }

    // 忽略两个碰撞体
    public static void Func_IgnoreCollision(Collider2D col1, Collider2D col2)
    {
        Physics2D.IgnoreCollision(col1, col2);
    }

    public static Collider2D Func_OverlapCircle(Vector2 pos, float radius, LayerMask layerMark)
    {
        return Physics2D.OverlapCircle(pos, radius, layerMark);
    }

    public static (Vector3[], float) Func_BezierCurveWithThreePoints(Vector3 v1, Vector3 v2, Vector3 v3, int vCount = 5)
    {
        Vector3[] pointList = new Vector3[vCount + 1];
        float dis = 0;
        for (int i = 0; i <= vCount; i++)
        {
            float ratio = (float)i / vCount;
            pointList[i] = Vector3.Lerp(Vector3.Lerp(v1, v2, ratio), Vector3.Lerp(v2, v3, ratio), ratio);
            if (i > 0)
            {
                dis += Vector3.Distance(pointList[i - 1], pointList[i]);
            }
        }
        return (pointList, dis);
    }

    public static float Func_LineCircleIntersectionFactor(Vector2 circleCenter, Vector2 linePoint1, Vector2 linePoint2, float radius)
    {
        Vector2 tmpVec = linePoint2 - linePoint1;
        float segmentLength = tmpVec.magnitude;
        Vector2 normalizedDirection = segmentLength > 0.00001f ? tmpVec / segmentLength : Vector2.zero;
        Vector2 dirToStart = linePoint1 - circleCenter;
        float dot = Vector2.Dot(dirToStart, normalizedDirection);
        float discriminant = dot * dot - (dirToStart.sqrMagnitude - radius * radius);
        if (discriminant < 0)
        {
            discriminant = 0;
        }
        float t = -dot + Mathf.Sqrt(discriminant);
        return segmentLength > 0.00001f ? (t / segmentLength) : 1;
    }

    // 设置布局，限定宽/高
    public static void Func_SetTextLayoutWithFixed(UnityEngine.UI.Text text, int axis, float maxValue)
    {
        float curValue = axis == 0 ? text.preferredWidth : text.preferredHeight;
        if (curValue > maxValue)
        {
            curValue = maxValue;
        }
        text.rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, curValue);
        int nextAxis = axis == 0 ? 1 : 0;
        curValue = nextAxis == 0 ? text.preferredWidth : text.preferredHeight;
        text.rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)nextAxis, curValue);
    }

    // 1普通刀刃2冰冻技能3火球技能4瘴气技能5闪电技能6狂暴7大宝剑8飞毛腿9爱心,道具数量,
    // 敌人类型1洋葱头2骑士3角斗士4大乌龟5冰魔导士6宝箱怪7ironBoss8axeBoss9bladeBoss 10角斗士2,11axeboss2,
    public static string[] roleTypeWithName = { RoleNames.onion, RoleNames.knight, RoleNames.gadiator, RoleNames.dafeilong, RoleNames.bingmodaoshi, null, RoleNames.ironboss, RoleNames.axeboss, RoleNames.bladeboss, RoleNames.gadiator2, RoleNames.axeboss2 };

    // 角色类型对应对象池
    public static string[] roleTypeWithPoolFile = { "bcmanyknives_role_onion", "bcmanyknives_role_knight", "bcmanyknives_role_gadiator", "bcmanyknives_role_dafeilong", "bcmanyknives_role_bingmodaoshi", null, "bcmanyknives_role_fireboss", "bcmanyknives_role_rushboss", "bcmanyknives_role_miasmaboss", "bcmanyknives_role_gadiator", "bcmanyknives_role_rush_retinue" };

    public static LayerMask MapLayerMask = 1 << LayerID.collisionMap;
    public static RaycastHit2D Physics2D_Raycast(Vector2 origin, Vector2 direction, float distance, LayerMask layerMask)
    {
        return Physics2D.Raycast(origin, direction, distance, layerMask);
    }

    public static Collider2D OverlapCircle(Vector2 point, float radius, LayerMask layerMask)
    {
        return Physics2D.OverlapCircle(point, radius, layerMask);
    }
}