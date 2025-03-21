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
        bgGo = ResManager.LoadPrefab("level" + levelId, Util.GetTransform(gameObject, "bgRoot"), Vector3.one, Vector3.zero);
    }

    public void Start()
    {

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
