using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XPD.UI;

[UIInfo("MainParent")]
public class MainParentController : UIControllerBase
{
    protected override void OnBind()
    {
        
    }

    protected override void OnCreate()
    {
        (UIView as MainParentView).button.onClick.AddListener(() => { UIFrame.Singleton.ShowUIPanel<Parent_02Controller>(); });
        
    }

}
