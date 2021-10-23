using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Word = archipelaGO.WordBank.Word;

namespace archipelaGO.UI
{
    public class AnimatedText : MonoBehaviour, IPointerClickHandler
    {
        #region Fields
        [SerializeField]
        private List<PreferredFont> m_fontSizes = new List<PreferredFont>();

        [SerializeField]
        private TextMeshProUGUI m_text = null;

        [SerializeField]
        private Color m_color = Color.white;

        [SerializeField]
        private float m_scrollSpeed = 29f;

        private int m_characterCount = 0;
        private float m_currentTime = 0f;
        private WordBank m_cachedWordBank = null;

        public static event WordSelected OnWordSelected;
        public delegate void WordSelected(Word word);
        private int m_previousScreenWidth = -1;
        #endregion


        #region Data Structure
        [System.Serializable]
        private struct PreferredFont
        {
            [SerializeField]
            private float m_size;

            [SerializeField]
            private int m_preferredScreenWidth;

            public float size => m_size;
            public int preferredScreenWidth => m_preferredScreenWidth;
        }
        #endregion


        #region MonoBehaviour Implementation
        private void Update()
        {
            if (m_previousScreenWidth == Screen.width)
                return;

            m_previousScreenWidth = Screen.width;
            InitializeFontSize();
        }
        #endregion


        #region Pointer Click Handler Implementation
        public void OnPointerClick(PointerEventData eventData)
        {
            if (m_cachedWordBank == null)
                return;

            int selectedHyperlinkIndex;

            if (!HyperlinkWasSelected(Input.mousePosition, out selectedHyperlinkIndex))
                return;

            var link = m_text.textInfo.linkInfo[selectedHyperlinkIndex];
            int hyperlinkIDValue;

            if (!int.TryParse(link.GetLinkID(), out hyperlinkIDValue))
                return;
            
            if (hyperlinkIDValue < 0 || hyperlinkIDValue >= m_cachedWordBank.wordCount)
                return;

            Word selectedWord = m_cachedWordBank.GetWord(hyperlinkIDValue);
            OnWordSelected?.Invoke(selectedWord);
        }
        #endregion


        #region Public Methods
        public IEnumerator ShowText(string text, Color color, WordBank wordBank)
        {
            if (m_text == null)
                yield break;

            m_characterCount = text.Length;
            m_text.faceColor = color;
            m_text.text = text;
            m_cachedWordBank = wordBank;

            yield return ScrollText();
        }

        public void SkipTextScrolling() =>
            m_currentTime = GetTotalDuration();
        #endregion


        #region Internal Methods
        private void InitializeFontSize() =>
            m_text.fontSize = GetPreferredFontSize(Screen.width);

        private float GetPreferredFontSize(int screenWidth)
        {
            if (m_text == null)
                return 0;
            
            if (m_fontSizes == null || m_fontSizes.Count <= 0)
                goto End;

            List<PreferredFont> orderedFontSizes = m_fontSizes.
                OrderByDescending(f => f.preferredScreenWidth).ToList();

            foreach (PreferredFont preferredFont in orderedFontSizes)
            {
                if (screenWidth >= preferredFont.preferredScreenWidth)
                    return preferredFont.size;
            }

            End:
            return m_text.fontSize;
        }

        private IEnumerator ScrollText()
        {
            float duration = GetTotalDuration();

            for (m_currentTime = 0f; m_currentTime < duration; m_currentTime += Time.deltaTime)
            {
                float t = Mathf.InverseLerp(0f, duration, m_currentTime);
                int position = Mathf.FloorToInt(Mathf.Lerp(0, m_characterCount, t));

                if (m_text != null)
                    m_text.maxVisibleCharacters = position;

                yield return null;
            }

            yield return null;

            if (m_text != null)
                m_text.maxVisibleCharacters = m_characterCount;
        }

        private float GetTotalDuration()
        {
            if (m_scrollSpeed <= 0f)
                return Mathf.Infinity;

            return (m_characterCount / m_scrollSpeed);
        }

        private bool HyperlinkWasSelected(Vector3 pointerPosition, out int index)
        {
            index = TMP_TextUtilities.FindIntersectingLink(m_text, pointerPosition, null);

            return (index != -1);
        }
        #endregion
    }
}