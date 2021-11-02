using Word = archipelaGO.WordBank.Word;

namespace archipelaGO.Puzzle
{
    public enum WordHintType
    {
        Word = 0,
        Definition = 1,
        Synonyms = 2,
        Antonyms = 3
    }

    public static class WordPuzzleHintGenerator
    {
        public static string GetHint(Word word, WordHintType type)
        {
            switch (type)
            {
                case WordHintType.Word:
                    return "";

                case WordHintType.Definition:
                    return $"({ word.partOfSpeechAbridged }) { word.definition }";

                case WordHintType.Synonyms:
                    return $"({ word.GetSynonyms() }";

                case WordHintType.Antonyms:
                    return $"({ word.GetAntonyms() }";

                default:
                    return string.Empty;
            }
        }
    }
}