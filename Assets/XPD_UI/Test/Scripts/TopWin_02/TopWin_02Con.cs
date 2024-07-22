using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XPD.UI;

[UIInfo(UIPanelType.Overlay, "TopWin_02")]
public class TopWin_02Con : UIControllerBase<TopWin_02View>
{
    protected override void OnCreate()
    {
        GetView().button.onClick.AddListener(() => { UIFrame.Singleton.CloseUIPanel<TopWin_02Con>(); });
        
    }
    protected override void OnBindUI()
    {
        
    }


}
