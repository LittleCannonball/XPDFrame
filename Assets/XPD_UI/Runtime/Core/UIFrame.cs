using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XPD.UI
{
    public class UIFrame : MonoBehaviour
    {
        private readonly Dictionary<Type, UIControllerBase> cacheUIAssets = new();//����

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
        /// ��ʾһ��UI���
        /// </summary>
        /// <param name="isChild">�Ƿ�Ϊ����壬����岻��Ӱ��</param>
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
                //��ͣ��һ�������
                if(CurrentPanel != null)
                    PauseBaseOrParentUIPanel(CurrentPanel.GetRootParent());

                panelStack.Push(uIController);

                //��ʾ�����
                CreateUIPanel(uIController);
                //��������Լ�
                uIController.RecentlyParent = uIController;
                CurrentPanel = panelStack.Peek();
            }
            else
            {
                if(panelStack.Count == 0) throw new Exception("������������Ҫ����һ���������Ϊ������壡����");

                if (isChild)
                {
                    //��ͣ��һ�������
                    PauseAdjoiningUIPanel(CurrentPanel);

                    CurrentPanel.childList.Add(uIController);

                    CreateUIPanel(uIController);

                    uIController.RecentlyParent = CurrentPanel;
                    CurrentPanel = uIController;
                }
                else
                {
                    //�ֵ����������ͣ��һ�����
                    CurrentPanel.RecentlyParent.childList.Add(uIController);

                    CreateUIPanel(uIController);

                    uIController.RecentlyParent = CurrentPanel.RecentlyParent;
                    CurrentPanel = uIController;
                }
            }
        }

        /// <summary>
        /// �ر�һ��UI���
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void CloseUIPanel<T>() where T : UIControllerBase
        {
            if (panelStack.Count == 0 || CurrentPanel == null) return;

            //�ж�����Ƿ����
            Type type = typeof(T);
            UIControllerBase uIController;
            if(!cacheUIAssets.TryGetValue(type, out uIController))
            {
                Debug.LogError("��Ҫ�رյ�UI�����ڣ����UI�Ƿ��Ѿ��رջ�������");
                return;
            }

            if(uIController.UiInfo.PanelType == UIPanelType.Base)
            {
                if(uIController != panelStack.Peek())
                {
                    Debug.LogError($"�����ֻ�ܸ���ջ˳��رգ�ջ��Ԫ��Ϊ{panelStack.Peek().GetType().Name}");
                    return;
                }
                //�ر����������
                CloseBaseOrParentUI(uIController);

                panelStack.Pop();

                List<UIControllerBase> uIControllers = GetDeepestNodes(panelStack.Peek());
                //�ָ����

                CurrentPanel = uIControllers[uIControllers.Count - 1];

                ResumeAdjoiningUIPanel(CurrentPanel);
            }
            else
            {
                if(uIController.RecentlyParent.childList.Count > 1)
                {
                    //��ĩ�����
                    UIControllerBase controller = uIController.RecentlyParent.childList[uIController.RecentlyParent.childList.Count - 1];
                    if(controller == uIController)
                    {
                        CurrentPanel = uIController.RecentlyParent.childList[uIController.RecentlyParent.childList.Count - 2];
                    }
                    else
                    {
                        CurrentPanel = uIController.RecentlyParent.childList[uIController.RecentlyParent.childList.Count - 1];
                    }
                    //�ر����������
                    CloseBaseOrParentUI(uIController);
                }
                else
                {
                    CurrentPanel = uIController.RecentlyParent;
                    //�ر����������
                    CloseBaseOrParentUI(uIController);
                    ResumeAdjoiningUIPanel(CurrentPanel);
                }
            }
        }


        private void CreateUIPanel(UIControllerBase uIController)
        {
            if(uIController.UIView == null)//˵��û�б��������贴����
            {
                uIController.UiInfo.LoadUIPanelAsync(obj => {
                    GameObject insObj = Instantiate(obj,this.Canvas.transform);
                    uIController.InnerOnCreate(insObj);

                    OnCreate?.Invoke(uIController);

                    uIController.InnerOnBind();
                    StartCoroutine(PlayEnterAnimation(uIController));
                });
            }
            else//˵���Ѿ������ˣ����������ã�
            {
                uIController.UIView.gameObject.SetActive(true);
                uIController.InnerOnBind();
                StartCoroutine(PlayEnterAnimation(uIController));
            }
        }

        /// <summary>
        /// ����һ�����
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
        /// ��ͣһ����壬�������������
        /// </summary>
        /// <param name="uIController"></param>
        private void PauseBaseOrParentUIPanel(UIControllerBase uIController)
        {
            //�������
            if (uIController == null) return;

            foreach (var child in uIController.childList)
            {
                if (child == null) continue;
                PauseBaseOrParentUIPanel(child);
            }

            PauseUIPanel(uIController);
        }

        /// <summary>
        /// ��ͣһ����壬�Լ������ֵ����
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
        /// �ָ�һ����壬�Լ������ֵ����
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
        /// �ر�һ��UI���
        /// </summary>
        /// <param name="uIController"></param>
        /// <returns></returns>
        private IEnumerator PlayExitAnimation(UIControllerBase uIController)
        {
            uIController.UIView.CanvasGroup.blocksRaycasts = false;
            //������ж�Ӧ��ϵ
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
        /// �ָ�һ����壬�������������
        /// </summary>
        /// <param name="uIController"></param>
        private void ResumeBaseOrParentUIPanel(UIControllerBase uIController)
        {
            //�������
            if (uIController == null) return;

            foreach (var child in uIController.childList)
            {
                if (child == null) continue;
                ResumeBaseOrParentUIPanel(child);
            }

            ResumeUIPanel(uIController);
        }

        /// <summary>
        /// ��ͣһ��UI���
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
        /// �ָ�һ��UI���
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
        /// �ر�һ����壬�������������
        /// </summary>
        /// <param name="uIController"></param>
        private void CloseBaseOrParentUI(UIControllerBase uIController)
        {
            //�������
            if (uIController == null) return;

            foreach (var child in uIController.childList)
            {
                if (child == null) continue;
                CloseBaseOrParentUI(child);
            }
            StartCoroutine(PlayExitAnimation(uIController));
        }

        /// <summary>
        /// ���������ݶ�����UI�����������������
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


