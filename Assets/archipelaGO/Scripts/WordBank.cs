using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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

            [SerializeField, TextArea, FormerlySerializedAs("m_description")]
            private string m_definition;

            public string title => m_title;
            public PartOfSpeech partOfSpeech => m_partOfSpeech;
            public string definition => m_definition;
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


        #region Public Methods
        public Word GetWord(int index)
        {
            if (index < 0 || index >= wordCount)
                return new Word();

            return m_words[index];
        }

        public int GetWordIndex(string match)
        {
            var matchedWord = (from word in m_words
                where word.title.Contains(match)
                select word).FirstOrDefault();

            return m_words.IndexOf(matchedWord);
        }
        #endregion
    }
}