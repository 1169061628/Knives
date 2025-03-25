
using DG.Tweening;
using UnityEngine;

public class CameraCtrl
{
    Scene sceneMgr;
    Camera camera;
    Transform cameraParent;

    float mapMinX, mapMaxX, mapMinY, mapMaxY;

    SpriteRenderer bgSpriteRenderer;
    Transform playerCenter;
    float nearClipPlane;

    Tween scaleTween;
    bool scaleFlag;

    Tween shakeTween;
    bool followPlayer;
    Transform focusTarget;

    const int cameraZ = -100;
    int minOSize = 14;
    int maxOSize = 18;
    int camMoveSp = 8;

    public float mapLeftBound, mapRightBound, mapTopBound, mapBottomBound;
    float orthographiSizeScale;
    float viewSizeX, viewSizeY;

    public CameraCtrl(Camera camera, Transform cp, SpriteRenderer sr, Scene scene)
    {
        this.camera = camera;
        bgSpriteRenderer = sr;
        cameraParent = cp;
        sceneMgr = scene;

        nearClipPlane = camera.nearClipPlane;
        camera.transparencySortMode = TransparencySortMode.CustomAxis;
        camera.transparencySortAxis = Vector3.up;

        sceneMgr.OnPauseStateChange.Add(PauseListener);
    }

    void PauseListener(bool pause)
    {
        var timeScale = pause ? 0 : 1;
        if (scaleTween != null) scaleTween.timeScale = timeScale;
        if (shakeTween != null) shakeTween.timeScale = timeScale;
    }

    public void Dispose()
    {
        KillScaleTween();
        KillShakeTween();
        sceneMgr.OnPauseStateChange.Remove(PauseListener);
    }

    public void Init(RoleBase player) // TODO 其实这里应该传RolePlayer先这么写后面改
    {
        //playerCenter = player.center;   TODO
        // 地图边界
        var bgCenter = bgSpriteRenderer.transform.position;
        var bgScale = bgSpriteRenderer.transform.lossyScale;
        var bgSize = bgSpriteRenderer.size * 0.5f;
        mapLeftBound = bgCenter.x - bgSize.x * bgScale.x;
        mapRightBound = bgCenter.x + bgSize.x * bgScale.x;
        mapBottomBound = bgCenter.y - bgSize.y * bgScale.y;
        mapTopBound = bgCenter.y + bgSize.y * bgScale.y;
        orthographiSizeScale = Screen.height / Screen.width * 0.5f;

        viewSizeX = minOSize / orthographiSizeScale * 0.5f;
        viewSizeY = viewSizeX * (Screen.height / Screen.width);
        bgSpriteRenderer.size = bgSpriteRenderer.size + new Vector2(viewSizeX * 2, viewSizeY * 2);
        //player.OnBigSword.Add(KillScaleTween);    // TODO
        scaleFlag = false;
    }

    public void Reset()
    {
        KillScaleTween();
        KillShakeTween();
        camera.transform.localPosition = Vector3.zero;
        var tmpPos = playerCenter.position;
        tmpPos.z = cameraZ;
        cameraParent.position = tmpPos;
        camera.orthographicSize = minOSize;
        followPlayer = true;
        focusTarget = null;
    }

    void BigSwordChange(bool value)
    {
        if (/*TODO!sceneMgr.overFlag && */scaleFlag != value)
        {
            KillScaleTween();
            var starSize = value ? minOSize : maxOSize;
            var tarSize = value ? maxOSize : minOSize;
            scaleTween = DOVirtual.Float(starSize, tarSize, 0.5f, val => camera.orthographicSize = val);
            scaleFlag = value;
            RefreshPause();
        }
    }

    // 相机聚集
    public void FocusPos(bool focus, Transform targetTran, bool scale)
    {
        followPlayer = !focus;
        if (focus) focusTarget = targetTran;
        if (scale)
        {
            KillScaleTween();
            scaleTween = DOVirtual.Float(camera.orthographicSize, 8, 0.3f, val => camera.orthographicSize = val);
            RefreshPause();
        }
    }

    Vector3 oriDir = new Vector3(1, 1, 0);
    void Shake(float duration, float strength, int vibrato, float randomNess, Vector3 dir = default)
    {
        KillShakeTween();
        if (dir == default) dir = oriDir;
        camera.transform.localPosition = Vector3.zero;
        camera.transform.DOShakePosition(duration, strength, vibrato, randomNess);
        RefreshPause();
    }
    // 玩家受伤
    public void Shake_PlayerHurt()
    {
        Shake(0.1f, 0.2f, 4, 90);
    }
    // 敌人受伤
    public void Shake_EnemyHurt()
    {
        Shake(0.05f, 0.1f, 3, 90);
    }
    // 拼刀
    public void Shake_BladeTrigger(Vector3 dir)
    {
        Shake(0.1f, 0.25f, 4, 90, dir);
    }
    // boss登场
    public void Shake_BossEntrance()
    {
        Shake(0.5f, 0.5f, 10, 90);
    }
    // boss去世
    public void Shake_BossDie()
    {
        Shake(0.5f, 0.8f, 10, 90);
    }

    public Vector2 LimitPosInMap(Vector2 vec2)
    {
        vec2.x = Mathf.Clamp(vec2.x, mapLeftBound, mapRightBound);
        vec2.y = Mathf.Clamp(vec2.y, mapBottomBound, mapTopBound);
        return vec2;
    }

    public void FixedUpdate()
    {
        // 非演出状态才跟随玩家
        if (followPlayer/*TODO && !sceneMgr.NoInjury*/)
        {
            var oSizeScale = camera.orthographicSize / minOSize;
            // 相机移动范围
            mapMaxX = mapLeftBound + viewSizeX * oSizeScale;
            mapMaxY = mapRightBound - viewSizeX * oSizeScale;
            mapMinY = mapBottomBound + viewSizeY * oSizeScale;
            mapMaxY = mapTopBound - viewSizeY * oSizeScale;
            var tmpPos = playerCenter.position;
            var clampedX = Mathf.Clamp(tmpPos.x, mapMinX, mapMaxX);
            var clampedY = Mathf.Clamp(tmpPos.y, mapMinY, mapMaxY);
            tmpPos = new Vector3(clampedX, clampedY, cameraZ);
            cameraParent.position = Vector3.Lerp(cameraParent.position, tmpPos, Time.deltaTime * camMoveSp);
        }
    }

    public void Update()
    {
        if (!followPlayer)
        {
            var tarPos = focusTarget.position;
            tarPos.z = cameraZ;
            cameraParent.position = tarPos;
            return;
        }
        if (true/*TODOsceneMgr.NoInjury*/)
        {
            var oSizeScale = camera.orthographicSize / minOSize;
            // 相机移动范围
            mapMaxX = mapLeftBound + viewSizeX * oSizeScale;
            mapMaxY = mapRightBound - viewSizeX * oSizeScale;
            mapMinY = mapBottomBound + viewSizeY * oSizeScale;
            mapMaxY = mapTopBound - viewSizeY * oSizeScale;
            var tmpPos = playerCenter.position;
            var clampedX = Mathf.Clamp(tmpPos.x, mapMinX, mapMaxX);
            var clampedY = Mathf.Clamp(tmpPos.y, mapMinY, mapMaxY);
            tmpPos = new Vector3(clampedX, clampedY, cameraZ);
            cameraParent.position = Vector3.Lerp(cameraParent.position, tmpPos, Time.deltaTime * camMoveSp);
        }
    }

    public Vector3 ViewportToWorldPoint(Vector2 pos)
    {
        return camera.ViewportToWorldPoint(new(pos.x, pos.y, nearClipPlane));
    }

    public Vector3 ScreenToWorldPoint(Vector2 pos)
    {
        return camera.ScreenToWorldPoint(new(pos.x, pos.y, nearClipPlane));
    }

    public Vector3 WorldToScreenPoint(Vector2 pos)
    {
        return camera.WorldToScreenPoint(new(pos.x, pos.y, nearClipPlane));
    }

    void KillScaleTween()
    {
        if (shakeTween != null)
        {
            shakeTween.Kill();
            shakeTween = null;
        }
    }

    void KillShakeTween()
    {
        if (scaleTween != null)
        {
            scaleTween.Kill();
            scaleTween = null;
        }
    }

    void RefreshPause()
    {
        PauseListener(sceneMgr.isPause);
    }
}
