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
        print("���Ž��붯��");
        return base.OnEnterAnimation();
    }

    protected override void OnPauseAnimation()
    {
        print("������ͣ����");
        base.OnPauseAnimation();
    }

    protected override void OnResumeAnimation()
    {
        print("���Żָ�����");
        base.OnResumeAnimation();
    }

    protected override float OnExitAnimation()
    {
        print("�����뿪����");
        return base.OnExitAnimation();
    }
}
