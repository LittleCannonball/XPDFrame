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
        /// ��UI������ʱִ�еĶ���
        /// </summary>
        protected virtual float OnEnterAnimation() { return 0; }
        /// <summary>
        /// ��UI��������ͣʱִ�еĶ���
        /// </summary>
        protected virtual void OnPauseAnimation() { }
        /// <summary>
        /// ��UI���ָ�ʱִ�еĶ���
        /// </summary>
        protected virtual void OnResumeAnimation() { }
        /// <summary>
        /// ��UI����˳�ʱִ�еĶ���
        /// </summary>
        protected virtual float OnExitAnimation() { return 0; }


        //UI��ʾ��غ���

    }
}

