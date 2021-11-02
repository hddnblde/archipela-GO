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
        public class BaseWord
        {
            [SerializeField]
            private string m_title;

            [SerializeField, TextArea, FormerlySerializedAs("m_description")]
            private string m_definition;

            [SerializeField]
            private PartOfSpeech m_partOfSpeech;

            public string title => m_title;
            public string definition => m_definition;
            public PartOfSpeech partOfSpeech => m_partOfSpeech;
            public string partOfSpeechAbridged => AbbreviatePartOfSpeech(m_partOfSpeech);

            private string AbbreviatePartOfSpeech(PartOfSpeech partOfSpeech)
            {
                switch (partOfSpeech)
                {
                    case PartOfSpeech.Noun:
                        return "n.";

                    case PartOfSpeech.Pronoun:
                        return "pron.";

                    case PartOfSpeech.Adjective:
                        return "adj.";

                    case PartOfSpeech.Verb:
                        return "v.";

                    case PartOfSpeech.Adverb:
                        return "adv.";

                    case PartOfSpeech.Conjuction:
                        return "conj.";
                    
                    case PartOfSpeech.Interjection:
                        return "interj.";

                    default:
                        return string.Empty;
                }
            }
        }

        [System.Serializable]
        public class Word : BaseWord
        {
            #region Fields
            [SerializeField]
            private string m_phoneticSpelling = string.Empty;

            [SerializeField]
            private AudioClip m_pronunciation = null;

            [SerializeField]
            private List<string> m_keywords = new List<string>();

            [SerializeField]
            private List<BaseWord> m_synonyms = new List<BaseWord>();

            [SerializeField]
            private List<BaseWord> m_antonyms = new List<BaseWord>();
            #endregion


            #region Properties
            public string phoneticSpelling => m_phoneticSpelling;
            public AudioClip pronunciationClip => m_pronunciation;
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

                if (title.Contains(key))
                {
                    match = title;
                    return true;
                }

                match = string.Empty;
                return false;
            }

            public List<string> GetKeywords()
            {
                List<string> keywords = new List<string>();
                keywords.Add(title);
                keywords.AddRange(m_keywords);

                return keywords;
            }

            private List<string> GetSortedListOfKeywords() =>
                m_keywords.OrderByDescending(key => key.Length).ToList();

            public string GetSynonyms() => GetConcatinatedListOfStrings(m_synonyms, ";\n");
            public string GetAntonyms() => GetConcatinatedListOfStrings(m_antonyms, ";\n");

            private string GetConcatinatedListOfStrings(List<BaseWord> listOfWords, string separator)
            {
                System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

                for (int i = 0; i < listOfWords.Count; i++)
                {
                    BaseWord word = listOfWords[i];
                    bool separate = (i < (listOfWords.Count - 1));
                    string text = $"{ word.title }—{ word.definition }";

                    if (separate)
                        text += separator;

                    stringBuilder.Append(word);
                }

                return stringBuilder.ToString();
            }
            #endregion
        }

        [System.Serializable]
        public enum PartOfSpeech
        {
            Noun = 0,
            Pronoun = 1,
            Adjective = 2,
            Verb = 3,
            Adverb = 4,
            Conjuction = 5,
            Interjection = 6
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