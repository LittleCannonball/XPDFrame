using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace XPD.UI
{
    public abstract class UIControllerBase
    {
        public bool isImmediatelyDestroy;
        public UIInfoAttribute UiInfo { get; private set; }
        public UIControllerBase RecentlyParent { get; set; }
        public UIView UIView { get; private set; }

        [HideInInspector] public readonly List<UIControllerBase> childList = new();
        public UIControllerBase()
        {
            UiInfo = this.GetType().GetCustomAttribute<UIInfoAttribute>();
            UiInfo.LoadUIPanelAsync(null, OnLoadUIException);
        }


        protected internal void InnerOnCreate(GameObject uiPanelObj)
        {
            UIView = uiPanelObj.GetComponent<UIView>();
            OnCreate();
        }
            
        protected internal void InnerOnBind() => OnBind();
        protected internal void InnerOnShow() => OnShow();
        protected internal void InnerOnHide() => OnHide();
        protected internal void InnerOnUnBind() => OnUnBind();
        protected internal void InnerOnDies() => OnDies();

        /// <summary>
        /// ������ʱ���ã�ʵ����֮��
        /// </summary>
        protected virtual void OnCreate() { }
        /// <summary>
        /// ���¼�
        /// </summary>
        protected abstract void OnBind();
        /// <summary>
        /// �����ʾ��ʱ�����
        /// </summary>
        protected virtual void OnShow() { }
        /// <summary>
        /// ��屻�����˵���
        /// </summary>
        protected virtual void OnHide() { }
        /// <summary>
        /// ��ʱӦ��ȡ���¼���
        /// </summary>
        protected virtual void OnUnBind() { }
        /// <summary>
        /// ��屻����ʱ����
        /// </summary>
        protected virtual void OnDies() { }

        /// <summary>
        /// ��ȡ�����
        /// </summary>
        /// <returns></returns>
        public UIControllerBase GetRootParent()
        {
            UIControllerBase root = null;
            GetChile(this);
            return root;

            void GetChile(UIControllerBase controllerBase)
            {
                if(controllerBase.RecentlyParent == controllerBase || controllerBase.RecentlyParent == null)
                {
                    root = controllerBase;
                    return;
                }
                else
                {
                    GetChile(controllerBase.RecentlyParent);
                }
            }
        }

        /// <summary>
        /// ��AB��UI��Դ����ʧ�ܺ�ִ��
        /// </summary>
        /// <param name="action"></param>
        private void OnLoadUIException(Exception e)
        {
            Debug.LogError($"UI����AB����Դ����ʧ�ܣ����ش���·����Ϣ��{UiInfo.UIPath}" + e);
        }
    }
}

