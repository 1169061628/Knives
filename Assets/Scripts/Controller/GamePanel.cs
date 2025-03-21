using System.Collections;
using System.Collections.Generic;
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
