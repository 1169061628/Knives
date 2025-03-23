using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeBase : ItemBase
{
    RoleBase role;  // 对应的角色
    bool isValid;   // 刀刃是否可用
    bool propFlag;  // 是否可捡起来
    BladeType bladeType;// 刀刃类型
}
