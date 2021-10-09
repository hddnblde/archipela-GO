using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

namespace archipelaGO.Puzzle.RevealAnimation
{
    [CreateAssetMenu(fileName = "Crossword Reveal Animation", menuName = "archipelaGO/Game Module/Word Puzzle Reveal Animation/Crossword")]
    public sealed class CrosswordCellRevealAnimation : WordCellRevealAnimation
    {
        [SerializeField]
        private AnimationCurve m_scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [SerializeField]
        private AnimationCurve m_alphaCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [SerializeField, Range(0f, 1f)]
        private float m_boldFontStyleThreshold = 0.5f;

        [SerializeField]
        private float m_minScale = 1f;

        [SerializeField]
        private float m_maxScale = 1.25f;

        [SerializeField, Range(0f, 1f)]
        private float m_startingAlpha = 0.15f;

        [SerializeField, Range(0f, 1f)]
        private float m_endingAlpha = 1f;

        public override void Animate(Text text, float t)
        {
            FontStyle fontStyle = (t <= m_boldFontStyleThreshold ? FontStyle.Bold : FontStyle.Normal);
            text.fontStyle = fontStyle;

            float scale = Mathf.LerpUnclamped(m_minScale, m_maxScale, m_scaleCurve.Evaluate(t));
            Color textColor = text.color;
            textColor.a = Mathf.LerpUnclamped(m_startingAlpha, m_endingAlpha, m_alphaCurve.Evaluate(t));

            text.transform.localScale = Vector3.one * scale;
            text.color = textColor;
        }
    }
}