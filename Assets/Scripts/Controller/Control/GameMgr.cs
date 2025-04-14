
public class GameMgr
{
    UIMgr uiMgr;
    GameScene sceneMgr;
    AudioMgr audioMgr;
    public int level;

    public void Init(GameScene scene, UIMgr uiMgr, AudioMgr audioMgr, int level)
    {
        sceneMgr = scene;
        this.uiMgr = uiMgr;
        this.audioMgr = audioMgr;
        this.level = level;
    }

    public void GameWin()
    {
        sceneMgr.OverBattle();
    }

    public void GameFail()
    {
        sceneMgr.OverBattle();
        uiMgr.OverBattle();
        audioMgr.OverBattle();
    }

    public void GameReady(UnityEngine.Transform canvas, int level)
    {
        sceneMgr.Ready(canvas, level);
        uiMgr.Ready();
        audioMgr.Ready();
    }
    public void StartBattle()
    {
        sceneMgr.StartBattle();
        uiMgr.StartBattle();
        audioMgr.StartBattle();
    }
    public void GameReset()
    {
        sceneMgr.ResetGame();
        uiMgr.ResetGame();
        audioMgr.ResetGame();
    }

    public void GameFailByUser()
    {
        GameFail();
    }
}
