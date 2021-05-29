using System.Collections.Generic;
using UnityEngine;

namespace archipelaGO.VisualNovel.StorySystem
{
    [CreateAssetMenu(fileName = "Plot", menuName = "archipelaGO/Visual Novel/Plot")]
    public class Plot : ScriptableObject
    {
        #region Fields
        [SerializeField]
        private List<Sprite> m_backgroundScenes = new List<Sprite>();

        [SerializeField]
        private List<Plotline> m_plotlines = new List<Plotline>();
        #endregion


        #region Properties
        public int scenesCount => m_backgroundScenes.Count;
        public int plotlineCount => m_plotlines.Count;
        #endregion


        #region Public Methods
        public Sprite GetScene(int index)
        {
            if (index < 0 || index >= scenesCount)
                return null;

            return m_backgroundScenes[index];
        }

        public Plotline GetPlotline(int sequenceIndex)
        {
            if (sequenceIndex < 0 || sequenceIndex >= plotlineCount)
                return null;

            return m_plotlines[sequenceIndex];
        }
        #endregion
    }
}