using UnityEngine;

public class RoleGadiator : RoleBase
{
    public override void Init(GameScene scene, UIMgr uiMgr, string roleName, RoleConfigArgs configData, Vector3 spawnPos)
    {
        base.Init(scene, uiMgr, roleName, configData, spawnPos);
        animCtrl.Play(ManyKnivesDefine.AnimatorState.walk);
    }
    public override void Update()
    {
        base.Update();
        MoveFollow();
    }
}