using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XPD.UI;

[UIInfo(UIPanelType.Overlay,"TopWin_01")]
public class TopWin_01Con : UIControllerBase<TopWin_01View>
{
    protected override void OnCreate()
    {
        GetView().button.onClick.AddListener(() => { UIFrame.Singleton.CloseUIPanel<TopWin_01Con>(); });
        GetView().button2.onClick.AddListener(() => { UIFrame.Singleton.ShowUIPanel<TopWin_02Con>(false); });
        Debug.Log("UI被创建了");
    }
    protected override void OnBindUI()
    {
        Debug.Log("绑定组件");
    }
    protected override void OnShow()
    {
        Debug.Log("UI显示了");
    }

    protected override void OnHide()
    {
        Debug.Log("UI隐藏了");
    }

    protected override void OnUnBind()
    {
        Debug.Log("UI解除绑定");
    }
}
