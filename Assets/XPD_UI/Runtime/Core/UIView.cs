using UnityEngine;

namespace XPD.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIView : MonoBehaviour
    {
        public CanvasGroup CanvasGroup { get; private set; }

        protected virtual void Awake()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
            CanvasGroup.blocksRaycasts = false;
        }

        protected internal float InnerOnEnterAnimation()
        {
            return OnEnterAnimation();
        }
        protected internal void InnerOnPauseAnimation() => OnPauseAnimation();
        protected internal void InnerOnResumeAnimation() => OnResumeAnimation();
        protected internal float InnerOnExitAnimation()
        {
            return OnExitAnimation();
        }


        /// <summary>
        /// 当UI面板进入时执行的动画
        /// </summary>
        protected virtual float OnEnterAnimation() { return 0; }
        /// <summary>
        /// 当UI面板进入暂停时执行的动画
        /// </summary>
        protected virtual void OnPauseAnimation() { }
        /// <summary>
        /// 当UI面板恢复时执行的动画
        /// </summary>
        protected virtual void OnResumeAnimation() { }
        /// <summary>
        /// 当UI面板退出时执行的动画
        /// </summary>
        protected virtual float OnExitAnimation() { return 0; }


        //UI显示相关函数

    }
}

