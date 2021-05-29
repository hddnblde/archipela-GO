using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using archipelaGO.VisualNovel.DialogueSystem.Elements;

namespace archipelaGO.VisualNovel.DialogueSystem
{
    [System.Serializable]
    public class Dialogue : IEnumerable
    {
        #region Field
        [SerializeField]
        private int m_characterIndex = 0;

        [SerializeField]
        private List<DialogueLine> m_lines = new List<DialogueLine>();
        #endregion


        #region Property
        public int characterIndex => m_characterIndex;
        #endregion


        #region Enumerable Implementation
        public IEnumerator GetEnumerator() => m_lines.GetEnumerator();
        #endregion
    }
}