using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XPD.UI
{
    public class UIFrame : MonoBehaviour
    {
        private readonly Dictionary<Type, UIControllerBase> cacheUIAssets = new();//缓存

        private readonly Stack<UIControllerBase> panelStack = new();

        private static UIFrame singleton;

        public static UIFrame Singleton => singleton;
        public UIControllerBase CurrentPanel { get; private set; }
        public Canvas Canvas { get; private set; }
        public Action<UIControllerBase> OnCreate { get; set; }
        public Action<UIControllerBase> OnShow { get; set; }
        public Action<UIControllerBase> OnHide { get; set; }
        public Action<UIControllerBase> OnDies { get; set; }

        private void Awake()
        {
            Canvas = GetComponent<Canvas>();
            singleton = this;
        }

        /// <summary>
        /// 显示一块UI面板
        /// </summary>
        /// <param name="isChild">是否为子面板，主面板不受影响</param>
        public void ShowUIPanel<T>(bool isChild = true) where T : UIControllerBase, new()
        {
            Type type = typeof(T);
            UIControllerBase uIController;
            if(!cacheUIAssets.TryGetValue(type, out uIController))
            {
                uIController = new T();
                cacheUIAssets.Add(type, uIController);
            }
            
            if(uIController.UiInfo.PanelType == UIPanelType.Base)
            {
                //暂停上一块子面板
                if(CurrentPanel != null)
                    PauseBaseOrParentUIPanel(CurrentPanel.GetRootParent());

                panelStack.Push(uIController);

                //显示新面板
                CreateUIPanel(uIController);
                //父面板是自己
                uIController.RecentlyParent = uIController;
                CurrentPanel = panelStack.Peek();
            }
            else
            {
                if(panelStack.Count == 0) throw new Exception("场景中最少需要包含一个主面板作为启动面板！！！");

                if (isChild)
                {
                    //暂停上一块子面板
                    PauseAdjoiningUIPanel(CurrentPanel);

                    CurrentPanel.childList.Add(uIController);

                    CreateUIPanel(uIController);

                    uIController.RecentlyParent = CurrentPanel;
                    CurrentPanel = uIController;
                }
                else
                {
                    //兄弟面板无需暂停上一个面板
                    CurrentPanel.RecentlyParent.childList.Add(uIController);

                    CreateUIPanel(uIController);

                    uIController.RecentlyParent = CurrentPanel.RecentlyParent;
                    CurrentPanel = uIController;
                }
            }
        }

        /// <summary>
        /// 关闭一个UI面板
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void CloseUIPanel<T>() where T : UIControllerBase
        {
            if (panelStack.Count == 0 || CurrentPanel == null) return;

            //判断面板是否存在
            Type type = typeof(T);
            UIControllerBase uIController;
            if(!cacheUIAssets.TryGetValue(type, out uIController))
            {
                Debug.LogError("需要关闭的UI不存在，检查UI是否已经关闭或者销毁");
                return;
            }

            if(uIController.UiInfo.PanelType == UIPanelType.Base)
            {
                if(uIController != panelStack.Peek())
                {
                    Debug.LogError($"主面板只能根据栈顺序关闭，栈顶元素为{panelStack.Peek().GetType().Name}");
                    return;
                }
                //关闭所有子面板
                CloseBaseOrParentUI(uIController);

                panelStack.Pop();

                List<UIControllerBase> uIControllers = GetDeepestNodes(panelStack.Peek());
                //恢复面板

                CurrentPanel = uIControllers[uIControllers.Count - 1];

                ResumeAdjoiningUIPanel(CurrentPanel);
            }
            else
            {
                if(uIController.RecentlyParent.childList.Count > 1)
                {
                    //最末端面板
                    UIControllerBase controller = uIController.RecentlyParent.childList[uIController.RecentlyParent.childList.Count - 1];
                    if(controller == uIController)
                    {
                        CurrentPanel = uIController.RecentlyParent.childList[uIController.RecentlyParent.childList.Count - 2];
                    }
                    else
                    {
                        CurrentPanel = uIController.RecentlyParent.childList[uIController.RecentlyParent.childList.Count - 1];
                    }
                    //关闭所有子面板
                    CloseBaseOrParentUI(uIController);
                }
                else
                {
                    CurrentPanel = uIController.RecentlyParent;
                    //关闭所有子面板
                    CloseBaseOrParentUI(uIController);
                    ResumeAdjoiningUIPanel(CurrentPanel);
                }
            }
        }


        private void CreateUIPanel(UIControllerBase uIController)
        {
            if(uIController.UIView == null)//说明没有被创建（需创建）
            {
                uIController.UiInfo.LoadUIPanelAsync(obj => {
                    GameObject insObj = Instantiate(obj,this.Canvas.transform);
                    uIController.InnerOnCreate(insObj);

                    OnCreate?.Invoke(uIController);

                    uIController.InnerOnBind();
                    StartCoroutine(PlayEnterAnimation(uIController));
                });
            }
            else//说明已经创建了（需重新启用）
            {
                uIController.UIView.gameObject.SetActive(true);
                uIController.InnerOnBind();
                StartCoroutine(PlayEnterAnimation(uIController));
            }
        }

        /// <summary>
        /// 销毁一块面板
        /// </summary>
        /// <param name="uIController"></param>
        private void DestroyUIPanel(UIControllerBase uIController)
        {
            cacheUIAssets.Remove(uIController.GetType());
            OnDies?.Invoke(uIController);
            uIController.InnerOnDies();
            Destroy(uIController.UIView.gameObject);
        }

        /// <summary>
        /// 暂停一块面板，包括所有子面板
        /// </summary>
        /// <param name="uIController"></param>
        private void PauseBaseOrParentUIPanel(UIControllerBase uIController)
        {
            //后序遍历
            if (uIController == null) return;

            foreach (var child in uIController.childList)
            {
                if (child == null) continue;
                PauseBaseOrParentUIPanel(child);
            }

            PauseUIPanel(uIController);
        }

        /// <summary>
        /// 暂停一个面板，以及所有兄弟面板
        /// </summary>
        /// <param name="uIController"></param>
        private void PauseAdjoiningUIPanel(UIControllerBase uIController)
        {
            if (uIController.RecentlyParent.childList.Count == 0)
            {
                PauseUIPanel(uIController);
                return;
            }
            for (int i = 0; i < uIController.RecentlyParent.childList.Count; i++)
            {
                PauseUIPanel(uIController.RecentlyParent.childList[i]);
            }
        }

        /// <summary>
        /// 恢复一个面板，以及所有兄弟面板
        /// </summary>
        /// <param name="uIController"></param>
        private void ResumeAdjoiningUIPanel(UIControllerBase uIController)
        {
            if (uIController.RecentlyParent.childList.Count == 0)
            {
                ResumeUIPanel(uIController);
                return;
            }
            for (int i = 0; i < uIController.RecentlyParent.childList.Count; i++)
            {
                ResumeUIPanel(uIController.RecentlyParent.childList[i]);
            }
        }

        private IEnumerator PlayEnterAnimation(UIControllerBase uIController)
        {
            yield return new WaitForSeconds(uIController.UIView.InnerOnEnterAnimation());
            uIController.UIView.CanvasGroup.blocksRaycasts = true;
            OnShow?.Invoke(uIController);
            uIController.InnerOnShow();
        }

        /// <summary>
        /// 关闭一个UI面板
        /// </summary>
        /// <param name="uIController"></param>
        /// <returns></returns>
        private IEnumerator PlayExitAnimation(UIControllerBase uIController)
        {
            uIController.UIView.CanvasGroup.blocksRaycasts = false;
            //清空树中对应关系
            uIController.RecentlyParent.childList.Remove(uIController);
            uIController.RecentlyParent = null;

            yield return new WaitForSeconds(uIController.UIView.InnerOnExitAnimation());
            uIController.UIView.gameObject.SetActive(false);
            OnHide?.Invoke(uIController);
            uIController.InnerOnHide();
            uIController.InnerOnUnBind();
            if (uIController.isImmediatelyDestroy)
                DestroyUIPanel(uIController);
        }

        /// <summary>
        /// 恢复一块面板，包括所有子面板
        /// </summary>
        /// <param name="uIController"></param>
        private void ResumeBaseOrParentUIPanel(UIControllerBase uIController)
        {
            //后序遍历
            if (uIController == null) return;

            foreach (var child in uIController.childList)
            {
                if (child == null) continue;
                ResumeBaseOrParentUIPanel(child);
            }

            ResumeUIPanel(uIController);
        }

        /// <summary>
        /// 暂停一块UI面板
        /// </summary>
        /// <param name="uIController"></param>
        private void PauseUIPanel(UIControllerBase uIController)
        {
            uIController.UIView.CanvasGroup.blocksRaycasts = false;
            uIController.UIView.InnerOnPauseAnimation();
            OnHide?.Invoke(uIController);
            uIController.InnerOnHide();
        }

        /// <summary>
        /// 恢复一块UI面板
        /// </summary>
        /// <param name="uIController"></param>
        private void ResumeUIPanel(UIControllerBase uIController)
        {
            uIController.UIView.InnerOnResumeAnimation();
            uIController.UIView.CanvasGroup.blocksRaycasts = true;
            OnShow?.Invoke(uIController);
            uIController.InnerOnShow();
            
        }

        /// <summary>
        /// 关闭一块面板，包括所有子面板
        /// </summary>
        /// <param name="uIController"></param>
        private void CloseBaseOrParentUI(UIControllerBase uIController)
        {
            //后序遍历
            if (uIController == null) return;

            foreach (var child in uIController.childList)
            {
                if (child == null) continue;
                CloseBaseOrParentUI(child);
            }
            StartCoroutine(PlayExitAnimation(uIController));
        }

        /// <summary>
        /// 搜索最深梯队所有UI，（深度优先搜索）
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public List<UIControllerBase> GetDeepestNodes(UIControllerBase root)
        {
            if(root == null) return new();

            Queue<UIControllerBase> queue = new();
            queue.Enqueue(root);

            List<UIControllerBase> currentLevelNodes = new();

            while (queue.Count > 0)
            {
                int levelSize = queue.Count;
                currentLevelNodes.Clear();

                for (int i = 0; i < levelSize; i++)
                {
                    UIControllerBase currentNode = queue.Dequeue();
                    currentLevelNodes.Add(currentNode);

                    foreach (var item in currentNode.childList)
                    {
                        queue.Enqueue(item);
                    }
                }

            }

            return currentLevelNodes;
        }

        private void Update()
        {
            print(CurrentPanel.GetType().Name);
        }

    }

    
    
    public enum UIPanelType
    {
        Base = 0,
        Overlay = 1,
        Popup = 2
    }
}


