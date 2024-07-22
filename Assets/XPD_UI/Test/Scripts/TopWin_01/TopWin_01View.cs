using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XPD.UI;

public class TopWin_01View : UIView
{
    public Button button;
    public Button button2;

    protected override float OnEnterAnimation()
    {
        print("播放进入动画");
        return base.OnEnterAnimation();
    }

    protected override void OnPauseAnimation()
    {
        print("播放暂停动画");
        base.OnPauseAnimation();
    }

    protected override void OnResumeAnimation()
    {
        print("播放恢复动画");
        base.OnResumeAnimation();
    }

    protected override float OnExitAnimation()
    {
        print("播放离开动画");
        return base.OnExitAnimation();
    }
}
