using DG.Tweening;
using UnityEngine;

/// <summary>
/// 角色类
/// </summary>
public class RolePlayer : RoleBase
{
    public CircleCollider2D collectRangeCollider;
    private Tweener repulseTween;
    private bool repulseFlag;

    void PauseListener(bool pause)
    {
        base.PauseListener(pause);
        int timeScale = pause ? 0 : 1;
    }

    /// <summary>
    /// 被击退
    /// </summary>
    void PlayerRepulse(Vector3 pos)
    {
        KillRepulseTween();
        repulseFlag = true;
        repulseTween = transform.DOMove(pos, 0.5f).SetEase(Ease.OutQuad);
        repulseTween.OnComplete(() =>
        {
            repulseFlag = false;
        });
        RefreshPause();
    }

    void KillRepulseTween()
    {
        if (!repulseTween.Equals(null))
        {
            repulseTween.Kill();
        }
    }
}
