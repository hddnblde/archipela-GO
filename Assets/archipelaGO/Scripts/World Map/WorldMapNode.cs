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

        public void Set(Sprite sprite, string label)
        {
            SetSprite(sprite);
            SetLabel(label.ToUpper());
        }

        private void SetSprite(Sprite sprite)
        {
            if (m_sprite != null)
                m_sprite.sprite = sprite;
        }

        private void SetLabel(string label)
        {
            if (m_label != null)
                m_label.text = label;
        }
    }
}