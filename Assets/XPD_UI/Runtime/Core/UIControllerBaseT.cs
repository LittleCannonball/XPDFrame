using XPD.UI;

namespace XPD.UI
{
    public abstract class UIControllerBase<T> : UIControllerBase where T : UIView
    {
        protected abstract void OnBindUI();
        protected override void OnBind() => OnBindUI();

        protected T GetView()
        {
            return UIView as T;
        }

    }

}
