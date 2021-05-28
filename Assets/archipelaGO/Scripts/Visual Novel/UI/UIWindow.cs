using UnityEngine;

namespace archipelaGO.VisualNovel.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIWindow : MonoBehaviour
    {
        private CanvasGroup m_canvasGroup = null;

        protected virtual void Awake() =>
            m_canvasGroup = GetComponent<CanvasGroup>();

        protected void ShowCanvas(bool shown)
        {
            if (m_canvasGroup == null)
                return;

            m_canvasGroup.alpha = (shown ? 1f : 0f);
            m_canvasGroup.blocksRaycasts = shown;
            m_canvasGroup.interactable = shown;
        }
    }
}