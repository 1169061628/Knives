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

    void SetSortingLayer(GameObject go)
    {
        var sps = go.transform.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < sps.Length; i++)
        {
            sps[i].sortingOrder = 100;
        }
    }

    public void OnOpen(int levelId)
    {
        curLevel = levelId;
        bgGo = ResManager.LoadPrefab("level" + levelId, Util.GetTransform(gameObject, "bgRoot"), Vector3.one, Vector3.zero);
        bgGo.transform.localScale = Vector3.one * 50;
        SetSortingLayer(bgGo);
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
