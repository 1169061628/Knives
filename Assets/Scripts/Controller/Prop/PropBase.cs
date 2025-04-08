using UnityEngine;

public class PropBase : ItemBase
{
    int type;
    Collider2D collider;
    GameScene sceneMgr;
    public override void InitComponent()
    {
        collider = gameObject.GetComponent<Collider2D>();
    }

    public virtual void Init(GameScene scene, int type)
    {
        sceneMgr = scene;
        this.type = type;
        gameObject.layer = ManyKnivesDefine.LayerID.triggerEnemy;
    }

    public int Trigger()
    {
        sceneMgr.PropPoolPushOne(type, this);
        return type;
    }
}
