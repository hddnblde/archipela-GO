using UnityEngine;

namespace archipelaGO.VisualNovel.StorySystem
{
    public class Plotline
    {
        [SerializeField]
        private int m_backgroundIndex = -1;

        [SerializeField]
        private Narrative m_narrative = null;

        public int backgroundIndex => m_backgroundIndex;
        public Narrative narrative => m_narrative;
    }
}