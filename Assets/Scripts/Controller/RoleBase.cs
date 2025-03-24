

using System.Collections.Generic;
using bc.MiniGameBase;
using UnityEngine;

public class RoleBase : ItemBase
{
    public override void InitComponent()
    {
        
    }
    
    // private var EaseTemp = DG.Tweening.Ease;
    // private var easeMove = EaseTemp.OutQuad;
    // private var easeHit = EaseTemp.OutQuad;
    // private var easeHIt2 = EaseTemp.InQuad;
    enum ItemNames {
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

    protected Collider collider;
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

    protected Scene sceneMgr;

    // protected UIMgr uiMgr;
    // 是否为boss
    protected bool isBoss;
    // 角色名字
    protected string roleName;

    protected RoleConfigArgs roleData;
    // 初始位置
    protected Vector3 initPos;
    protected Dictionary<GameObject, GameObject> bladeList;
    protected Transform disply;
    // 无敌状态
    protected bool invincible;
    // 无伤状态
    protected bool noInjury;
    // 不受击退控制
    protected bool uncontrolled;
}
