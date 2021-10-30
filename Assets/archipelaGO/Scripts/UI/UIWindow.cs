using UnityEngine;

namespace archipelaGO.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIWindow : MonoBehaviour
    {
        private CanvasGroup m_canvasGroup = null;
        public delegate void Activity();

        public event Activity OnShow;
        public event Activity OnHide;

        public bool shown => m_canvasGroup.blocksRaycasts;

        protected virtual void Awake() =>
            m_canvasGroup = GetComponent<CanvasGroup>();

        private void ShowCanvas(bool shown)
        {
            if (m_canvasGroup == null || m_canvasGroup.interactable == shown)
                return;

            m_canvasGroup.alpha = (shown ? 1f : 0f);
            m_canvasGroup.blocksRaycasts = shown;
            m_canvasGroup.interactable = shown;

            if (shown)
                OnShow?.Invoke();
            else
                OnHide?.Invoke();
        }

        protected virtual void Show() => ShowCanvas(true);
        public virtual void Hide() => ShowCanvas(false);
    }
}