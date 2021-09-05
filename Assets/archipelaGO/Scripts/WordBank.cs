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
            #region Fields
            [SerializeField]
            private string m_title;

            [SerializeField]
            private string m_phoneticSpelling;

            [SerializeField]
            private List<string> m_keywords;

            [SerializeField]
            private PartOfSpeech m_partOfSpeech;

            [SerializeField]
            private AudioClip m_pronunciation;

            [SerializeField, TextArea, FormerlySerializedAs("m_description")]
            private string m_definition;
            #endregion


            #region Properties
            public string title => m_title;
            public string phoneticSpelling => m_phoneticSpelling;
            public PartOfSpeech partOfSpeech => m_partOfSpeech;
            public AudioClip pronunciationClip => m_pronunciation;
            public string definition => m_definition;
            #endregion


            #region Methods
            public bool IsMatched(string key)
            {
                string match;

                return IsMatched(key, out match);
            }

            public bool IsMatched(string key, out string match)
            {
                foreach (string keyword in GetSortedListOfKeywords())
                {
                    if (!string.Equals(keyword, key))
                        continue;
                    
                    match = keyword;
                    return true;
                }

                if (m_title.Contains(key))
                {
                    match = m_title;
                    return true;
                }

                match = string.Empty;
                return false;
            }

            public List<string> GetKeywords()
            {
                List<string> keywords = new List<string>();
                keywords.Add(m_title);
                keywords.AddRange(m_keywords);

                return keywords;
            }

            private List<string> GetSortedListOfKeywords() =>
                m_keywords.OrderByDescending(key => key.Length).ToList();
            #endregion
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
                where word.IsMatched(match)
                select word).FirstOrDefault();

            return m_words.IndexOf(matchedWord);
        }
        #endregion
    }
}