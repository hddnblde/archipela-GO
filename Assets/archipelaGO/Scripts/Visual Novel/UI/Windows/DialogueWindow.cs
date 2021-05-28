using UnityEngine;

namespace archipelaGO.VisualNovel.UI.Windows
{
    public sealed class DialogueWindow : UIWindow
    {
        [SerializeField]
        private AnimatedText m_animatedText = null;

        public void Show(string text)
        {
            if (m_animatedText != null)
                m_animatedText.ShowText(text, Color.white);

            ShowCanvas(true);
        }
    }
}