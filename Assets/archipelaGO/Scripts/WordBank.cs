using System.Collections.Generic;
using UnityEngine;

namespace achipelaGO
{
    [CreateAssetMenu(fileName = "Word Bank", menuName = "archipelaGO/Word Bank", order = 0)]
    public class WordBank : ScriptableObject
    {
        #region Fields
        [SerializeField]
        private List<Word> m_words = new List<Word>();
        #endregion


        #region Data Structure
        [System.Serializable]
        public struct Word
        {
            [SerializeField]
            private string m_title;

            [SerializeField, TextArea]
            private string m_description;

            public string title => m_title;
            public string description => m_description;
        }
        #endregion
    }
}