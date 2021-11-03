using UnityEngine;
using UnityEngine.UI;

namespace archipelaGO.WorldMap
{
    public class WorldMapNode : MonoBehaviour
    {
        [SerializeField]
        private Image m_sprite = null;

        [SerializeField]
        private Text m_label = null;

        [SerializeField]
        private Color m_enabledColor = Color.white;

        [SerializeField]
        private Color m_disabledColor = Color.gray;

        public void SetVisuals(Sprite sprite, string label, bool unlocked)
        {
            Color color = (unlocked ? m_enabledColor : m_disabledColor);
            SetSprite(sprite, color);
            SetLabel(label.ToUpper());
        }

        private void SetSprite(Sprite sprite, Color color)
        {
            if (m_sprite == null)
                return;

            m_sprite.sprite = sprite;
            m_sprite.color = color;
        }

        private void SetLabel(string label)
        {
            if (m_label != null)
                m_label.text = label;
        }
    }
}