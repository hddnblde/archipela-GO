using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace archipelaGO.UI.Windows
{
    public class GameHintWindow : UIWindow, IPointerClickHandler
    {
        [SerializeField]
        private TextMeshProUGUI m_text = null;

        public void SetHintText(string text)
        {
            if (m_text != null)
                m_text.text = text;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (shown)
                Hide();
        }
    }
}