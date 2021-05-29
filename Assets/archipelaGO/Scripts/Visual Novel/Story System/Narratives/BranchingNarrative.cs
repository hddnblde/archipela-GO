using System.Collections.Generic;
using UnityEngine;
using archipelaGO.VisualNovel.DialogueSystem;

namespace archipelaGO.VisualNovel.StorySystem.Narratives
{
    [CreateAssetMenu(fileName = "Branching Narrative", menuName = "archipelaGO/Visual Novel/Narrative/Branching", order = 1)]
    public sealed class BranchingNarrative : Narrative
    {
        #region Fields
        [SerializeField]
        private string m_title = string.Empty;

        [SerializeField]
        private List<NarrativeBranch> m_branches = new List<NarrativeBranch>();

        private string[] m_cachedChoices = null;
        #endregion


        #region Narrative Implementation
        public override bool GetChoices(out string title, out string[] choices)
        {
            title = m_title;
            choices = m_cachedChoices;

            return true;
        }

        public override Conversation GetConversation(int choice)
        {
            if (choice < 0 || choice >= m_branches.Count)
                return null;

            return m_branches[choice].GetConversation();
        }
        #endregion


        #region Data Structure
        [System.Serializable]
        private struct NarrativeBranch
        {
            [SerializeField]
            private string m_subtitle;

            [SerializeField]
            private Narrative m_narrative;

            public string subtitle => m_subtitle;
            public Conversation GetConversation() => m_narrative.GetConversation();
        }
        #endregion


        #region Scriptable Object Implementation
        private void OnEnable() => InitializeChoices();
        private void OnDisable() => m_cachedChoices = null;
        #endregion


        #region Internal Method
        private void InitializeChoices()
        {
            m_cachedChoices = new string[m_branches.Count];

            for (int i = 0; i < m_cachedChoices.Length; i++)
                m_cachedChoices[i] = m_branches[i].subtitle;
        }
        #endregion
    }
}