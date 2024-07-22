using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace XPD.UI
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class UIInfoAttribute : Attribute
    {
        private readonly UIPanelType panelType;

        private readonly string uiPath;

        private readonly LoadUIWay loadUIWay;

        private AsyncOperationHandle<GameObject> addressableHandleObj;

        private ResourceRequest resourcesObj;

        public UIPanelType PanelType => panelType;
        public string UIPath => uiPath;
        public LoadUIWay LoadUIWay => loadUIWay;
        //�������������ڻ�ȡ��������ؽ���
        public AsyncOperationHandle<GameObject> AddressableHandleObj => addressableHandleObj;
        public ResourceRequest ResourcesObj => resourcesObj;


        public UIInfoAttribute(string path) {
            this.uiPath = path;
            panelType = UIPanelType.Base;
            loadUIWay = LoadUIWay.Addressable;
        }
        public UIInfoAttribute(UIPanelType panelType, string path) {
            this.uiPath = path;
            this.panelType = panelType;
            loadUIWay = LoadUIWay.Addressable;
        }
        public UIInfoAttribute(LoadUIWay loadUIWay, string path)
        {
            this.uiPath = path;
            this.panelType = UIPanelType.Base;
            this.loadUIWay = loadUIWay;
        }
        public UIInfoAttribute(UIPanelType panelType, LoadUIWay loadUIWay, string path)
        {
            this.uiPath = path;
            this.panelType = panelType;
            this.loadUIWay = loadUIWay;
        }

        public void LoadUIPanelAsync(Action<GameObject> onSucceed, Action<Exception> onFailure = null)
        {
            if (loadUIWay == LoadUIWay.Addressable)
            {
                if(!addressableHandleObj.IsValid())
                {
                    addressableHandleObj = Addressables.LoadAssetAsync<GameObject>(UIPath);
                }
                else
                {
                    if (addressableHandleObj.IsDone)
                    {
                        onSucceed?.Invoke(addressableHandleObj.Result);
                        return;
                    }
                }
                addressableHandleObj.Completed += h => {
                    if (h.Status == AsyncOperationStatus.Succeeded)
                        onSucceed?.Invoke(h.Result);
                    else
                        onFailure?.Invoke(new Exception("UI������ʧ�ܣ�AB������ʧ�ܡ�"));
                };
            }
            else if (loadUIWay == LoadUIWay.Resources)
            {
                if(resourcesObj == null)
                {
                    resourcesObj = Resources.LoadAsync(UIPath);
                }
                else
                {
                    if(resourcesObj.isDone)
                    {
                        onSucceed?.Invoke((GameObject)resourcesObj.asset);
                        return;
                    }
                }
                resourcesObj.completed += a => {
                    if (resourcesObj.asset != null)
                        onSucceed?.Invoke((GameObject)resourcesObj.asset);
                    else
                        onFailure?.Invoke(new Exception("UI������ʧ�ܣ�Resources��Դ����ʧ�ܡ�"));
                };
            }
        }

        public void ReleaseUIPanel()
        {
            if (loadUIWay == LoadUIWay.Addressable && addressableHandleObj.IsValid())
            {
                Addressables.Release(addressableHandleObj);
            }
            else if (loadUIWay == LoadUIWay.Resources && resourcesObj != null)
            {
                Resources.UnloadAsset(resourcesObj.asset);
                resourcesObj = null;
            }
        }
    }

    public enum LoadUIWay
    {
        Addressable,
        Resources
    }
}

