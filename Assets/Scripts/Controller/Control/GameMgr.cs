
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
        //uiMgr.OverBattle();TODO
        audioMgr.OverBattle();
    }

    public void GameReady()
    {
        //sceneMgr.Ready();TODO
        //uiMgr.Ready();TODO
        audioMgr.Ready();
    }
    public void StartBattle()
    {
        sceneMgr.StartBattle();
        //uiMgr.StartBattle();TODO
        audioMgr.StartBattle();
    }
    public void GameReset()
    {
        sceneMgr.ResetGame();
        //uiMgr.ResetGame();TODO
        audioMgr.ResetGame();
    }

    public void GameFailByUser()
    {
        GameFail();
    }
}
