using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using archipelaGO.VisualNovel.DialogueSystem.Elements;

namespace archipelaGO.VisualNovel.DialogueSystem
{
    [System.Serializable]
    public class Dialogue
    {
        #region Field
        [SerializeField]
        private int m_characterIndex = 0;

        [SerializeField]
        private List<DialogueLine> m_lines = new List<DialogueLine>();
        #endregion


        #region Property
        public int characterIndex => m_characterIndex;
        public int lineCount => m_lines.Count;
        #endregion


        #region Public Method
        public DialogueLine GetLine(int index)
        {
            if (index < 0 || index >= lineCount)
                return new DialogueLine();

            return m_lines[index];
        }
        #endregion
    }
}