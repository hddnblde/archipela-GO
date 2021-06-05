using System.Collections.Generic;
using archipelaGO.VisualNovel.DialogueSystem.Elements;

namespace archipelaGO.VisualNovel.DialogueSystem
{
    public class Conversation
    {
        public Conversation (List<DialogueCharacter> characters, List<Dialogue> dialogues) =>
            (m_characters, m_dialogues) = (characters, dialogues);

        #region Fields
        private List<DialogueCharacter> m_characters = new List<DialogueCharacter>();
        private List<Dialogue> m_dialogues = new List<Dialogue>();
        #endregion


        #region Property
        public int dialogueCount => m_dialogues?.Count ?? 0;
        #endregion


        #region Public Method
        public (DialogueCharacter character, Dialogue dialogue) GetDialogueLine(int index)
        {
            if (m_dialogues == null || m_characters.Count <= 0)
                return (null, null);
            
            if (index < 0 || index >= dialogueCount)
                return (null, null);
            
            Dialogue dialogue = m_dialogues[index];
            
            if (dialogue.characterIndex < 0 || dialogue.characterIndex >= m_characters.Count)
                return (null, dialogue);

            DialogueCharacter character = m_characters[dialogue.characterIndex];

            return (character, dialogue);
        }
        #endregion
    }
}