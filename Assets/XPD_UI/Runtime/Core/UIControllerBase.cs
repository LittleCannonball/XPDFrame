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
        /// 当创建时调用，实例化之后
        /// </summary>
        protected virtual void OnCreate() { }
        /// <summary>
        /// 绑定事件
        /// </summary>
        protected abstract void OnBind();
        /// <summary>
        /// 面板显示的时候调用
        /// </summary>
        protected virtual void OnShow() { }
        /// <summary>
        /// 面板被隐藏了调用
        /// </summary>
        protected virtual void OnHide() { }
        /// <summary>
        /// 此时应该取消事件绑定
        /// </summary>
        protected virtual void OnUnBind() { }
        /// <summary>
        /// 面板被销毁时调用
        /// </summary>
        protected virtual void OnDies() { }

        /// <summary>
        /// 获取根面板
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
        /// 当AB的UI资源加载失败后执行
        /// </summary>
        /// <param name="action"></param>
        private void OnLoadUIException(Exception e)
        {
            Debug.LogError($"UI面板的AB包资源加载失败，加载错误路径信息：{UiInfo.UIPath}" + e);
        }
    }
}

