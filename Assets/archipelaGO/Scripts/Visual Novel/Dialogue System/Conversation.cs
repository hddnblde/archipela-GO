using System.Collections.Generic;
using archipelaGO.VisualNovel.DialogueSystem.Elements;

namespace archipelaGO.VisualNovel.DialogueSystem
{
    public class Conversation
    {
        public Conversation (CharacterSet characters, List<Dialogue> dialogues) =>
            (m_characters, m_dialogues) = (characters, dialogues);

        #region Fields
        private CharacterSet m_characters = null;
        private List<Dialogue> m_dialogues = new List<Dialogue>();
        #endregion


        #region Property
        public int dialogueCount => m_dialogues?.Count ?? 0;
        #endregion


        #region Public Method
        public (DialogueCharacter character, DialogueCharacterBlocking blocking, Dialogue dialogue) GetDialogueLine(int index)
        {
            if (m_dialogues == null || m_characters.Count <= 0)
                return (null, DialogueCharacterBlocking.StageLeft, null);
            
            if (index < 0 || index >= dialogueCount)
                return (null, DialogueCharacterBlocking.StageLeft, null);
            
            Dialogue dialogue = m_dialogues[index];
            
            if (dialogue.characterIndex < 0 || dialogue.characterIndex >= m_characters.Count)
                return (null, dialogue.blocking, dialogue);

            DialogueCharacter character = m_characters.GetCharacter(dialogue.characterIndex);

            return (character, dialogue.blocking, dialogue);
        }
        #endregion
    }
}