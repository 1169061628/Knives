using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeBase : ItemBase
{
    RoleBase role;  // 对应的角色
    bool isValid;   // 刀刃是否可用
    bool propFlag;  // 是否可捡起来
    BladeType bladeType;// 刀刃类型
    float height;    // 刀刃高度
    bool flyFlag;   // 是否在飞行状态
    Transform parent;   // 刀刃父物体，只有RolePlayer有用
    bool autoSpawnFlag; // 是否自动生成

    GameObject blade;
    Collider2D collider2D;
    SpriteRenderer spriteRenderer;

    public void Awake()
    {
        blade = Util.GetGameObject(gameObject, "blade");
        spriteRenderer = blade.GetComponent<SpriteRenderer>();
        collider2D = blade.GetComponent<Collider2D>();
        height = spriteRenderer.bounds.size.y * 0.5f;
    }

    public void Init()
    {

    }
}
