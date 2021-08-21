using System.Collections;
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