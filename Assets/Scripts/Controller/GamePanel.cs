using UnityEngine;

public class GamePanel
{
    GameObject gameObject;
    int curLevel;
    GameObject bgGo;
    string bgName;
    public GamePanel() { }
    public GamePanel(GameObject go)
    {
        gameObject = go;
    }

    public void OnOpen(int levelId)
    {
        curLevel = levelId;
        bgName = "level" + levelId;
        bgGo = ResManager.LoadPrefab(bgName, Util.GetTransform(gameObject, "bgRoot"), Vector3.one, Vector3.zero);
    }

    void SetLayerIgnore()
    {
        Physics2D.IgnoreLayerCollision(ManyKnivesDefine.LayerID.collisionMap, ManyKnivesDefine.LayerID.collisionMap, false);
        Physics2D.IgnoreLayerCollision(ManyKnivesDefine.LayerID.collisionMap, ManyKnivesDefine.LayerID.collisionEnemy, false);
        Physics2D.IgnoreLayerCollision(ManyKnivesDefine.LayerID.collisionMap, ManyKnivesDefine.LayerID.triggerPlayer, true);
        Physics2D.IgnoreLayerCollision(ManyKnivesDefine.LayerID.collisionMap, ManyKnivesDefine.LayerID.triggerEnemy, true);
        Physics2D.IgnoreLayerCollision(ManyKnivesDefine.LayerID.collisionEnemy, ManyKnivesDefine.LayerID.collisionEnemy, true);
        Physics2D.IgnoreLayerCollision(ManyKnivesDefine.LayerID.collisionEnemy, ManyKnivesDefine.LayerID.triggerPlayer, true);
        Physics2D.IgnoreLayerCollision(ManyKnivesDefine.LayerID.collisionEnemy, ManyKnivesDefine.LayerID.triggerEnemy, true);
        Physics2D.IgnoreLayerCollision(ManyKnivesDefine.LayerID.triggerPlayer, ManyKnivesDefine.LayerID.triggerEnemy, false);
        Physics2D.IgnoreLayerCollision(ManyKnivesDefine.LayerID.triggerPlayer, ManyKnivesDefine.LayerID.triggerPlayer, true);
        Physics2D.IgnoreLayerCollision(ManyKnivesDefine.LayerID.triggerEnemy, ManyKnivesDefine.LayerID.triggerEnemy, true);

        Application.targetFrameRate = 60;
        Time.fixedDeltaTime = 0.02f;
    }

    public void Start()
    {
        SetLayerIgnore();
    }


    public void Update()
    {

    }

    public void LateUpdate()
    {

    }

    public void FixedUpdate()
    {

    }

    void Close()
    {
        ResManager.UnloadPrefab(bgName, bgGo);
    }
}
