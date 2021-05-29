using System.Collections.Generic;
using UnityEngine;
using archipelaGO.VisualNovel.DialogueSystem;
using archipelaGO.VisualNovel.DialogueSystem.Elements;

namespace archipelaGO.VisualNovel.StorySystem.Narratives
{
    [CreateAssetMenu(fileName = "Linear Narrative", menuName = "archipelaGO/Visual Novel/Narrative/Linear", order = 0)]
    public sealed class LinearNarrative : Narrative
    {
        #region Fields
        [SerializeField]
        private List<DialogueCharacter> m_characters = new List<DialogueCharacter>();

        [SerializeField]
        private List<Dialogue> m_dialogues = new List<Dialogue>();
        #endregion


        #region Narrative Implementation
        public override bool GetChoices(out string title, out string[] choices)
        {
            title = string.Empty;
            choices = null;

            return false;
        }

        public override Conversation GetConversation(int choice) =>
            new Conversation(m_characters, m_dialogues);
        #endregion
    }
}