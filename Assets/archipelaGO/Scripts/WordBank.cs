using System.Collections.Generic;
using UnityEngine;

namespace archipelaGO
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

            [SerializeField]
            private PartOfSpeech m_partOfSpeech;

            [SerializeField, TextArea]
            private string m_description;

            public string title => m_title;
            public PartOfSpeech partOfSpeech => m_partOfSpeech;
            public string description => m_description;
        }

        [System.Serializable]
        public enum PartOfSpeech
        {
            Noun = 0,
            Pronoun = 1,
            Adjective = 2,
            Verb = 3,
            Adverb = 4
        }
        #endregion


        #region Property
        public int wordCount => m_words?.Count ?? 0;
        #endregion


        #region Public Method
        public Word GetWord(int index)
        {
            if (index < 0 || index >= wordCount)
                return new Word();

            return m_words[index];
        }

        public int LongestCharacterCount()
        {
            int count = 0;

            foreach (Word word in m_words)
            {
                int currentCharacterCount = word.title.Length;

                if (currentCharacterCount > count)
                    count = currentCharacterCount;
            }

            return count;
        }
        #endregion
    }
}