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
        Debug.Log("UI��������");
    }
    protected override void OnBindUI()
    {
        Debug.Log("�����");
    }
    protected override void OnShow()
    {
        Debug.Log("UI��ʾ��");
    }

    protected override void OnHide()
    {
        Debug.Log("UI������");
    }

    protected override void OnUnBind()
    {
        Debug.Log("UI�����");
    }
}
