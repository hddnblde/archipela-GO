using System.Collections.Generic;
using UnityEngine;
using GameModuleConfig = archipelaGO.Game.GameModuleConfig;
using DialogueCharacter = archipelaGO.VisualNovel.DialogueSystem.Elements.DialogueCharacter;

namespace archipelaGO.VisualNovel.StorySystem
{
    [CreateAssetMenu(fileName = "Plot", menuName = "archipelaGO/Game Module/Visual Novel Plot", order = 0)]
    public class Plot : GameModuleConfig
    {
        #region Fields
        [SerializeField]
        private WordBank m_wordBank = null;

        [SerializeField]
        private DialogueCharacter m_mainCharacter = null;

        [SerializeField]
        private List<Sprite> m_backgroundScenes = new List<Sprite>();

        [SerializeField]
        private List<Plotline> m_plotlines = new List<Plotline>();
        #endregion


        #region Property
        public int plotlineCount => m_plotlines.Count;

        public WordBank wordBank => m_wordBank;
        #endregion


        #region Public Method
        public (Narrative narrative, Sprite scene, DialogueCharacter mainCharacter) GetPlotline(int index)
        {
            if (index < 0 || index >= plotlineCount)
                return (null, null, m_mainCharacter);
            
            Plotline plotline = m_plotlines[index];
            Sprite scene = GetScene(plotline.backgroundIndex);
            Narrative narrative = plotline.narrative;

            return (narrative, scene, m_mainCharacter);
        }
        #endregion


        #region Helper Method
        private Sprite GetScene(int index)
        {
            if (index < 0 || index >= m_backgroundScenes.Count)
                return null;

            return m_backgroundScenes[index];
        }
        #endregion
    }
}