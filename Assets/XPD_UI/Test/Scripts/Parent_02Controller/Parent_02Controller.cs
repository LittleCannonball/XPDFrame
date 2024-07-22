using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XPD.UI;

[UIInfo("Parent_02")]
public class Parent_02Controller : UIControllerBase
{
    protected override void OnCreate()
    {
        (UIView as Parent_02View).button.onClick.AddListener(() => { UIFrame.Singleton.CloseUIPanel<Parent_02Controller>(); });
        (UIView as Parent_02View).button2.onClick.AddListener(() => { UIFrame.Singleton.ShowUIPanel<TopWin_01Con>(); });
        (UIView as Parent_02View).button3.onClick.AddListener(() => { UIFrame.Singleton.ShowUIPanel<TopWin_02Con>(); });

    }
    protected override void OnBind()
    {
        
    }
}
