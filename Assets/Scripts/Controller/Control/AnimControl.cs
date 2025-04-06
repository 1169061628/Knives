using UnityEngine;
public class AnimControl
{
    Animator animator;
    int curState;
    int moveSp;
    int atkSp;
    bool freezeFlag;

    EventHandler<int> moveSpBind, atkBind;

    public AnimControl(Animator animator, EventHandler<int> moveSpBind, EventHandler<int> atkBind)
    {
        this.animator = animator;
        this.moveSpBind = moveSpBind;
        this.atkBind = atkBind;
        moveSpBind.Add(MoveSpeedListener);
        atkBind.Add(AtkSpeedListener);
    }

    void AtkSpeedListener(int value)
    {
        atkSp = value;
        SetAnimSpeed();
    }
    void MoveSpeedListener(int value)
    {
        moveSp = value;
        SetAnimSpeed();
    }

    void SetAnimSpeed()
    {
        var sp = 1f;
        if (freezeFlag) sp = 0;
        else
        {
            if (curState == ManyKnivesDefine.AnimatorState.attack) sp = atkSp;
            else if (curState == ManyKnivesDefine.AnimatorState.walk) sp = moveSp / 3;
        }
        animator.speed = sp;
    }

    public void Freeze(bool value)
    {
        freezeFlag = value;
        SetAnimSpeed();
    }
    public void Play(int animState)
    {
        if (curState != animState)
        {
            curState = animState;
            animator.SetInteger(ManyKnivesDefine.AnimatorName, animState);
        }
        SetAnimSpeed();
    }
    public void Dispose()
    {
        moveSpBind.Remove(MoveSpeedListener);
        atkBind.Remove(AtkSpeedListener);
    }
}