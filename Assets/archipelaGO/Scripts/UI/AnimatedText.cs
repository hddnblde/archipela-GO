using System.Collections;
using UnityEngine;
using TMPro;

namespace archipelaGO.UI
{
    public class AnimatedText : MonoBehaviour
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
        private string m_cachedText = string.Empty;
        #endregion


        #region Public Methods
        public IEnumerator ShowText(string text, Color color)
        {
            if (m_text == null)
                yield break;

            m_cachedText = text;
            m_characterCount = text.Length;
            m_text.faceColor = color;

            yield return ScrollText();
        }

        public void SkipTextScrolling() =>
            m_currentTime = GetTotalDuration();
        #endregion


        #region Internal Methods
        private void ShowTextAtPosition(float position)
        {
            position = Mathf.Clamp(position, 0f, m_characterCount);
            (int index, float alpha) character = GetCharacterIndexAndAlpha(position);
            string text = GetTextUpToPosition3(character.index, character.alpha);
            m_text.text = text;
        }

        private IEnumerator ScrollText()
        {
            float duration = GetTotalDuration();

            for (m_currentTime = 0f; m_currentTime < duration; m_currentTime += Time.deltaTime)
            {
                float t = Mathf.InverseLerp(0f, duration, m_currentTime);
                float position = Mathf.Lerp(0, m_characterCount, t);
                ShowTextAtPosition(position);
                yield return null;
            }

            yield return null;
            ShowTextAtPosition(m_characterCount);
        }

        private float GetTotalDuration()
        {
            if (m_scrollSpeed <= 0f)
                return Mathf.Infinity;

            return (m_characterCount / m_scrollSpeed);
        }

        private string GetTextUpToPosition3(int index, float alpha)
        {
            if (m_characterCount <= 0)
                return string.Empty;

            string text = m_cachedText.Substring(0, index);

            if (alpha < 1f)
            {
                Color color = new Color(m_color.r, m_color.g, m_color.b, alpha);
                string htmlStringRGBA = ColorUtility.ToHtmlStringRGBA(color);
                text += GetFormatedTextWithColor(m_cachedText[index], color);
            }
            else
                text += m_cachedText[index];

            int nextIndex = index + 1;

            if (nextIndex < m_characterCount)
                text += GetFormatedTextWithColor(m_cachedText.Substring(index + 1), Color.clear);

            return text;
        }

        private string GetFormatedTextWithColor(char c, Color color) =>
            GetFormatedTextWithColor($"{ c }", color);

        private string GetFormatedTextWithColor(string text, Color color)
        {
            string htmlStringRGBA = ColorUtility.ToHtmlStringRGBA(color);
            return $"<#{ htmlStringRGBA }>{ text }</color>";
        }

        private (int index, float alpha) GetCharacterIndexAndAlpha(float position)
        {
            if (m_characterCount <= 0)
                return (-1, 0f);

            int index = Mathf.FloorToInt(position);
            float alpha = (position - index);

            if (index >= m_characterCount)
            {
                index = (m_characterCount - 1);
                alpha = 1f;
            }

            return (index, alpha);
        }
        #endregion
    }
}