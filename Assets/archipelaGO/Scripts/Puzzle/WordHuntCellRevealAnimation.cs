using UnityEngine;
using UnityEngine.UI;

namespace archipelaGO.Puzzle.RevealAnimation
{
    [CreateAssetMenu(fileName = "Word Hunt Reveal Animation", menuName = "archipelaGO/Game Module/Word Puzzle Reveal Animation/Word Hunt")]
    public sealed class WordHuntCellRevealAnimation : WordCellRevealAnimation
    {
        [SerializeField, Range(0f, 1f)]
        private float m_boldFontStyleThreshold = 0.5f;

        [SerializeField]
        private AnimationCurve m_curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [SerializeField]
        private float m_minScale = 1f;

        [SerializeField]
        private float m_maxScale = 1.25f;

        public override void Animate(Text text, float t)
        {
            FontStyle fontStyle = (t >= m_boldFontStyleThreshold ? FontStyle.Bold : FontStyle.Normal);
            text.fontStyle = fontStyle;

            t = m_curve.Evaluate(t);

            float scale = Mathf.LerpUnclamped(m_minScale, m_maxScale, t);
            text.transform.localScale = Vector3.one * scale;
        }
    }
}